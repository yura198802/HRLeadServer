using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Settings.DataAdapter.Models;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Interfaces
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
