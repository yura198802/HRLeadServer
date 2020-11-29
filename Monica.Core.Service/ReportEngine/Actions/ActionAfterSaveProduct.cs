using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.Service.ReportEngine.Actions
{
    public class ActionAfterSaveProduct : IActionAfterSave
    {
        private IManagerClients _managerClients;


        public ActionAfterSaveProduct(IManagerClients managerClients, ClientDbContext clientDbContext)
        {
            _managerClients = managerClients;
        }

        public async Task<ResultCrmDb> BeforeSave(int modelId, string userName)
        {
            await _managerClients.AddProductPostProcess(modelId);
            return new ResultCrmDb();
        }
    }
}
