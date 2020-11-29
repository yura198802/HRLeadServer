using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.Abstraction.ReportEngine
{
    public interface IActionAfterSave
    {
        Task<ResultCrmDb> BeforeSave(int modelId, string userName);
    }
}
