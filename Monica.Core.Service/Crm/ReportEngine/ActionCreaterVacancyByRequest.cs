using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Service.Crm.ReportEngine
{
    public class ActionCreaterVacancyByRequest : IActionBtnFormModel
    {
        private HrDbContext _hrDbContext;
        private IHrService _hrService;


        public ActionCreaterVacancyByRequest(HrDbContext hrDbContext, IHrService hrService)
        {
            _hrDbContext = hrDbContext;
            _hrService = hrService;
        }

        public async Task<ResultCrmDb> Action(ActionArgs obj, string userName, int formId)
        {
            var requests = await _hrDbContext.Requests
                .Include(i => i.RequestRequirements)
                .Where(f => obj.Ids.Contains(f.Id)).ToListAsync();
            foreach (var request in requests)
            {
                var vacancy = await _hrService.GetAutomaticVacancy(request);
            }
            return new ResultCrmDb();
        }
    }
}
