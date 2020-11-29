using Monica.Core.DbModel.Extension;

namespace Monica.Core.DbModel.ModelCrm.Settings
{
    public class ItemAccess
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int FieldId { get; set; } = -1;
        public int FormId { get; set; } = -1;
        public int TypeId { get; set; } = -1;
        public int BtnId { get; set; } = -1;
        public bool IsBtn { get; set; } = false;
        public bool IsField { get; set; } = false;
        public bool IsForm { get; set; } = false;
        public bool IsType { get; set; } = false;
        public TypeAccec typeAccess { get; set; }
        public string Text { get; set; }
    }
}
