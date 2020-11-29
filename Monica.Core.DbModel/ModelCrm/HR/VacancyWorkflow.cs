using Monica.Core.DbModel.ModelCrm.HR.Enums;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class VacancyWorkflow
    {
        [Key]
        public int Id { get; set; }
        public Vacancy Vacancy { get; set; }

        public Resume Resume { get; set; }

        public ResumeState State { get; set; }
    }
}
