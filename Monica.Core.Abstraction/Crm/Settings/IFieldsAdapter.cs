using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Settings;

namespace Monica.Core.Abstraction.Crm.Settings
{
    public interface IFieldsAdapter
    {
        Task<IEnumerable<ItemAccess>> GetFieldsTreeAsync(int idRole);
        Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] selected);
    }
}
