using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Monica.Core.Abstraction.Stimulsoft;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.ModelParametrs.ModelsArgs;

namespace Monica.Core.Service.Stimulsoft
{
    public class StimulsoftEngineAdapter : IStimulsoftEngine
    {
        private IDataBaseMain _dataBaseMain;
        private IConfiguration _configuration;
        private ReportDbContext _reportDbContext;

        public StimulsoftEngineAdapter(IDataBaseMain iDataBaseMain, IConfiguration configuration, ReportDbContext reportDbContex)
        {
            _reportDbContext = reportDbContex;
            _dataBaseMain = iDataBaseMain;
            _configuration = configuration;
            _dataBaseMain.ConntectionString = _dataBaseMain.ConntectionString + "AllowUserVariables=true;";
        }

        public Task<StimulSoftResult> ActionResultData(CommandJson commandJson,string userName)
        {
            commandJson.ConnectionString = _dataBaseMain.ConntectionString;
            StimulSoftResult result = new StimulSoftResult();
            if (commandJson.Database == "MySQL") result = MySQLAdapter.Process(commandJson,userName);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Получить содержимое файла отчета
        /// </summary>
        /// <param name="fileName">Имя файла, который нужно получить</param>
        /// <returns></returns>
        public async Task<string> GetFileMrt(int id)
        {
            var fileName = (await (_reportDbContext.s_documentreport.FirstOrDefaultAsync(x => x.Sysid == id))).NameReport + ".mrt";
            var pathReport = _configuration["PathDocumentReport"];
            return await File.ReadAllTextAsync(Path.Combine(pathReport, fileName));
        }
    }
}
