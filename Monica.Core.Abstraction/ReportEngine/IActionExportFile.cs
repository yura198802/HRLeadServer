using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IActionExportFile
    {
        /// <summary>
        /// Действие для экспорта файла
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="userName"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        Task<List<ActionExportResult>> ActionExportFile(ActionArgs obj, string userName, int formId);
    }
}
