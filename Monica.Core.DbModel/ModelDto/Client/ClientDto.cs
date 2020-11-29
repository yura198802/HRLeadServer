using System;
using System.Collections.Generic;
using System.Text;

namespace Monica.Core.DbModel.ModelDto.Client
{
    public class ClientDto
    {
        public int client_id { get; set; }
        public int age { get; set; }
        public string gender_code { get; set; }
        public string directory { get; set; }
        public decimal aMRG_eop { get; set; }
        public decimal aCSH_eop { get; set; }
        public decimal aCRD_eop { get; set; }
        public decimal pCUR_eop { get; set; }
        public decimal pCRD_eop { get; set; }
        public decimal pSAV_eop { get; set; }
        public decimal pDEP_eop  { get; set; }
        public decimal sWork_S { get; set; }
        public decimal tPOS_S { get; set; }
    }
}
