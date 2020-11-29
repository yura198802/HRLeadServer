using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class CategoryVacancy
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Vacancy> Vacancies { get; set; }
    }
}
