namespace Monica.Settings.DataAdapter.Models
{
    public class FieldItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int FieldId { get; set; }
        public int FormId { get; set; }
        public int NodeId { get; set; }
        public bool IsField { get; set; }
        public string Text { get; set; }
    }
}
