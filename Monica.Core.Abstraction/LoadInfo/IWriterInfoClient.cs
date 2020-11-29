using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelDto.Client;

namespace Monica.Core.Abstraction.LoadInfo
{
    /// <summary>
    /// Сервис считывания и загрузки данных по клиенту из Excel файла
    /// </summary>
    public interface IWriterInfoClient
    {
        /// <summary>
        /// Считать информацию из Excel файла
        /// </summary>
        /// <param name="file">Битовый массив файла</param>
        /// <returns>Список клиента</returns>
        Task<List<ClientDto>> ReaderClient(Stream file);
        /// <summary>
        /// Записать клиента в БД
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        Task WriterInfo(List<ClientDto> clients);
    }
}
