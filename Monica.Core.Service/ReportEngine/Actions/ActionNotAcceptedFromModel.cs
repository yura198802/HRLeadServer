using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Service.ReportEngine.Actions
{
    public class ActionNotAcceptedFromModel : IActionBtnFormModel
    {
        private readonly IManagerClients _managerClients;
        private readonly ClientDbContext _clientDbContext;

        public ActionNotAcceptedFromModel(IManagerClients managerClients, ClientDbContext clientDbContext)
        {
            _managerClients = managerClients;
            _clientDbContext = clientDbContext;
        }


        public async Task<ResultCrmDb> Action(ActionArgs obj, string userName, int formId)
        {
            var user = await _clientDbContext.User.FirstOrDefaultAsync(f => f.Account == userName);
            var clientProduct = await _clientDbContext.ClientProduct.Include(c => c.Client).
                FirstOrDefaultAsync(f => obj.Ids.Contains(f.Id));
            await _managerClients.RefuseProduct(user?.Id ?? 0, clientProduct.Client?.Id ?? 0);
            return new ResultCrmDb();
        }
    }
}
