using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Monica.Core.DataBaseUtils;
using Monica.Core.Utils;

namespace Monica.PlatformMain.LoaderModules
{
    public class DataBaseMain : IDataBaseMain
    {

        public DataBaseMain(IConfiguration configuration)
        {
            var configuration1 = configuration;
            var database =  configuration1["DataBase:NameDefault"] ;
            ConntectionString = string.Format(configuration1["ConnectionString:DefaultConnection"], database);
            
            switch (configuration1["DataBase:Used"])
            {
                case "MySql": TypeDataBase = DataBaseName.MySql;
                    break;
                case "MsSql":
                    TypeDataBase = DataBaseName.MsSql;
                    break;
                case "PostgreSql":
                    TypeDataBase = DataBaseName.PostgreSql;
                    break;

            }
        }

        public string ConntectionString { get; set; }
        public DataBaseName TypeDataBase { get; set; }
    }

    public class DataBaseIs4 : IDataBaseIs4
    {
        public DataBaseIs4(IConfiguration configuration)
        {
            var configuration1 = configuration;
            ConntectionString = configuration1["ConnectionString:IdentityServer4"];
            switch (configuration1["DataBase:Used"])
            {
                case "MySql":
                    TypeDataBase = DataBaseName.MySql;
                    break;
                case "MsSql":
                    TypeDataBase = DataBaseName.MsSql;
                    break;
                case "PostgreSql":
                    TypeDataBase = DataBaseName.PostgreSql;
                    break;

            }
        }

        public string ConntectionString { get; set; }
        public DataBaseName TypeDataBase { get; set; }
    }

}
