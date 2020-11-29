namespace Monica.Settings.DataAdapter.Models
{
    public class ButtonItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ButtonId { get; set; }
        public int FormId { get; set; }
        public bool IsButton { get; set; }
        public string Text { get; set; }
    }
}
