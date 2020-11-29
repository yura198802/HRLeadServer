using Monica.Core.DbModel.ModelCrm.Client.Enums;
using Monica.Core.DbModel.ModelCrm.Profile;
using System;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ManagerAction
    {
        [Key]
        public int Id { get; set; }

        public User User { get; set; }

        public ClientProduct ClientProduct { get; set; }

        public ManagerActionResult Result { get; set; }

        public DateTime? CallAgainDateTime { get; set; }

        public DateTime DateTime { get; set; }
    }
}
