using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Monica.Settings.DataAdapter.Interfaces;
using Monica.Settings.DataAdapter.Models;

namespace Frgo.Dohod.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ButtonsSetController : BaseController
    {
        IBtnsAdapter _btns;
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"ButtonsSetController";
        public ButtonsSetController(IBtnsAdapter btns) : base(ModuleName)
        {
            this._btns = btns;
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
        public async Task<IActionResult> GetTreeAsync(int idRole)
        {
            return Tools.CreateResult(true, "", await _btns.GetButtonsTreeAsync(idRole));
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
        public async Task<IActionResult> EditAccessAsync(int idRole,[FromBody] ItemAccess[] items)
        {
            return Tools.CreateResult(true, "", await _btns.EditAccessAsync(idRole, items));
        }
    }
}