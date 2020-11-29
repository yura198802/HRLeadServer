using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Controllers.Crm.Admin
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        IUsersAdapter _users;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"UsersController";
        public UsersController(IUsersAdapter users) : base(ModuleName)
        {
            this._users = users;
        }

        /// <summary>
        /// Получить всех пользователей связанных с ролью
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsersByRole(int idRole)
        {
            return Tools.CreateResult(true, "", await _users.GetUsersByRoleAsync(idRole));
        }
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers()
        {
            return Tools.CreateResult(true, "", await _users.GetUsersAsync());
        }
        /// <summary>
        /// Получить пользователей не привязанных к пользователям
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetFree()
        {
            return Tools.CreateResult(true, "", await _users.GetFreeAsync());
        }
        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RegisterUser(RegistrationUserArgs args)
        {
            return Tools.CreateResult(true, "", await _users.RegisterUserAsync(args));
        }
        /// <summary>
        /// удаление пользователя 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveUserss(IEnumerable<string> accounts)
        {
            return Tools.CreateResult(true, "", await _users.RemoveUsersAsync(accounts));
        }
        /// <summary>
        /// Редактирование нового пользователя
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> EditUserr(RegistrationUserArgs args)
        {
            return Tools.CreateResult(true, "", await _users.EditUserAsync(args));
        }
        /// <summary>
        /// Добавить пользователя к роли
        /// </summary>
        /// <param name="idUser">Системный номер записи пользователя</param>
        /// <param name="idRole">Системный номер записи роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddUserToRole(int idRole,[FromBody] IEnumerable<int> idsUsers)
        {
            return Tools.CreateResult(true, "", await _users.AddUserToRoleAsync(idRole, idsUsers));
        }
        /// <summary>
        /// Удаление связи  между ролью и пользователем
        /// </summary>
        /// <param name="idUser">Системный номер записи пользователя</param>
        /// <param name="idRole">Системный номер записи роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))] 
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> RemoveUserFromRole(int idUser, int idRole)
        {
            return Tools.CreateResult(true, "", await _users.RemoveUserFromRoleAsync(idUser, idRole));
        }
    }
}
