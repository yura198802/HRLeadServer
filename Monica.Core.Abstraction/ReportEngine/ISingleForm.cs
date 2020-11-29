using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface ISingleForm
    {
        /// <summary>
        /// Получить список полей для редактирования
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<string> GetProppertys(int formModelId, string userName);

        /// <summary>
        /// Получить данные для формы
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<IDictionary<string, object>> GetFormData(int formModelId, string userName);

        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        Task<ResultCrmDb> SaveModel(int formModelId, string userName, IDictionary<string, object> formData);

        /// <summary>
        /// Создать таблицу и заполнить ее данными на основании модели данных
        /// </summary>
        /// <param name="formModelId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task CreateAndFillTableAsync(int formModelId, string userName);
    }
}
