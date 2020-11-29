using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Client;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm
{
    /// <summary>
    /// Контекст для работы с данными кинфигуратора режимов
    /// </summary>
    public class ClientDbContext : BaseDbContext
    {
        /// <summary>
        /// Клиенты
        /// </summary>
        public DbSet<Client.Client> Client { get; set; }
        /// <summary>
        /// Клиенты менеджера
        /// </summary>
        public DbSet<Client.ManagerClient> ManagerClient { get; set; }
        /// <summary>
        /// Действия менеджера
        /// </summary>
        public DbSet<Client.ManagerAction> ManagerAction { get; set; }

        /// <summary>
        /// Критэрии
        /// </summary>
        public DbSet<Client.Criteria> Сriteria { get; set; }
        /// <summary>
        /// Продукты
        /// </summary>
        public DbSet<Client.Product> Product { get; set; }
        /// <summary>
        /// Типы продукта
        /// </summary>
        public DbSet<Client.ProductType> ProductType { get; set; }
        /// <summary>
        /// Параметры продукта
        /// </summary>
        public DbSet<Client.ProductParam> ProductParam { get; set; }
        /// <summary>
        /// Типы параметров продукта
        /// </summary>
        public DbSet<Client.ProductParamType> ProductParamType { get; set; }
        /// <summary>
        /// Критэрии клиента
        /// </summary>
        public DbSet<Client.ClientCriteria> ClientСriteria { get; set; }
        /// <summary>
        /// Продукты клиента
        /// </summary>
        public DbSet<Client.ClientProduct> ClientProduct { get; set; }
        /// <summary>
        /// Транзакции
        /// </summary>
        public DbSet<Transactions> Transactions { get; set; }

        public ClientDbContext(IDataBaseMain dataBaseMain) : base(dataBaseMain)
        {

        }
    }
}
