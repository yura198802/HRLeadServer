using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.Service.ReportEngine;
using SqlKata.Execution;

namespace Monica.Core.Service.Crm.ReportEngine
{
    public class RequestEngine : ReportEngineDefaultData
    {
        public RequestEngine(ReportDbContext reportDbContext, IReportManager reportManager, IConnectorManager connectorManager) : base(reportDbContext, reportManager, connectorManager)
        {
        }

        public override async Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p)
        {
            var formModel = await ReportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
            if (formModel == null)
                return null;
            var fileds = (await ReportManager.GetFieldsFormAsync(p.UserName, p.FormId, false, fields =>
                fields.Where(f => !(f.IsVirtual ?? false)))).ToList();
            if (fileds.Count == 0)
                return null;
            using var connection = ConnectorManager.GetConnection();
            var db = new QueryFactory(connection, ConnectorManager.Compiler);
            var resQuery = await db.Query("Requests").GetAsync();


            return resQuery.Cast<IDictionary<string, object>>();
        }

        public override async Task<IDictionary<string, object>> GetDataEditModel(BaseModelReportParam p)
        {
            var formModel = await ReportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
            if (formModel == null)
                return null;
            var fileds = (await ReportManager.GetFieldsFormAsync(p.UserName, p.FormId, false, fields =>
                fields.Where(f => !(f.IsVirtual ?? false)))).ToList();
            if (fileds.Count == 0)
                return null;
            using var connection = ConnectorManager.GetConnection();
            var db = new QueryFactory(connection, ConnectorManager.Compiler);
            var resQuery = await db.Query("Requests").Where( "Id", p.ModelId).FirstOrDefaultAsync();
            var resQueryItems = await db.Query("requestrequirements").Where("RequestId", p.ModelId)
                .SelectRaw("Id as id, Text as value, Id as modelId").GetAsync();
            var results = resQuery as IDictionary<string, object>;
            if (results != null)
            {
                results.Add("Requests", resQueryItems.Cast<IDictionary<string, object>>());
                return results;
            }

            return null;
        }
    }
}
