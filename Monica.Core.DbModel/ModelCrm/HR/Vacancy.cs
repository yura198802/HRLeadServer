using System;
using Monica.Core.DbModel.ModelCrm.HR.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Vacancy
    {
        public Vacancy()
        {
            AppDate = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double SalaryMin { get; set; }

        public double SalaryMax { get; set; }

        public SkillLevel SkillCandidat { get; set; }

        public CategoryVacancy CategoryVacancy { get; set; }

        public ICollection<Competence> Competences { get; set; }

        public ICollection<Work> Works { get; set; }

        public ICollection<Requirement> Requirements { get; set; }

        public ICollection<Offer> Offers { get; set; }

        public VacancyState State { get; set; }
        public DateTime AppDate { get; set; }
        public DateTime? PublishDate { get; set; }
    }
}
