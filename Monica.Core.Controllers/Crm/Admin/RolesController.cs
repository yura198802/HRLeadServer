using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.ModelDto.Roles;

namespace Monica.Core.Controllers.Crm.Admin
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RolesController : BaseController
    {
        IRolesAdapter _roles;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"RolesController";
        public RolesController(IRolesAdapter roles) : base(ModuleName)
        {
            this._roles = roles;
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddRole(RoleCreateArgs args)
        {
            return Tools.CreateResult(true, "", await _roles.AddRoleForLevelOrgAsync(args));
        }
        /// <summary>
        /// Получить список всех ролей организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetRolesByOrg(int idOrg)
        {
            return Tools.CreateResult(true, "", await _roles.GetRolesByLevelOrgAsync(idOrg));
        }
        /// <summary>
        /// удаление роли для организации
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Remove(int idRole)
        {
            return Tools.CreateResult(true, "", await _roles.RemoveUserRoleAsync(idRole));
        }
        /// <summary>
        /// Изменить роль
        /// </summary>
        /// <param name="idRole">Системный номер записи пользователя</param>
        /// <param name="newName">Новое название роли</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Edit(int idRole, string newName)
        {
            return Tools.CreateResult(true, "", await _roles.EditUserRoleAsync(idRole,newName));
        }

    }
}
