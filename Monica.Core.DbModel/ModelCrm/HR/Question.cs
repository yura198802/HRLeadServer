using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class QuestionOld
    {
        [Key]
        public int Id { get; set; }

        public Test Test { get; set; }
    }
}
