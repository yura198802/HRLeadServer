using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Расширение для загрузки данных фильтра
    /// </summary>
    public interface ILoadDataFilter
    {
        /// <summary>
        /// Получить данные для фильтра
        /// </summary>
        /// <param name="dataFilter">Модель фильтра, на случай если будет созависимый фильтр</param>
        /// <returns></returns>
        Task<JArray> ActionLoadDataFilter(IDictionary<string, object> dataFilter);
    }
}
