using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelDto.Core;
using MySql.Data.MySqlClient;

namespace Monica.Core.Service.ReportEngine.Actions
{
    public class ReportClientProductData : ReportEngineDefaultData
    {
        private IDataBaseMain _dataBaseMain;

        public ReportClientProductData(ReportDbContext reportDbContext, IReportManager reportManager, IConnectorManager connectorManager, IDataBaseMain dataBaseMain) : base(reportDbContext, reportManager, connectorManager)
        {
            _dataBaseMain = dataBaseMain;
        }

        public override async Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p)
        {
            using (var conn = new MySqlConnection(_dataBaseMain.ConntectionString))
            {
                
                var result = await conn.QueryAsync(@$"SELECT c.Id, p.Name AS ProductName FROM clientproduct c
                                                                            JOIN product p ON c.ProductId = p.Id AND c.ClientId = {p.FormIdByDetail} group by p.Id;
                                                              "); 
                return result.Cast<IDictionary<string, object>>();
            }
        }
    }
}
