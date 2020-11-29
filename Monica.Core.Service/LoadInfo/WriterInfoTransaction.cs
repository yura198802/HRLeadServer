using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.Abstraction.LoadInfo;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Client;
using Monica.Core.DbModel.ModelDto.Client;
using OfficeOpenXml;

namespace Monica.Core.Service.LoadInfo
{
    public class WriterInfoTransaction : IWriterInfoTransaction
    {
        private ClientDbContext _clientDbContext;

        public WriterInfoTransaction(ClientDbContext clientDbContext)
        {
            _clientDbContext = clientDbContext;
        }

        public Task<List<Transactions>> Reader(Stream file)
        {
            ExcelPackage excelPackage = new ExcelPackage(file);
            var transactionses = new List<Transactions>();
            for (int i = 2; i < excelPackage.Workbook.Worksheets[0].Cells.Rows; i++)
            {
                var trabsaction = new Transactions();
                trabsaction.client_id = excelPackage.Workbook.Worksheets[0].Cells[i, 1].GetValue<int>();
                if (trabsaction.client_id == 0)
                    break;
                trabsaction.TRANSACTION_DT = excelPackage.Workbook.Worksheets[0].Cells[i, 2].GetValue<DateTime>();
                trabsaction.MCC_KIND_CD = excelPackage.Workbook.Worksheets[0].Cells[i, 3].GetValue<string>();
                trabsaction.MCC_CD = excelPackage.Workbook.Worksheets[0].Cells[i, 4].GetValue<string>() == string.Empty ? 0 : excelPackage.Workbook.Worksheets[0].Cells[i, 4].GetValue<int>();
                trabsaction.CARD_AMOUNT_EQV_CBR = excelPackage.Workbook.Worksheets[0].Cells[i, 5].GetValue<decimal>();
                transactionses.Add(trabsaction);
            }

            return Task.FromResult(transactionses);
        }

        public async Task WriterInfo(List<Transactions> transactionses)
        {
            foreach (Transactions transactionse in transactionses)
            {
                await _clientDbContext.AddAsync(transactionse);
            }

            await _clientDbContext.SaveChangesAsync();
        }
    }
}
