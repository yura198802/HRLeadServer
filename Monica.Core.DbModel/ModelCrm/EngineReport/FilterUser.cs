using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    public class FilterUser : BaseModel
    {
        public string UserName { get; set; }
        public string ModelFilter { get; set; }
        public FormModel FormModel { get; set; }
        public ButtonForm ButtonForm { get; set; }
    }
}
