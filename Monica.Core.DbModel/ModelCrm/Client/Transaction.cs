using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class Transactions
    {
        [Key]
        public int Id { get; set; }
        public int client_id { get; set; }
        public DateTime TRANSACTION_DT { get; set; }
        public string MCC_KIND_CD { get; set; }
        public int MCC_CD { get; set; }
        public decimal CARD_AMOUNT_EQV_CBR { get; set; }
    }
}
