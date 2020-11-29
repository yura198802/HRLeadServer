using System.Collections;
using System.Collections.Generic;
using Monica.Core.DbModel.ModelDto.HR.Testing.Candidates;

namespace Monica.Core.ModelParametrs.ModelsArgs
{
    /// <summary>
    /// Аргументы получаемы от тестируемого кандидата
    /// </summary>
    public class VacancyTestingInArgs
    {
        /// <summary>
        /// Ид кандидата
        /// </summary>
        public int CandidateId { get; set; }
        /// <summary>
        /// Ответы кандидатов
        /// </summary>
        public IEnumerable<ResultTestingDto> Results { get; set; }
    }
}