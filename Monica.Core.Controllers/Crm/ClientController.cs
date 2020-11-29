using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Monica.Core.Controllers.Crm
{
    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class ClientController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"ClientController";
        protected ClientController(string moduleName) : base(ModuleName)
        {
        }


        /// <summary>
        /// Получить список форм для редактирования
        /// </summary>
        /// <returns>Список элементов для формы</returns>
        //[HttpPost]
        //public async Task<IActionResult> GetDataList([FromBody] BaseModelReportParam p)
        //{
        //    p.UserName = GetUserName();
        //    var result = await _reportData.GetDataList(p);
        //    return Tools.CreateResult(true, "", result);
        //}
    }
}
