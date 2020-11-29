using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Settings.DataAdapter.Models.Crm.Core;
using Monica.Settings.DataAdapter.Models.Dto;

namespace Monica.Settings.DataAdapter.Interfaces
{
    public interface ILevelOrgAdapter
    {
        /// Метод удаления организации из t_levelorg
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveAsync(int idOrg);
        /// <summary>
        /// Метод для добавления записи в t_levelorg
        /// </summary>
        /// <param name="levelOrg"></param>
        /// <returns></returns>
        Task<ResultCrmDb> AddAsync(LevelOrgAddArgs args);
        /// <summary>
        /// Метод получения всех объектов из t_levelorg
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LevelOrgDto>> GetAll();

        ///// <summary>
        ///// Метод получения списка пользователей для выбранной роли
        ///// </summary>
        ///// <param name="sysIdRole"></param>
        ///// <returns></returns>
        //Task<IEnumerable<UserDto>> GetUsersByUserRole(int sysIdRole);

        Task<ResultCrmDb> EditLevelOrgAsync(LevelOrgDto levelOrg);

    }
}
