using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto.Report;

namespace Monica.Core.Abstraction.ReportEngine
{
    /// <summary>
    /// Действие перед удалением
    /// </summary>
    public interface IActionBeforeDelete
    {
        Task<ResultCrmDb> Action(string userName, FormModelDto formModel, string[] ids);
    }
}
