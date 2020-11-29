using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Settings.DataAdapter.Models;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Interfaces
{
    public interface IFieldsAdapter
    {
        Task<IEnumerable<ItemAccess>> GetFieldsTreeAsync(int idRole);
        Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] selected);
    }
}
