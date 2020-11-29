using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.Abstraction.Stimulsoft;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json;

namespace Monica.Core.Controllers.Crm
{

    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DesignerDataController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"DesignerDataController";

        private readonly IFilterPanelPropertysCreater _filterPanelPropertysCreater;
        private readonly IReportData _reportData;
        private readonly IReportManager _reportManager;
        private readonly IStimulsoftEngine _stimulsoftEngine;
        private readonly ISingleForm _singleForm;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DesignerDataController(IReportData reportData, IReportManager reportManager, IStimulsoftEngine stimulsoftEngine, IFilterPanelPropertysCreater filterPanelPropertysCreater, ISingleForm singleForm) : base(ModuleName)
        {
            _reportData = reportData;
            _reportManager = reportManager;
            _stimulsoftEngine = stimulsoftEngine;
            _filterPanelPropertysCreater = filterPanelPropertysCreater;
            _singleForm = singleForm;
        }

        /// <summary>
        /// Получить список фильтров для панели фильтров формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetProperiesFilterPanel(int formId)
        {
            var result = await _filterPanelPropertysCreater.GetProperiesFilterPanel(formId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }


        /// <summary>
        /// Получить список фильтров для панели фильтров формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetProperiesFilterPanelDialog(int formId, int buttonId)
        {
            var result = await _filterPanelPropertysCreater.GetProperiesFilterDialog(formId, GetUserName(), buttonId);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить 
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetLoadDataFilterModel([FromBody] IDictionary<string, object> models, int idFilter)
        {
            var result = await _filterPanelPropertysCreater.GetLoadDataFilterModel(models, idFilter);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для фильтра диалоговой формы
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetLoadDialogDataFilter([FromBody] IDictionary<string, object> models, int idFilter)
        {
            var result = await _filterPanelPropertysCreater.GetLoadDialogDataFilter(models, idFilter);
            return Tools.CreateResult(true, "", result);
        }
        
        /// <summary>
        /// Сохранение значение фильтров
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> SaveFilterDialogValue([FromBody]IDictionary<string,object> models,int formId, int buttonId)
        {
            await _reportManager.AddFilterUser(formId, models,GetUserName(), buttonId);
            return Tools.CreateResult(true, "", "");
        }


        /// <summary>
        /// Получить список форм для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> AfterSaveModel(int idModel, int formId)
        {
            var result = await _reportData.AfterSaveModel(formId, idModel, GetUserName());
            return Tools.CreateResult(true, "", result);
        }
        /// <summary>
        /// Выполнить действие
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> ActionByModel([FromBody] ActionArgs otherInfo, int formId, string sysname)
        {

            var result = await _reportData.Action(formId,  GetUserName(), otherInfo, sysname);
            return Tools.CreateResult(true, "", result);
        }
        /// <summary>
        /// Выполнить действие
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> ActionByDetailModel([FromBody] IDictionary<string,object> formModel, int formId, string sysname)
        {

            var result = await _reportData.ActionByDetailModel(formId, GetUserName(), formModel, sysname);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Выполнить действие формирование файла
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> ActionExportResult([FromBody] ActionArgs otherInfo, int formId, string sysname)
        {
            var result = await _reportData.ActionExportResult(formId, GetUserName(), otherInfo, sysname);
            return Tools.CreateResult(true, "", result);
        }
        

        /// <summary>
        /// Получить список форм для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetDataList([FromBody]BaseModelReportParam p)
        {
            p.UserName = GetUserName();
            var result = await _reportData.GetDataList(p);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список кнопок
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetButtons([FromBody] BaseModelReportParam p)
        {
            var result = await _reportData.GetButtons(p.FormId, GetUserName(),p.FilterModel);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить список кнопок
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        [HttpPost]
        public async Task<IActionResult> GetButtonsDetail([FromBody] BaseModelReportParam p)
        {
            var result = await _reportData.GetButtonsDetail(p.FormId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получение дерева моделей
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetSolutionModel()
        {
            var result = await _reportManager.GetSolutionModel(GetUserName(), false);
            return Tools.CreateResult(true, "", result);
        }
        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetEditModel(int id, int formId)
        {
            var result = await _reportData.GetEditModel(new BaseModelReportParam
                {FormId = formId, ModelId = id.ToString(), UserName = GetUserName()});
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetCopyModel(int id, int formId)
        {
            var result = await _reportData.GetEditModel(new BaseModelReportParam
                { FormId = formId, ModelId = id.ToString(), UserName = GetUserName() });
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="saveModel">Модель данных которую необходимо сохранить</param>
        /// <param name="formId"></param>
        /// <param name="typeEditor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveModel([FromBody] SaveModelArgs saveModel, int formId, int typeEditor)
        {
            var result = await _reportData.SaveModel(saveModel, typeEditor,formId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для формы редактирования
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveEntity([FromBody] string[] key, int formId)
        {
            var result = await _reportData.RemoveEntity(formId, GetUserName(), key);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Проверить данные перед сохранением
        /// </summary>
        /// <param name="saveModel"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateRuleEntity([FromBody] dynamic saveModel, int formId)
        {
            var result = await _reportData.ValidateModel(saveModel,formId);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для детальной информации объекта
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDetailInfoModels(int formId)
        {
            var result = await _reportData.GetDetailInfoModels(formId, GetUserName());
            return Tools.CreateResult(true, "", result);
        }


        /// <summary>
        /// Получить данные для детальной информации объекта
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> StimulsoftData()
        {
            var res = new byte[HttpContext.Request.Body.Length];
            HttpContext.Request.Body.Read(res, 0, (int)HttpContext.Request.Body.Length);
            var commandJson = JsonConvert.DeserializeObject<CommandJson>(System.Text.Encoding.Default.GetString(res));
            var result = await _stimulsoftEngine.ActionResultData(commandJson,GetUserName());
            return Tools.CreateResult(true, "", result);
        }
        
        /// <summary>
        /// Получить файл отчета для стимулсофт
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> GetStimulsoftReport(int id)
        {
            var result = await _stimulsoftEngine.GetFileMrt(id);
            return Tools.CreateResult(true, "", result);
        }
        /// <summary>
        /// Действие, иморт файла
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportFile([FromForm] ActionArgs otherInfo, int formId, string sysname)
        {
            var result = new ResultCrmDb();
            if ((Request.Form?.Files?.Count ?? 0) == 0)
            {
                result.AddError("1002", "Файлы для импорта не переданы!");
                return Tools.CreateResult(true, "Файлы не передан!", result);
            }
            List<IFormFile> files = new List<IFormFile>();
            foreach (var formfile in Request.Form.Files)
                files.Add(formfile);
            result = await _reportData.ActionImportFile(formId, GetUserName(), otherInfo, sysname, files);
            return Tools.CreateResult(true, "", result);
        }

        /// <summary>
        /// Получить данные для одиночной формы редактирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetFormDataSingle(int formId)
        {
            var data = await _singleForm.GetFormData(formId, GetUserName());
            return Tools.CreateResult(true, "", data);
        }
        /// <summary>
        /// Получить поля для одиночной формы редактирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetProppertysSingleForm(int formId)
        {
            var data = await _singleForm.GetProppertys(formId, GetUserName());
            return Tools.CreateResult(true, "", data);
        }
        /// <summary>
        /// Сохранение данных на одиночной форме
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveModelSingleForm([FromBody] IDictionary<string,object> saveModel, int formId)
        {
            var data = await _singleForm.SaveModel(formId, GetUserName(), saveModel);
            return Tools.CreateResult(true, "", data);
        }
    }
}
