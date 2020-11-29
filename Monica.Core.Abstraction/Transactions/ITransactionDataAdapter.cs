using System.Collections.Generic;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Paging;

namespace Monica.Core.Abstraction.Transactions
{
    /// <summary>
    /// Менеджер для работы с профилями пользователей
    /// </summary>
    public interface ITransactionDataAdapter
    {
        /// <summary>
        /// Получить транзации клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Cost>> GetCosts(int clientId);
    }

    public class Cost
    {
        public string Arg { get; set; }

        public double Val { get; set; }
    }
}
