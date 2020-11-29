using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.DbModel.ModelCrm.Client;

namespace Monica.Core.Abstraction.Client
{
    /// <summary>
    /// Сервис работы с данными БД продукта клиента
    /// </summary>
    public interface IProductDataAdapter
    {
        /// <summary>
        /// Получить список продуктов
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetProducts();
        /// <summary>
        /// Получить список критериев для продукта, чтобы было удобней редактировать условие для продукта
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Criteria>> GetCriteria();
    }
}
