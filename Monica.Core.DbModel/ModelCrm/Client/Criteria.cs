using Monica.Core.DbModel.ModelCrm.Client.Enums;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class Criteria
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public TypeValue TypeValue { get; set; }
    }
}
