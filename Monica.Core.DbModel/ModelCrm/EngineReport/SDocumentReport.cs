using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    public class DocumentReport
    {
        [Key]
        public int Sysid { get; set; }
        public string NameReport { get; set; }
        public DocumentReport() { }
    }
}