using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IActionBtnFormModel
    {
        Task<ResultCrmDb> Action(ActionArgs obj, string userName, int formId);
    }
    public interface IActionBtnDetailFormModel
    {
        Task<IDictionary<string,object>> Action(IDictionary<string, object> obj, string userName, int formId);
    }
}
