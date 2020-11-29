using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.Abstraction.Testing.Vacancy;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Service.Crm;
using Monica.Core.Service.ReportEngine;
using Monica.Core.Service.Testing.Candidates;
using Monica.Core.Utils;

namespace Monica.Core.Controllers.Crm.HR
{
    /// <summary>
    /// Основной контроллер для работы с кандидатами
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class HrController : BaseController
    {
        /// <summary>
        /// Наименование модуля
        /// </summary>
        
        public static string ModuleName => @"HrController";

        private readonly IVacancyTestingAdapter _vacancyTesting;
        private IHrService _hrService;

        public HrController(IVacancyTestingAdapter vacancyTesting, IHrService hrService) : base(ModuleName)
        {
            _hrService = hrService;
            _vacancyTesting = vacancyTesting;
        }

        /// <summary>
        /// Получть список заданий для тестирования кандидата исходя из его навыков
        /// </summary>
        /// <param name="userId">Ид пользователя</param>
        /// <returns>Ид кандидата,задания для тестирования</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestions(int userId)
        {
            return Tools.CreateResult(true, "", await _vacancyTesting.GetQuestionsAsync(userId));
        }
        /// <summary>
        /// Сохранить результаты тестирования кандидата
        /// </summary>
        /// <param name="args">Ид пользователя, список результатов</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveResults(VacancyTestingInArgs args)
        {
            return Tools.CreateResult(true, "", await _vacancyTesting.SaveResultsAsync(args));
        }

        
        /// <summary>
        /// Сохранить результаты тестирования кандидата
        /// </summary>
        /// <param name="args">Ид пользователя, список результатов</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetVacansiesDiagram(int id)
        {
            return Tools.CreateResult(true, "", await _hrService.GetVacansiesDiagram(id));
        }
    }
}