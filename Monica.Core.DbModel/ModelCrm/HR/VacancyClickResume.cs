using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class VacancyClickResume
    {
        [Key]
        public int Id { get; set; }

        public Vacancy Vacancy { get; set; }

        public Resume Resume { get; set; }


    }
}
