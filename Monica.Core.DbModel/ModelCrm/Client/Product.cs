using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ProductType ProductType { get; set; }

        public int OrderIndex { get; set; }

        public string Expression { get; set; }
    }
}
