using System.ComponentModel.DataAnnotations;
using Monica.Core.DbModel.Extension;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    public class DialogButtonForm : IFilterForm
    {
        [Key]
        public int Id { get; set; }
        public string Sysname { get; set; }
        public string SqlData { get; set; }
        public string Caption { get; set; }
        public int? Order { get; set; }
        public int? ParentId { get; set; }
        public ButtonForm ButtonForm { get; set; }
        public TypeFilter TypeFilter { get; set; }
        public FormModel FormModel { get; set; }
    }
}
