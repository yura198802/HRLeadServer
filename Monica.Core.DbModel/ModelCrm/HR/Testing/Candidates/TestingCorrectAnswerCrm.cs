namespace Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных правильных ответов
    /// </summary>
    public class TestingCorrectAnswerCrm : VacancyTestingBaseCrm
    {
        /// <summary>
        /// Ссылка на вопрос для тестирования
        /// </summary>
        public QuestionCrm Question { get; set; }
        /// <summary>
        /// Правильный ответ
        /// </summary>
        public string Value { get; set; }
    }
}