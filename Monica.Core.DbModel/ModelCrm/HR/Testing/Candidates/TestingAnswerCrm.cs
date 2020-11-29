namespace Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных вариантов ответа
    /// </summary>
    public class TestingAnswerCrm : VacancyTestingBaseCrm
    {
        /// <summary>
        /// Ссылка на вопрос для тестирования
        /// </summary>
        public QuestionCrm Question { get; set; }
        /// <summary>
        /// Номер варианта
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// Значение варианта
        /// </summary>
        public string Value { get; set; }
    }
}