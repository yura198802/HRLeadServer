using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelDto.HR.Testing.Candidates
{
    /// <summary>
    /// Аргументы которые отправляются кандидату для тестирования
    /// </summary>
    public class VacancyTestingDto
    {
        /// <summary>
        /// Ид кандидата
        /// </summary>
        public int CandidateId { get; set; }
        /// <summary>
        /// Вопросы для тестирования
        /// </summary>
        public IEnumerable<VacancyTestingQuestionDto> Questions { get; set; }
    }
}