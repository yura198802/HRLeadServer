using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Service.ReportEngine.Actions
{
    public class ActionAcceptedFromModel : IActionBtnFormModel
    {
        private readonly IManagerClients _managerClients;
        private readonly ReportDbContext _reportDbContext;

        public ActionAcceptedFromModel(IManagerClients managerClients, ReportDbContext reportDbContext)
        {
            _managerClients = managerClients;
            _reportDbContext = reportDbContext;
        }

        public async Task<ResultCrmDb> Action(ActionArgs obj, string userName, int formId, int[] modelIds)
        {
            var user = await _reportDbContext.User.FirstOrDefaultAsync(f => f.Account == userName); 
            await _managerClients.AcceptProduct(user?.Id ?? 0, modelIds);
            return new ResultCrmDb();
        }

        public Task<ResultCrmDb> Action(ModelParametrs.ModelsArgs.ActionArgs obj, string userName, int formId)
        {
            throw new NotImplementedException();
        }
    }
}
