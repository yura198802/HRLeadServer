using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.Crm.Settings
{
    public interface IUsersAdapter
    {
        /// <summary>
        /// Получить пользователей привязанных к роли
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersByRoleAsync(int idRole);
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        Task<IEnumerable<User>> GetUsersAsync();
        /// <summary>
        /// Получить свободных пользователей
        /// </summary>
        Task<IEnumerable<User>> GetFreeAsync();
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RegisterUserAsync(RegistrationUserArgs args);
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="accUsers"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveUsersAsync(IEnumerable<string> accUsers);
        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<ResultCrmDb> EditUserAsync(RegistrationUserArgs args);
        /// <summary>
        /// Удаление связи  между ролью и пользователем
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idRole"></param>
        /// <returns></returns>
        Task<ResultCrmDb> RemoveUserFromRoleAsync(int idUser, int idRole);
        /// <summary>
        /// Добавление связи  между ролью и пользователем
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="idsUsers"></param>
        /// <returns></returns>
        Task<ResultCrmDb> AddUserToRoleAsync(int idRole, IEnumerable<int> idsUsers);
    }
}
