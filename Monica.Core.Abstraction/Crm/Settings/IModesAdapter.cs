using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Settings;

namespace Monica.Core.Abstraction.Crm.Settings
{
    public interface IModesAdapter
    {
        /// <summary>
        /// Получить дерево элементов для выбора доступных форм по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<IEnumerable<ItemAccess>> GetModesTreeAsync(int idRole);
        /// <summary>
        /// Настройка доступа к режимам по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] items);
    }
}
