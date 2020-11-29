namespace Monica.Core.DbModel.ModelDto.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных результатов тестирования кандидата
    /// </summary>
    public class ResultTestingDto
    {
        /// <summary>
        /// Ид вопроса на который бл произведён ответ
        /// </summary>
        public int QuestionId { get; set; }
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