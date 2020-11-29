using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public ICollection<RequestRequirement> RequestRequirements { get; set; }
    }
}
