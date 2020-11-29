using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto.HR.Testing.Candidates;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Abstraction.Testing.Vacancy
{
    public interface IVacancyTestingAdapter
    {
        /// <summary>
        /// Получить вопросы для тестирования кандидата
        /// </summary>
        /// <returns></returns>
        Task<VacancyTestingDto> GetQuestionsAsync(int candidateId);
        /// <summary>
        /// Сохранить результаты тестирования
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<ResultCrmDb> SaveResultsAsync(VacancyTestingInArgs args);
    }
}