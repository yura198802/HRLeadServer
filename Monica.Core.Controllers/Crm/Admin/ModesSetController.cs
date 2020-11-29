using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.ModelCrm.Settings;

namespace Monica.Core.Controllers.Crm.Admin
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ModesSetController : BaseController
    {
        IModesAdapter _modes;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"ModesSetController";
        public ModesSetController(IModesAdapter modes) : base(ModuleName)
        {
            this._modes = modes;
        }
        /// <summary>
        /// Поучить дерево форм по типам форм
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTree(int idRole)
        {
            return Tools.CreateResult(true, "", await _modes.GetModesTreeAsync(idRole));
        }
        /// <summary>
        /// Изменть доступ к формам
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> EditAccess(int idRole, ItemAccess[] items)
        {
            return Tools.CreateResult(true, "", await _modes.EditAccessAsync(idRole, items));
        }
    }
}
