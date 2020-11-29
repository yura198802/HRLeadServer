using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates
{
    /// <summary>
    /// Модель для получения данных типа элемента тестирования из БД
    /// </summary>
    public class CandidateTestingTypeCrm : VacancyTestingBaseCrm
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Type { get; set; }
        public ICollection<QuestionCrm> Quetions { get; set; }
    }
}