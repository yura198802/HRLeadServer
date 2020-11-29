using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.HR.Enums;
using Monica.Core.ModelParametrs.ModelsArgs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Monica.Core.Service.Crm.ReportEngine
{
    public class ActionPublishVacancy : IActionBtnFormModel
    {
        private HrDbContext _hrDbContext;

        public ActionPublishVacancy(HrDbContext hrDbContext)
        {
            _hrDbContext = hrDbContext;
        }

        public async Task<ResultCrmDb> Action(ActionArgs obj, string userName, int formId)
        {
            var vacancies = await _hrDbContext.Vacancies
                .Where(f => obj.Ids.Contains(f.Id)).ToListAsync();
            foreach (var vacancy in vacancies)
            {
                vacancy.State = VacancyState.Published;
                vacancy.PublishDate = DateTime.Now;
                _hrDbContext.Update(vacancy);
            }

            await _hrDbContext.SaveChangesAsync();
            return new ResultCrmDb();
        }
    }
}
