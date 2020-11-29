using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelDto.Core;
using MySql.Data.MySqlClient;

namespace Monica.Core.Service.ReportEngine.Actions
{
    public class ReportManagerData : ReportEngineDefaultData
    {
        private IManagerClients _managerClients;
        private IDataBaseMain _dataBaseMain;
        private ClientDbContext _clientDbContext;
        public ReportManagerData(ReportDbContext reportDbContext, IReportManager reportManager, IConnectorManager connectorManager, IDataBaseMain dataBaseMain, IManagerClients managerClients, ClientDbContext clientDbContext) : base(reportDbContext, reportManager, connectorManager)
        {
            _dataBaseMain = dataBaseMain;
            _managerClients = managerClients;
            _clientDbContext = clientDbContext;
        }

        public override async Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p)
        {
            using (var conn = new MySqlConnection(_dataBaseMain.ConntectionString))
            {

                var result = await conn.QueryAsync(@$"SELECT
                                                                          u.*, COUNT(DISTINCT m.Id) AS CountUser 
                                                                        FROM client c
                                                                          JOIN managerclient m ON c.Id = m.ClientId
                                                                          JOIN user u ON m.UserId = u.Id 
                                                                          JOIN clientproduct cp ON cp.ClientId = c.Id
                                                                          JOIN product p ON p.Id = cp.ProductId
                                                                        GROUP BY u.Id
                                                                        ORDER BY p.OrderIndex
                                                              ");
                return result.Cast<IDictionary<string, object>>();
            }
        }
    }
}
