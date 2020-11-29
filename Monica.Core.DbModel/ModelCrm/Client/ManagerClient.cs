using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Profile;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ManagerClient
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("UserId")]
        public User User { get; set; } 
        
        //[ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}
