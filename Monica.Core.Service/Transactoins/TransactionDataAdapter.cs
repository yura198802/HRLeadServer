using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Transactions;
using Monica.Core.DbModel.ModelCrm;

namespace Monica.Core.Service.Transactoins
{
    public class TransactionDataAdapter : ITransactionDataAdapter
    {
        private ClientDbContext _clientDbContext;

        public TransactionDataAdapter(ClientDbContext clientDbContext)
        {
            _clientDbContext = clientDbContext;
        }
        public async Task<List<Cost>> GetCosts(int clientId)
        {
            var result = new List<Cost>();

            var transactions = await _clientDbContext.Transactions.Where(t => t.client_id == clientId).ToArrayAsync();
            foreach(var tr in transactions)
            {
                var cost = result.FirstOrDefault(c => c.Arg == tr.MCC_KIND_CD);
                if (cost != null)
                {
                    cost.Val += (double)tr.CARD_AMOUNT_EQV_CBR;
                }
                else
                {
                    cost = new Cost
                    {
                        Arg = tr.MCC_KIND_CD,
                        Val = (double)tr.CARD_AMOUNT_EQV_CBR
                    };
                    result.Add(cost);
                }
            }

            return result;
        }
    }
}
