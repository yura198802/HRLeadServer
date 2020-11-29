using Monica.Core.DbModel.ModelCrm.Client.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ProductsOrder
    {
        [Key]
        public int Id { get; set; }

        public Product ProductId { get; set; }

        public Criteria CriteriaId { get; set; }

        public SortingDirection Direction { get; set; }

        public int Index { get; set; }
    }
}
