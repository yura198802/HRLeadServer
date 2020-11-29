using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Exceptions;
using SqlKata;
using SqlKata.Execution;

namespace Monica.Core.Service.ReportEngine
{
    public class SingleForm : ISingleForm
    {
        private readonly IColumnCreater _columnCreater;
        private readonly IAccessManager _accessManager;
        private readonly ReportDbContext _reportDbContext;
        private readonly IReportManager _reportManager;
        private readonly IConnectorManager _connectorManager;

        public SingleForm(IColumnCreater columnCreater, ReportDbContext reportDbContext, IAccessManager accessManager, IReportManager reportManager, IConnectorManager connectorManager)
        {
            _columnCreater = columnCreater;
            _reportDbContext = reportDbContext;
            _accessManager = accessManager;
            _reportManager = reportManager;
            _connectorManager = connectorManager;
        }
        /// <summary>
        /// Получить список полей для редактирования
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<string> GetProppertys(int formModelId, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModelId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var fields = await _reportManager.GetFieldsFormWithProfileAsync(userName, formModelId, false,
                queryable => queryable.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));

            return await _columnCreater.GetProperty(fields, formModelDto);
        }
        /// <summary>
        /// Получить данные для формы
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetFormData(int formModelId, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModelId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var fields = await _reportManager.GetFieldsFormWithProfileAsync(userName, formModelId, false,
                queryable => queryable.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));

            using var connection = _connectorManager.GetConnection();
            using var db = new QueryFactory(connection, _connectorManager.Compiler);
            var dictionaryModel = new Dictionary<string,object>();
            var data = (await db.Query(formModelDto.TableName).GetAsync())?.Cast<IDictionary<string, object>>().ToList();
            if (data == null)
                return null;
            foreach (var fieldAccessDto in fields.Where(f => f.IsDetail == true && f.TypeGroup == TypeGroup.None && !(f.IsVirtual ?? false)))
            {
                var valueData =
                    data.FirstOrDefault(f => f.ContainsKey("SysName") && f["SysName"]?.ToString() == fieldAccessDto.Name);
                dictionaryModel.Add(fieldAccessDto.Name, valueData?["Value"]);
            }

            return dictionaryModel;
        }
        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public async Task<ResultCrmDb> SaveModel(int formModelId, string userName, IDictionary<string, object> formData)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var fields = await _reportManager.GetFieldsFormWithProfileAsync(userName, formModelId, false,
                queryable => queryable.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));
            var createSql = string.Format(CreateTable, formModelDto.TableName);
            using var connection = _connectorManager.GetConnection();
            var userId = await _reportDbContext.User.Where(f => f.Account == userName).Select(s => s.Id).FirstOrDefaultAsync();
            await connection.ExecuteAsync(createSql);
            using var db = new QueryFactory(connection, _connectorManager.Compiler);
            foreach (var fieldAccessDto in fields.Where(f => f.IsDetail == true && f.TypeGroup == TypeGroup.None && !(f.IsVirtual ?? false)))
            {
                var query = new Query(formModelDto.TableName).Where("SysName", fieldAccessDto.Name);
                var res = await db.FromQuery(query).FirstOrDefaultAsync() as IDictionary<string, object>;
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("SysName", fieldAccessDto.Name);
                dictionary.Add("Name", fieldAccessDto.DisplayName);
                dictionary.Add("Value", formData[fieldAccessDto.Name]);
                dictionary.Add("UserId", userId);
                if (res == null)
                    await db.Query(formModelDto.TableName).InsertAsync(dictionary);
                else
                {
                    dictionary.Add("Id", res["Id"]);
                    await db.Query(formModelDto.TableName).Where("id", res["Id"]).UpdateAsync(dictionary);
                }
            }
            return new ResultCrmDb();
        }
        /// <summary>
        /// Создать таблицу и заполнить ее данными на основании модели данных
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task CreateAndFillTableAsync(int formModelId, string userName)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto.TypeEditor != TypeField.Edit)
                return;
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var fields = await _reportManager.GetFieldsFormWithProfileAsync(userName, formModelId, false,
                queryable => queryable.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));
            var createSql = string.Format(CreateTable, formModelDto.TableName);
            using var connection = _connectorManager.GetConnection();
            var userId = await _reportDbContext.User.Where(f => f.Account == userName).Select(s => s.Id).FirstOrDefaultAsync();
            await connection.ExecuteAsync(createSql);
            using var db = new QueryFactory(connection, _connectorManager.Compiler);
            foreach (var fieldAccessDto in fields.Where(f => f.IsDetail == true && f.TypeGroup == TypeGroup.None && !(f.IsVirtual ?? false)))
            {
                var query = new Query(formModelDto.TableName).Where("SysName", fieldAccessDto.Name).AsCount();
                var res = await db.FromQuery(query).CountAsync<int>();
                if (res != 0)
                    continue;
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("SysName", fieldAccessDto.Name);
                dictionary.Add("Value", string.Empty);
                dictionary.Add("UserId", userId);
                dictionary.Add("Name", fieldAccessDto.DisplayName);
                await db.Query(formModelDto.TableName).InsertAsync(dictionary);
            }
        }


        private const string CreateTable = @"CREATE TABLE IF NOT EXISTS `{0}` (
                                              Id int(11) NOT NULL AUTO_INCREMENT COMMENT 'Sysid',
                                              SysName varchar(64) DEFAULT NULL COMMENT 'Ключ',
                                              Value varchar(2000) DEFAULT NULL COMMENT 'Значение',
                                              Name varchar(255) DEFAULT NULL COMMENT 'Коментарый',
                                              UserId int(11),
                                              PRIMARY KEY (Id)
                                            )
                                            ENGINE = INNODB,
                                            CHARACTER SET utf8,
                                            COLLATE utf8_general_ci;";
    }
}
