namespace Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных результатов тестирования кандидата
    /// </summary>
    public class ResultTestingCrm : VacancyTestingBaseCrm
    {
        /// <summary>
        /// Ссылка на вопрос для тестирования
        /// </summary>
        public QuestionCrm Question { get; set; }
        /// <summary>
        /// Ссылка на кандидата который произведёл ответ
        /// </summary>
        public Resume User { get; set; }
        /// <summary>
        /// Ответ кандидата
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Время затраченное на ответ
        /// </summary>
        public string Time { get; set; }
    }
}