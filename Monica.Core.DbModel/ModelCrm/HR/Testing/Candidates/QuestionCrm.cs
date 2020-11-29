using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates
{
    /// <summary>
    /// Модель данных вопроса для тестирования пользователя в БД
    /// </summary>
    public class QuestionCrm : VacancyTestingBaseCrm
    {
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
        public CandidateTestingTypeCrm Type { get; set; }
        /// <summary>
        /// Теги скилов вакансии, через запятую
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// Варианты ответов для вопросов
        /// </summary>
        public IEnumerable<TestingAnswerCrm> Answers { get; set; }
        /// <summary>
        /// Корректный ответ
        /// </summary>
        public ICollection<TestingCorrectAnswerCrm> CorrectAnswer { get; set; }
    }
}