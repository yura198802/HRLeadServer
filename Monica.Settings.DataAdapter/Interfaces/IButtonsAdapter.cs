using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Settings.DataAdapter.Models;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Interfaces
{
    public interface IBtnsAdapter
    {
        /// <summary>
        /// Получить дерево элементов для выбора доступных кнопок по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<IEnumerable<ItemAccess>> GetButtonsTreeAsync(int idRole);
        /// <summary>
        /// Настройка доступа к кнопкам по роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] items);
    }
}
