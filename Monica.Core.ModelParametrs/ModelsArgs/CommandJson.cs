namespace Monica.Core.ModelParametrs.ModelsArgs
{
    public class CommandJson
    {
        public string Command { get; set; }

        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public string Event { get; set; }

        public bool PreventDefault { get; set; }

        public double Rnd { get; set; }

        public string QueryString { get; set; }
    }
}