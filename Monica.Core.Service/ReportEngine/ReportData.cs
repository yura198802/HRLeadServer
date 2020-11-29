using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Exceptions;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Service.Extension;
using Monica.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Получение данных универсальным путем
    /// </summary>
    public class ReportData : IReportData
    {
        private readonly ReportDbContext _reportDbContext;
        private readonly IReportManager _reportManager;
        private readonly IAccessManager _accessManager;
        private readonly IColumnCreater _columnCreater;
        private readonly IButtonCreater _buttonCreater;

        public ReportData(ReportDbContext reportDbContext, IReportManager reportManager, IAccessManager accessManager, IColumnCreater columnCreater, IButtonCreater buttonCreater)
        {
            _reportDbContext = reportDbContext;
            _reportManager = reportManager;
            _accessManager = accessManager;
            _columnCreater = columnCreater;
            _buttonCreater = buttonCreater;
        }

        public async Task<ResultCrmDb> AfterSaveModel(int formId, int idModel, string userName)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IActionAfterSave>(formModelDto.TableName, true);
            if (report == null)
                return new ResultCrmDb();
            return await report.BeforeSave(idModel, userName);
        }
        public async Task<ResultCrmDb> ActionImportFile(int formId, string userName, ActionArgs otherInfo, string sysname, List<IFormFile> files)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IActionImportFileFormModel>(sysname);
            if (report == null)
                return new ResultCrmDb();
            return await report.ActionImport(otherInfo, userName, formId,files);
        }

        public async Task<ResultCrmDb> Action(int formId, string userName, ActionArgs otherInfo, string sysname)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IActionBtnFormModel>(sysname);
            if (report == null)
                return new ResultCrmDb();
            return await report.Action(otherInfo, userName, formId);
        }

        public async Task<IDictionary<string,object>> ActionByDetailModel(int formId, string userName, IDictionary<string, object> formModel, string sysname)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IActionBtnDetailFormModel>(sysname);
            if (report == null)
                return null;
            return await report.Action(formModel, userName, formId);
        }

        public async Task<List<ActionExportResult>> ActionExportResult(int formId, string userName, ActionArgs otherInfo, string sysname)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IActionExportFile>(sysname);
            if (report == null)
                return new List<ActionExportResult>();
            return await report.ActionExportFile(otherInfo, userName, formId);
        }


        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        public async Task<ReportResultData> GetDataList(BaseModelReportParam parametr)
        {
            var resultData = new ReportResultData();
            var accessForm = await _accessManager.GetAccessFormAsync(parametr.UserName, parametr.FormId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == parametr.FormId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);

            resultData.FormModel = formModelDto;
            resultData.FieldAccess = await report.GetFields(parametr);

            var resultDataFieldAccess = resultData.FieldAccess as FieldAccessDto[] ?? resultData.FieldAccess.ToArray();

            resultData.Data = GetDataAccess(await report.GetDataList(parametr), resultDataFieldAccess);
            resultData.Columns = await _columnCreater.GetColumns(resultDataFieldAccess, formModelDto);
            resultData.KeyField = resultDataFieldAccess.FirstOrDefault(f => f.IsKey ?? false)?.Name;
            resultData.FormProperty = await _columnCreater.GetProperty(resultDataFieldAccess, formModelDto);
            return resultData;
        }


        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SolutionModel>> GetDetailInfoModels(int formModel, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);

            return await report.GetDetailInfoModels(formModelDto);
        }

        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public async Task<JArray> GetButtons(int formId, string userName, IDictionary<string, object> filterData)
        {
            var buttons = (await _reportManager.GetButtonsAsync(userName, formId, false)).Where(f => !f.IsDetail);
            return await _buttonCreater.GetButtonJson(buttons, filterData);
        }


        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        public async Task<JArray> GetButtonsDetail(int formId, string userName)
        {
            var buttons = (await _reportManager.GetButtonsAsync(userName, formId, false)).Where(f => f.IsDetail);
            return await _buttonCreater.GetButtonJson(buttons, null);
        }

        public async Task<ResultCrmDb> SaveModel(SaveModelArgs saveModel, int typeEditor, int formModel, string userName)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            
            var actionBeforeSave = AutoFac.ResolveNamed<IActionBeforeSave>(formModelDto.TableName, true);
            if (actionBeforeSave != null)
                saveModel.SaveModel = await actionBeforeSave.ActionBefore(saveModel.SaveModel);
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            return await report.SaveModels(formModelDto, userName, saveModel);
        }



        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> ValidateModel(dynamic saveModel, int formModelId)
        {
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            return await report.ValidateModel(saveModel, formModelDto);
        }

        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveEntity(int formModel, string userName, string[] key)
        {
            var accessForm = await _accessManager.GetAccessFormAsync(userName, formModel);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                (await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModel)).Map<FormModelDto>();
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");
            var report = AutoFac.ResolveNamed<IReportEngineData>(string.IsNullOrWhiteSpace(formModelDto.NameClassDataEngine) ? nameof(ReportEngineDefaultData) : formModelDto.NameClassDataEngine);
            var actionBeforeDelete = AutoFac.ResolveNamed<IActionBeforeDelete>(formModelDto.TableName,true);
            if (actionBeforeDelete != null)
            {
                var res = await actionBeforeDelete.Action(userName, formModelDto, key);
                if (!res.Succeeded)
                    return res;
            }

            return await report.RemoveEntity(formModelDto, userName, key);

        }


        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        public async Task<ReportResultData> GetEditModel(BaseModelReportParam parametr)
        {
            var resultData = new ReportResultData();
            var accessForm = await _accessManager.GetAccessFormAsync(parametr.UserName, parametr.FormId);
            if (accessForm != null && (int)accessForm.TypeAccec < 2)
                throw new UserMessageException("Вы запросили данные режима, на который у вас не открыты права");
            var formModelDto =
                Mapper.Map<FormModelDto>((
                    await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == parametr.FormId)));
            if (formModelDto == null)
                throw new UserMessageException("Не удалось определить форму");

            var fieldss = (await _reportManager.GetFieldsFormWithProfileAsync(parametr.UserName, parametr.FormId, false, fields => fields.Where(f => f.IsDetail ?? false))).ToList();
            resultData.FormModel = formModelDto;
            // resultData.Buttons = await _reportManager.GetButtonsAsync(parametr.UserName, parametr.FormId, false);
            var report = AutoFac.ResolveNamed<IReportEngineData>(formModelDto.NameClassDataEngine);


            var results = report == null ? null : await report.GetDataEditModel(parametr);
            fieldss = fieldss.WhereFieldAccess(results);
            resultData.FieldAccess = fieldss;
            resultData.FormProperty = await _columnCreater.GetProperty(fieldss, formModelDto);
            resultData.Data = report == null || results == null ? "" : GetModelAccess(results, fieldss);

            return resultData;
        }



        private string GetDataAccess(IEnumerable<IDictionary<string, object>> data, IEnumerable<FieldAccessDto> fields)
        {
            var jArray = new JArray();
            if (data == null)
                return jArray.ToString();
            foreach (var model in data)
            {
                var jObject = new JObject();
                foreach (var fieldAccessDto in fields)
                {
                    var proper = model.FirstOrDefault(f => f.Key.ToLower() == ColumnHelper.GetFieldName(fieldAccessDto.Name)?.ToLower());
                    if (string.IsNullOrWhiteSpace(proper.Key))
                        continue;
                    jObject.Add(proper.Key, new JValue(proper.Value));
                }
                jArray.Add(jObject);
            }

            return jArray.ToString();
        }

        private string GetModelAccess(IDictionary<string, object> model, IEnumerable<FieldAccessDto> fields)
        {
            var jObject = new JObject();
            foreach (var fieldAccessDto in fields)
            {
                var proper = model.FirstOrDefault(f => f.Key.ToLower() == ColumnHelper.GetFieldName(fieldAccessDto.Name)?.ToLower());
                if (string.IsNullOrWhiteSpace(proper.Key))
                    continue;
                if (proper.Value is IEnumerable<IDictionary<string, object>>)
                    jObject.Add(proper.Key, JArray.FromObject(proper.Value));
                else jObject.Add(proper.Key, new JValue(proper.Value));
            }

            return jObject.ToString();
        }
    }
}
