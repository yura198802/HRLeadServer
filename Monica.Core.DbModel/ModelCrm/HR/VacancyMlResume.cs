using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class VacancyMlResume
    {
        [Key]
        public int Id { get; set; }
        public Vacancy Vacancy { get; set; }

        public Resume Resume { get; set; }

        public double Rate { get; set; }
    }
}
