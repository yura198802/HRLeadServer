using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ClientProduct
    {
        [Key]
        public int Id { get; set; }

        public Client Client { get; set; }

        public Product Product { get; set; }

        public bool Accepted { get; set; }

        public int Index { get; set; }
    }
}
