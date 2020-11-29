using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Competence
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Test> Tests { get; set; }

        public CategoryCompetence Category { get; set; }
    }
}
