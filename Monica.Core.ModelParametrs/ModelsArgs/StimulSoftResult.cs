namespace Monica.Core.ModelParametrs.ModelsArgs
{
    public class StimulSoftResult
    {
        public bool Success { get; set; }

        public string Notice { get; set; }

        public string[] Columns { get; set; }

        public string[][] Rows { get; set; }

        public string[] Types { get; set; }
    }
}