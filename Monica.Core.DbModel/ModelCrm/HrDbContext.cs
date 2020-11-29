using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.HR;
using Monica.Core.DbModel.ModelCrm.HR.Testing.Candidates;

namespace Monica.Core.DbModel.ModelCrm
{
    /// <summary>
    /// Контекст для работы с данными кинфигуратора режимов
    /// </summary>
    public class HrDbContext : DbContext
    {
        private readonly IDataBaseMain _dataBaseMain;

        public DbSet<CategoryCompetence> CategoryCompetences { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<QuestionOld> QuestionsOld{ get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<VacancyClickResume> VacancyClickResumes { get; set; }
        public DbSet<VacancyMlResume> VacancyMlResumes { get; set; }
        public DbSet<VacancyWorkflow> VacancyWorkflows { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<CategoryVacancy> CategoryVacancies { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<RequestRequirement> RequestRequirements { get; set; }

        public DbSet<TestingCorrectAnswerCrm> TestingCorrectAnswers { get; set; }
        public DbSet<CandidateTestingTypeCrm> CandidateTestingTypes { get; set; }
        public DbSet<ResultTestingCrm> ResultsTesting { get; set; }
        public DbSet<TestingAnswerCrm> TestingAnswers { get; set; }
        public DbSet<QuestionCrm> Questions { get; set; }


        public HrDbContext(IDataBaseMain dataBaseMain)
        {
            _dataBaseMain = dataBaseMain;
        }
        public HrDbContext() 
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dataBaseMain == null)
                optionsBuilder.UseMySql("Server=84.201.188.15;Port=3306;Database=hackatonfinal_gpb;;User Id=develop;Password=$zXcvbnm512$;TreatTinyAsBoolean=true;charset=utf8;");
            else optionsBuilder.UseMySql(_dataBaseMain.ConntectionString);
        }
    }
}
