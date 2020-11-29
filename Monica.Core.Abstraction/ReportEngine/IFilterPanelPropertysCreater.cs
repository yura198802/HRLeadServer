using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IFilterPanelPropertysCreater
    {
        /// <summary>
        /// Получить список фильтров для формы
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<JArray> GetProperiesFilterPanel(int formId, string userName);

        /// <summary>
        /// Получить список фильтров для диалоговой формы
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userName"></param>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        Task<JArray> GetProperiesFilterDialog(int formId, string userName, int buttonId);

        /// <summary>
        /// Получить данные для панели фильтров в основном режиме
        /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        Task<JArray> GetLoadDataFilterModel(IDictionary<string, object> filterModel, int idFilter);

        /// <summary>
        /// Получить данные для формы фильтра, которая вызывается через диалоговую форму фильтров
        /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        Task<JArray> GetLoadDialogDataFilter(IDictionary<string, object> filterModel, int idFilter);
    }
}
