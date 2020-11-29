using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Client;
using Monica.Core.DbModel.ModelDto.Client;

namespace Monica.Core.Abstraction.LoadInfo
{
    /// <summary>
    /// Сервис считывания и загрузки данных по транзакциям клиента из Excel файла
    /// </summary>
    public interface IWriterInfoTransaction
    {
        /// <summary>
        /// Считать информацию из Excel файла
        /// </summary>
        /// <param name="file">Битовый массив файла</param>
        /// <returns>Список клиента</returns>
        Task<List<DbModel.ModelCrm.Client.Transactions>> Reader(Stream file);
        /// <summary>
        /// Записать транзакцию в БД
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        Task WriterInfo(List<DbModel.ModelCrm.Client.Transactions> transactionses);
    }
}
