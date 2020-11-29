using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class CategoryCompetence
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Competence> Competences { get; set; }
    }
}
