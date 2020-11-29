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
    public class FieldsSetController : BaseController
    {
        private IFieldsAdapter _fields;
        public static string ModuleName => @"FieldsSetController";
        public FieldsSetController(IFieldsAdapter fields) : base(ModuleName)
        {
            _fields = fields;
        }
        /// <summary>
        /// Поучить дерево кнопок форм
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
            return Tools.CreateResult(true, "", await _fields.GetFieldsTreeAsync(idRole));
        }
        /// <summary>
        /// Изменть доступ к кнопокам
        /// </summary>
        /// <param name="idRole"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> EditAccess(int idRole, [FromBody] ItemAccess[] selected)
        {
            return Tools.CreateResult(true, "", await _fields.EditAccessAsync(idRole, selected));
        }
    }
}
