using Monica.Core.DbModel.ModelCrm.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Monica.Core.Abstraction.Crm
{
    public interface IManagerClients
    {
        Task AddProductPostProcess(int productId);
        Task<DbModel.ModelCrm.Client.Client[]> GetOrderedManagerClients(int managerId);
        Task<Product[]> GetOrderedClientProducts(int clientId);
        Task AcceptProduct(int managerId, int[] clientProductIds);
        Task CallAgainProduct(int managerId, int clientId, DateTime dateTime);
        Task RefuseProduct(int managerId, int clientId);

        Task<List<DiagramData>> GetProductTypesProfit(int managerId);
        Task<List<DiagramData>> GetClientWorkingTime(int managerId);
    }
}
