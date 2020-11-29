using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public Vacancy Vacancy { get; set; }
    }
}
