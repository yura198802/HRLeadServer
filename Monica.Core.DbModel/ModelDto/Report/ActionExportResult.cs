namespace Monica.Core.DbModel.ModelDto.Report
{
    public class ActionExportResult
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string TypeFile { get; set; }
    }
}
