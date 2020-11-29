using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.Abstraction.LoadInfo;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Client;
using Monica.Core.DbModel.ModelDto.Client;
using OfficeOpenXml;

namespace Monica.Core.Service.LoadInfo
{
    public class WriterInfoClient : IWriterInfoClient
    {
        private ClientDbContext _clientDbContext;

        public WriterInfoClient(ClientDbContext clientDbContext)
        {
            _clientDbContext = clientDbContext;
        }


        public Task<List<ClientDto>> ReaderClient(Stream stream)
        {
            ExcelPackage excelPackage = new ExcelPackage(stream);
            var clients = new List<ClientDto>();
            for (int i = 2; i < excelPackage.Workbook.Worksheets[0].Cells.Rows; i++)
            {
                var client = new ClientDto();
                client.client_id = excelPackage.Workbook.Worksheets[0].Cells[i, 1].GetValue<int>();
                if (client.client_id == 0)
                    break;
                client.age = excelPackage.Workbook.Worksheets[0].Cells[i, 2].GetValue<int>();
                client.gender_code = excelPackage.Workbook.Worksheets[0].Cells[i, 3].GetValue<string>();
                client.directory = excelPackage.Workbook.Worksheets[0].Cells[i, 4].GetValue<string>();
                client.aMRG_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 5].GetValue<decimal>();
                client.aCSH_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 6].GetValue<decimal>();
                client.aCRD_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 7].GetValue<decimal>();
                client.pCUR_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 8].GetValue<decimal>();
                client.pCRD_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 9].GetValue<decimal>();
                client.pSAV_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 10].GetValue<decimal>();
                client.pDEP_eop = excelPackage.Workbook.Worksheets[0].Cells[i, 11].GetValue<decimal>();
                client.sWork_S = excelPackage.Workbook.Worksheets[0].Cells[i, 12].GetValue<decimal>();
                client.tPOS_S = excelPackage.Workbook.Worksheets[0].Cells[i, 13].GetValue<decimal>();
                clients.Add(client);
            }

            return Task.FromResult(clients);
        }

        public async Task WriterInfo(List<ClientDto> clients)
        {
            foreach (var client in clients.Select(clientDto => clientDto.Map<DbModel.ModelCrm.Client.Client>()))
            {
                await _clientDbContext.AddAsync(client);
            }

            await _clientDbContext.SaveChangesAsync();
        }
    }
}
