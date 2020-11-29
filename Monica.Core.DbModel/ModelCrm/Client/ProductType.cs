using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
