using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class RequestRequirement
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public Request Request { get; set; }
    }
}
