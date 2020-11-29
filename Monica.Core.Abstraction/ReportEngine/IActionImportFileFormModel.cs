using Microsoft.AspNetCore.Http;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IActionImportFileFormModel
    {
        Task<ResultCrmDb> ActionImport(ActionArgs obj, string userName, int formId, List<IFormFile> files);
    }
}
