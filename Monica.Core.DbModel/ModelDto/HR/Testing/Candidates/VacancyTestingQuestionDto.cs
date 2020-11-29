using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelDto.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных вопроса, который отправляется для тестирования пользователя
    /// </summary>
    public class VacancyTestingQuestionDto
    {
        /// <summary>
        /// Ид вопроса
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// Описание к вопросу
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Тип элемента вопроса
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Теги скилов вакансии
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// Варианты ответов для вопросов
        /// </summary>
        public IEnumerable<VacancyTestingAnswerDto> Answers { get; set; }
    }
}