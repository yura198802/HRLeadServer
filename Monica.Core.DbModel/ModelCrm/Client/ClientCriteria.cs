using Monica.Core.DbModel.ModelCrm.Client.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monica.Core.DbModel.ModelCrm.Client
{
    public class ClientCriteria
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("Clientclient_id")]
        public Client Client { get; set; }

        //[ForeignKey("CriteriaId")]
        public Criteria Criteria { get; set; }

        public bool BoolValue { get; set; }
        public long? LongValue { get; set; }
        public double? DoubleValue { get; set; }
        public string StringValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        public object GetValue()
        {
            switch(Criteria.TypeValue)
            {
                case TypeValue.Bool:
                    return BoolValue;
                case TypeValue.Long:
                    return LongValue;
                case TypeValue.Double:
                    return DoubleValue;
                case TypeValue.String:
                    return StringValue;
                case TypeValue.DateTime:
                    return DateTimeValue;
                default: throw new Exception("Тип входного значения данных не существует в данном контексте");
            }
        }
    }
}
