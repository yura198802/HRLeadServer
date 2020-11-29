using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Service.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata;
using SqlKata.Execution;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Формирование данных по умолчанию
    /// </summary>
    public class ReportEngineDefaultData : IReportEngineData
    {
        protected readonly ReportDbContext ReportDbContext;
        protected readonly IReportManager ReportManager;
        protected readonly IConnectorManager ConnectorManager;

        public ReportEngineDefaultData(ReportDbContext reportDbContext, IReportManager reportManager, IConnectorManager connectorManager)
        {
            ReportDbContext = reportDbContext;
            ReportManager = reportManager;
            ConnectorManager = connectorManager;
        }


        /// <summary>
        /// Получить данные для формы редактирования данных
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p)
        {
            try
            {
                var formModel = await ReportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
                if (formModel == null)
                    return null;
                var fileds = (await ReportManager.GetFieldsFormAsync(p.UserName, p.FormId, false, fields =>
                    fields.Where(f => !(f.IsVirtual ?? false)))).ToList();
                if (fileds.Count == 0)
                    return null;
                using var connection = ConnectorManager.GetConnection();
                var typeControls = new[] { TypeControl.Details };

                var db = new QueryFactory(connection, ConnectorManager.Compiler);

                var query = (await GenerateSqlView(fileds.Where(f => !typeControls.Contains(f.TypeControl)),
                    formModel.TableName));
                if (p.FormIdByDetail != 0)
                    query.Where(p.FieldWhere, "=", p.FormIdByDetail);
                await AddWhereList(query, p.FilterModel, formModel.Id, p.UserName);
                await ReportManager.AddFilterUser(formModel.Id, p.FilterModel, p.UserName);
                var resQuery = await db.FromQuery(query).GetAsync();


                return resQuery.Cast<IDictionary<string, object>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        /// <summary>
        /// Получить модель для редактирования данных
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual async Task<IDictionary<string, object>> GetDataEditModel(BaseModelReportParam p)
        {
            var formModel = await ReportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
            if (formModel == null)
                return null;
            var fileds = (await ReportManager.GetFieldsFormAsync(p.UserName, p.FormId, false)).ToList();
            using var connection = ConnectorManager.GetConnection();
            var typeControls = new[] { TypeControl.Details };

            var db = new QueryFactory(connection, ConnectorManager.Compiler);

            var query = await GenerateSqlViewEditModel(fileds.Where(f => !typeControls.Contains(f.TypeControl)),
                formModel.TableName, p.ModelId);
            var resQuery = await db.FromQuery(query).FirstOrDefaultAsync();

            var results = resQuery as IDictionary<string, object>;

            return results;
        }

        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> ValidateModel(dynamic saveModel, FormModelDto formModelId)
        {
            var result = new ResultCrmDb();
            if (!(saveModel is JObject))
                return result;
            var fields = await ReportDbContext.Field.Where(f => f.FormModelId == formModelId.Id).ToListAsync();
            var saveRuleModel = saveModel as JObject;

            var models = ReportDbContext.ValidationRuleEntity.Where(f => f.FormModelId == formModelId.Id && f.IsDeleted == false);
            foreach (var ruleEntity in models)
            {
                if (ruleEntity.TypeValidation == TypeValidation.Component)
                    continue;
                var sql = ruleEntity.Content;
                foreach (var jObject in saveRuleModel)
                {
                    sql = sql.Replace($"[{jObject.Key}]", jObject.Value.ToObject<string>());
                }

                foreach (var field in fields)
                {
                    sql = sql.Replace($"[{field.Name.GetFieldName()}]", field.TypeControl == TypeControl.NumericEdit ? "0" : "null");
                }

                using (var connection = ConnectorManager.GetConnection())
                {
                    var res = await connection.QueryFirstOrDefaultAsync<int>(sql);
                    if (res > 0)
                        result.AddError(ruleEntity.Name, ruleEntity.ToolTip);
                }
            }


            return result;
        }

        /// <summary>
        /// Сохранение измененных данных с фронта
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="formModelSave">Модель для сохранения данных</param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public virtual async Task<ResultCrmDb> SaveModels(FormModelDto formModel, string userName, SaveModelArgs formModelSave)
        {
            var result = new ResultCrmDb();
            var modelSave = formModelSave.SaveModel;
            if (modelSave == null)
                return result;
            var fieldsBase = (await ReportManager.GetFieldsFormAsync(userName, formModel.Id, false, fields =>
                fields.Where(f => !(f.IsVirtual ?? false) && f.TypeControl != TypeControl.Details))).ToList();
            fieldsBase = fieldsBase.WhereFieldAccess(modelSave).Where(f => f.TypeAccec == TypeAccec.Full).ToList();
            var fileds = fieldsBase.Where(f => (((f.IsDetail ?? false) && !(f.IsVirtual ?? false))
                                                && modelSave.Keys.FirstOrDefault(ff => ff == ColumnHelper.GetFieldName(f.Name)) != null) || (f.IsKey ?? false)).ToList();

            fileds = fileds.Where(f => f.TypeAccec == TypeAccec.Full).ToList();
            var fieldKey = fileds.FirstOrDefault(f => f.IsKey ?? false);
            if (fieldKey == null)
                return result;

            var valueKey = modelSave[fieldKey.Name]?.ToString();
            bool isNew = string.IsNullOrWhiteSpace(valueKey) || valueKey == "0";

            using var connection = ConnectorManager.GetConnection();
            var db = new QueryFactory(connection, ConnectorManager.Compiler);

            var query = db.Query(formModel.TableName);

            if (isNew)
            {
                int i = 0;
                await UpdateFieldModel(modelSave, fieldsBase, true, userName);
                var saves = new Dictionary<string, object>(modelSave.Where(f =>
                    fieldsBase.Select(s => ColumnHelper.GetFieldName(s.Name)).Contains(f.Key)));
                var id = await query.InsertGetIdAsync<int>(saves);
                result.Result = id;
            }
            else
            {
                await UpdateFieldModel(modelSave, fieldsBase, false, userName);
                var saves = new Dictionary<string, object>(modelSave.Where(f =>
                    fieldsBase.Select(s => ColumnHelper.GetFieldName(s.Name)).Contains(f.Key)));
                query.Where(fieldKey.Name, "=", valueKey).AsUpdate(saves);
                await db.ExecuteAsync(query);
                result.Result = valueKey;
            }

            var queryModel = await GenerateSqlView(fieldsBase, formModel.TableName);
            queryModel.Where($"{formModel.TableName}.{fieldKey.Name}", "=", result.Result);
            queryModel = db.FromQuery(queryModel);
            var resultModel = (await queryModel.GetAsync()).FirstOrDefault();
            result.Result = resultModel == null ? "" : JsonConvert.SerializeObject(resultModel);
            return result;
        }

        protected async Task<string> GetReturnDetailModels(string userName, FormModelDto formModel, IDictionary<string, object> modelSave, string keyField, object value)
        {
            var fieldsBase = (await ReportManager.GetFieldsFormAsync(userName, formModel.Id, false, fields =>
                fields.Where(f => !(f.IsVirtual ?? false) && f.TypeControl != TypeControl.Details))).ToList();
            fieldsBase = fieldsBase.WhereFieldAccess(modelSave).Where(f => f.TypeAccec == TypeAccec.Full).ToList();

            using var connection = ConnectorManager.GetConnection();
            var db = new QueryFactory(connection, ConnectorManager.Compiler);
            var queryModel = await GenerateSqlView(fieldsBase, formModel.TableName);
            queryModel.Where($"{formModel.TableName}.{keyField}", "=", value);
            queryModel = db.FromQuery(queryModel);
            var resultModel = (await queryModel.GetAsync()).FirstOrDefault();
            return resultModel == null ? "" : JsonConvert.SerializeObject(resultModel);
        }

        private async Task UpdateFieldModel(IDictionary<string, object> dictionary, List<FieldAccessDto> fields, bool isNew, string userName)
        {
            var defaultValue = await GetStaticDefaultValue(userName);
            if (isNew)
            {
                var filedsIsNew = fields.Where(f => f.Name.IsDefaultValueByNew());
                foreach (var field in filedsIsNew)
                {
                    if (!dictionary.ContainsKey(field.Name.GetFieldName()))
                        dictionary.Add(field.Name.GetFieldName(), field.Name.GetDefaultValue(defaultValue));
                    else dictionary[field.Name.GetFieldName()] = field.Name.GetDefaultValue(defaultValue);
                }
            }
            var filedByEdit = fields.Where(f => f.Name.IsDefaultValueByEdit());
            foreach (var field in filedByEdit)
            {
                if (!dictionary.ContainsKey(field.Name.GetFieldName()))
                    dictionary.Add(field.Name.GetFieldName(), field.Name.GetDefaultValue(defaultValue));
                else dictionary[field.Name.GetFieldName()] = field.Name.GetDefaultValue(defaultValue);
            }
        }

        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveEntity(FormModelDto formModel, string userName, string[] key)
        {
            var result = new ResultCrmDb();
            var fileds = (await ReportManager.GetFieldsFormAsync(userName, formModel.Id, false)).Where(f => (f.IsDetail ?? false) && !(f.IsVirtual ?? false)).ToList();

            var fieldKey = fileds.FirstOrDefault(f => f.IsKey ?? false);
            if (fieldKey == null)
                return result;

            using var connection = ConnectorManager.GetConnection();
            var db = new QueryFactory(connection, ConnectorManager.Compiler);
            var query = db.Query(formModel.TableName).WhereIn(fieldKey.Name, key).AsDelete();
            await db.ExecuteAsync(query);
            return result;
        }

        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <param name="formModel">Модель</param>
        /// <returns></returns>
        public async Task<IEnumerable<SolutionModel>> GetDetailInfoModels(FormModelDto formModel)
        {
            using var connection = ConnectorManager.GetConnection();

            var db = new QueryFactory(connection, ConnectorManager.Compiler);
            var query = db.Query("field as f").Join("formModel as m", "m.Id", "f.FormModelDetailId")
                .Where("FormModelId", formModel.Id)
                .Select(
                    "m.Caption as Text"
                    , "m.Id as IdModel"
                    , "f.Name as FieldWhere"
                    , "m.VueComponent"
                );
            var models = await query.GetAsync<SolutionModel>();
            return models.Select(s =>
            {
                s.VueComponent = string.IsNullOrEmpty(s.VueComponent) ? "DesignerDictionary" : s.VueComponent;
                return s;
            });
        }

        public virtual async Task<IEnumerable<FieldAccessDto>> GetFields(BaseModelReportParam parametr)
        {
            return await ReportManager.GetFieldsFormWithProfileAsync(parametr.UserName, parametr.FormId, false,
                fields => fields.Where(f => (f.IsVisibleList ?? false) || string.IsNullOrWhiteSpace(f.Express)));
        }

        private async Task<Query> GenerateSqlViewEditModel(IEnumerable<FieldAccessDto> fieldAccess, string formModel, string id)
        {
            var fieldAccessDtos = fieldAccess as FieldAccessDto[] ?? fieldAccess.ToArray();

            var tablePk = (await GetTablePrimaryKey(new string[] { }, formModel)).FirstOrDefault();
            var queryTable =
                (await GetTables(
                    fieldAccessDtos.Select(s => new ColumnTable
                    { ColumnName = s.Name, TableName = s.TableName }).Where(f =>
                      !string.IsNullOrEmpty(f.TableName) &&
                      !string.IsNullOrWhiteSpace(f.ColumnName)).ToArray(), formModel))
                .Where($"{formModel}.{tablePk?.ColumnName}", "=", id);

            GetColumn(fieldAccessDtos.Where(f => !(f.IsVirtual ?? false) && (f.IsDetail ?? false)), formModel, queryTable);
            return queryTable;
        }

        private async Task<Query> GenerateSqlView(IEnumerable<FieldAccessDto> fieldAccess, string formModel)
        {
            var fieldAccessDtos = fieldAccess as FieldAccessDto[] ?? fieldAccess.ToArray();
            var queryTable =
                await GetTables(
                    fieldAccessDtos.Select(s => new ColumnTable
                    { ColumnName = s.Name, TableName = s.TableName, IsOneToOne = s.IsOneToOne ?? false }).Where(f =>
                       !string.IsNullOrEmpty(f.TableName) &&
                       !string.IsNullOrWhiteSpace(f.ColumnName)).ToArray(), formModel);
            GetColumn(fieldAccessDtos, formModel, queryTable);

            return queryTable;
        }

        private async Task<Query> GetTables(ColumnTable[] tableName, string formModel)
        {
            var tablePk = (await GetTablePrimaryKey(tableName.Select(s => s.TableName).ToArray(), formModel)).ToList();
            var queryBuilder = new Query($"{formModel} as {formModel}");
            foreach (var tbJoin in tableName.Where(f => f.TableName != formModel).GroupBy(g => g.TableName)
                .Select(s => new { TableName = s.Key, s.FirstOrDefault()?.ColumnName }))
            {

                var pk = tablePk.FirstOrDefault(f => f.TableName == tbJoin.TableName);
                if (pk == null)
                    continue;

                var columnJoin = GetJoinTable(tbJoin.ColumnName);
                var columnPk = GetPkTable(tbJoin.ColumnName, pk.ColumnName);

                queryBuilder.LeftJoin($"{tbJoin.TableName} as {tbJoin.TableName}", $"{tbJoin.TableName}.{columnPk}",
                    $"{formModel}.{columnJoin}");
            }

            return queryBuilder;
        }

        private string GetColumnBySelect(string columnJoin, string tableName, bool isDetail, Query query, bool isSorting)
        {
            var column = GetFieldSelectTable(columnJoin, isDetail);
            string columnResult = $"{tableName}.{column}";

            if (!columnJoin.Contains("{"))
            {
                query.Select(columnResult);
                if (isSorting)
                    query.OrderBy(columnResult);
                return columnResult;
            }

            var obj = JObject.Parse(columnJoin);
            var alias = obj["Alias"];
            if (alias == null)
            {
                query.Select(columnResult);
                if (isSorting)
                    query.OrderBy(columnResult);
                return columnResult;
            }
            query.SelectRaw(column);
            if (isSorting)
                query.OrderByRaw(obj["Field"]?.ToString());
            return column;
        }

        private string GetFieldSelectTable(string columnJoin, bool isDetail)
        {
            if (!columnJoin.Contains("{"))
                return columnJoin;
            var obj = JObject.Parse(columnJoin);
            var alias = obj["Alias"] ?? obj["Fk"];
            return string.Format("{0} as {1}", isDetail ? obj["DetailField"] : obj["Field"], alias);
        }

        private string GetJoinTable(string columnJoin)
        {
            if (!columnJoin.Contains("{"))
                return columnJoin;
            var obj = JObject.Parse(columnJoin);
            return obj["Fk"]?.ToString();
        }

        private string GetPkTable(string columnJoin, string columnPk)
        {
            if (!columnJoin.Contains("{"))
                return columnPk;
            var obj = JObject.Parse(columnJoin);
            return obj["Pk"]?.ToString();
        }

        private void GetColumn(IEnumerable<FieldAccessDto> fieldAccess, string formModel, Query queryTable)
        {
            var strings = fieldAccess.Select(s =>
                    GetColumnBySelect(s.Name, (string.IsNullOrWhiteSpace(s.TableName) ? formModel : s.TableName),
                        false, queryTable, s.Sorting != null && s.Sorting != 0))
                .ToArray();
        }

        private async Task<IEnumerable<ColumnTable>> GetTablePrimaryKey(string[] table, string formModel)
        {
            var tables = string.Join(',', table.Union(new[] { formModel }).Select(s => $"'{s}'"));
            using var connection = ConnectorManager.GetConnection();
            return await connection.QueryAsync<ColumnTable>(string.Format(GetTablePk, connection.Database, tables));
        }

        private async Task AddWhereList(Query query, IDictionary<string, object> jObject, int formModelId, string userName)
        {
            var models = await ReportDbContext.FilterFormModel
                .Include(c => c.FormModel)
                .Where(f => f.FormModel.Id == formModelId).ToListAsync();
            if (jObject != null)
                foreach (var property in jObject)
                {
                    var modelFilter = models.FirstOrDefault(f => f.Sysname == property.Key);
                    if (modelFilter == null)
                        continue;
                    var where = string.Format(modelFilter.Where, property.Value?.GetType() == typeof(DateTime) ?
                        (((DateTime)property.Value).AddHours(+3)).ToString("yyyy-MM-dd")
                        : property.Value?.ToString());
                    query.WhereRaw(where);
                }

            var staticValue = await GetStaticDefaultValue(userName);
            foreach (var model in models.Where(f => !(f.IsVisibleFilter ?? false)))
            {
                foreach (var o in staticValue)
                {
                    model.Where = model.Where.Replace(o.Key, o.Value?.ToString());
                }

                query.WhereRaw(model.Where);
            }
        }


        protected async Task<IDictionary<string, object>> GetStaticDefaultValue(string userName)
        {
            var values = new Dictionary<string, object>();
            var userId = await ReportDbContext.User.Where(f => f.Account == userName).Select(s => s.Id).FirstOrDefaultAsync();
            values.Add("{CurrentUser}", userId);
            values.Add("{DateTime.Now}", DateTime.Now);
            return values;
        }


        private const string GetTablePk = @"SELECT COLUMN_NAME as ColumnName,TABLE_NAME AS TableName
                                                FROM INFORMATION_SCHEMA.COLUMNS
                                                WHERE TABLE_NAME in ({1}) AND TABLE_SCHEMA ='{0}' and COLUMN_KEY = 'PRI'";


    }
}
