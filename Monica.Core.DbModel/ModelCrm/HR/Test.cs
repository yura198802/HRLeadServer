using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        public Competence Competence { get; set; }

        public ICollection<QuestionOld> Questionsold { get; set; }
    }
}
