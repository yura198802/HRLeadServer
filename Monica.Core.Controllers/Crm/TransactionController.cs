using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.Profile;
using Monica.Core.Abstraction.Transactions;
using Monica.Core.DbModel.IdentityModel;
using Monica.Core.ModelParametrs.ModelsArgs;
using Newtonsoft.Json;

namespace Monica.Core.Controllers.Crm
{
    /// <summary>
    /// Основной контроллер для авторизации пользователей в системе
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TransactionController : BaseController
    {

        /// <summary>
        /// Наименование модуля
        /// </summary>
        public static string ModuleName => @"Transaction";

        private readonly ITransactionDataAdapter _transaction;
        private readonly IManagerClients _managerClient;

        public TransactionController(ITransactionDataAdapter transaction,IManagerClients managerClient) : base(ModuleName)
        {
            _transaction = transaction;
            _managerClient = managerClient;
        }
        /// <summary>
        /// Получить данные для диаграммы расходов по различным точкам
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCosts(int id)
        {
            return Tools.CreateResult(true, "", await _transaction.GetCosts(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetProductTypesProfit(int id)
        {
            return Tools.CreateResult(true, "", await _managerClient.GetProductTypesProfit(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetClientWorkingTime(int id)
        {
            return Tools.CreateResult(true, "", await _managerClient.GetClientWorkingTime(id));
        }
    }
}
