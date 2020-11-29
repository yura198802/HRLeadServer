using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ProductParam
    {
        [Key]
        public int Id { get; set; }

        public Product Product { get; set; }

        public ProductParamType ProductParamType { get; set; }

        public string Expression { get; set; }
    }
}
