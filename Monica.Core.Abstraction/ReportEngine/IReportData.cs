using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Получение данных для запрашиваемых режимов
    /// </summary>
    public interface IReportData
    {
        /// <summary>
        /// Действие после сохранения объекта
        /// </summary>
        /// <param name="idModel"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ResultCrmDb> AfterSaveModel(int formId, int idModel, string userName);
        /// <summary>
        /// Событие нажатия кнопки на форме
        /// </summary>
        /// <param name="formId">Модель, для которой вызывается событие</param>
        /// <param name="userName">Пользователь сделавший это событие</param>
        /// <param name="otherInfo">Параметны события</param>
        /// <param name="sysname">Системное имя кнопки</param>
        /// <returns></returns>
        Task<ResultCrmDb> Action(int formId, string userName, ActionArgs otherInfo, string sysname);
        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        Task<ReportResultData> GetDataList(BaseModelReportParam parametr);

        /// <summary>
        /// Получить данные для списка
        /// </summary>
        /// <param name="parametr"></param>
        /// <returns></returns>
        Task<ReportResultData> GetEditModel(BaseModelReportParam parametr);

        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        Task<JArray> GetButtons(int formId, string userName, IDictionary<string, object> filterData);

        /// <summary>
        /// Получить список кнопок для режима
        /// </summary>
        /// <param name="formId">Id режима</param>
        /// <param name="userName">Имя пользователя</param>
        /// <returns></returns>
        Task<JArray> GetButtonsDetail(int formId, string userName);

        /// <summary>
        /// Метод сохранения данных
        /// </summary>
        /// <param name="saveModel">данные модели</param>
        /// <param name="typeEditor">Тип редактирования: 1 - Добавление, 2 - Редактирование, 3 - удалаение</param>
        /// <param name="formModel"></param>
        /// <param name="userName">Имя пользователя, который делает добавление записи</param>
        /// <returns></returns>
        Task<ResultCrmDb> SaveModel(SaveModelArgs saveModel, int typeEditor, int formModel, string userName);

        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveEntity(int formModel, string userName, string[] key);

        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        Task<ResultCrmDb> ValidateModel(dynamic saveModel, int formModelId);

        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SolutionModel>> GetDetailInfoModels(int formModel, string userName);
        /// <summary>
        /// Событие нажажатия на кнопу импорт файла
        /// </summary>
        /// <param name="formId">Модель, для которой вызывается событие</param>
        /// <param name="userName">Пользователь сделавший это событие</param>
        /// <param name="otherInfo">Параметны события</param>
        /// <param name="sysname">Системное имя кнопки</param>
        /// <param name="files">Файлы для импорта</param>
        /// <returns></returns>
        Task<ResultCrmDb> ActionImportFile(int formId, string userName, ActionArgs otherInfo, string sysname, List<IFormFile> files);

        /// <summary>
        /// Выполнить действие формирования файла
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userName"></param>
        /// <param name="otherInfo"></param>
        /// <param name="sysname"></param>
        /// <returns></returns>
        Task<List<ActionExportResult>> ActionExportResult(int formId, string userName, ActionArgs otherInfo,
            string sysname);

        Task<IDictionary<string, object>> ActionByDetailModel(int formId, string userName,
            IDictionary<string, object> formModel, string sysname);
    }
}
