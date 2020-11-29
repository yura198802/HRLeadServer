using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Действие перед сохранением объекта (можно только модернизировать сохраняемый объект)
    /// </summary>
    public interface IActionBeforeSave
    {
        /// <summary>
        /// Действие перед сохранением
        /// </summary>
        /// <param name="data">Объект данных</param>
        /// <returns>результат обработки</returns>
        Task<IDictionary<string, object>> ActionBefore(IDictionary<string, object> data);
    }
}
