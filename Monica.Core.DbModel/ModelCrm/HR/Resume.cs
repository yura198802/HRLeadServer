using System;
using System.ComponentModel.DataAnnotations;

namespace Monica.Core.DbModel.ModelCrm.HR
{
    public class Resume
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public string Education { get; set; }
        public string Skills { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Text { get; set; }

        public int Experience { get; set; }
    }
}
