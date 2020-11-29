using DynamicExpresso;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Monica.Core.Abstraction.Crm;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Client;
using Monica.Core.DbModel.ModelCrm.Client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monica.Core.Service.Crm
{
    public class ManagerClients : IManagerClients
    {
        private ClientDbContext _dbContext;
        public ManagerClients(ClientDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddProductPostProcess(int productId)
        {
            var product = await _dbContext.Product.FirstOrDefaultAsync(p => p.Id == productId);

            var result = await _dbContext.ClientСriteria
                .Include(c => c.Client)
                .Include(c => c.Criteria)
                .ToArrayAsync();

            var clientСriterias = result.GroupBy(c => c.Client).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var client in clientСriterias)
            {
                var interpreter = new Interpreter();

                foreach (var clCriteria in client.Value)
                {
                    interpreter.SetVariable(clCriteria.Criteria.Name, clCriteria.GetValue());
                }

                if (product != null && interpreter.Eval<bool>(product.Expression))
                {
                    if (product != null)
                    {
                        var clientProduct = new ClientProduct
                        {
                            Client = client.Key,
                            Product = product,
                        };
                        await _dbContext.ClientProduct.AddAsync(clientProduct);

                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product[]> GetOrderedClientProducts(int clientId)
        {
            return await _dbContext.ClientProduct
                .Where(p => p.Client.Id == clientId)
                .Select(p => p.Product)
                .OrderBy(p => p.OrderIndex)
                .ToArrayAsync();
        }

        public async Task<DbModel.ModelCrm.Client.Client[]> GetOrderedManagerClients(int managerId)
        {

            var clients = await _dbContext.ManagerClient.Join(_dbContext.ClientProduct,
                p => p.Client,
                c => c.Client,
                (p, c) => new
                {
                    Client = p.Client,
                    Product = c.Product,
                    User = p.User,
                    Accepted = c.Accepted
                })
                .Where(d => !d.Accepted && d.User.Id == managerId)
                .OrderBy(d => d.Product.OrderIndex)
                .Select(d=>d.Client)
                .Distinct()
                .ToListAsync();

            var lastClients = new List<DbModel.ModelCrm.Client.Client>();
            var result = new List<DbModel.ModelCrm.Client.Client>();
            var managerActions = await _dbContext.ManagerAction
                    .Include(m => m.ClientProduct).ThenInclude(m => m.Client)
                    .Where(m => m.User.Id == managerId)
                    .OrderByDescending(m => m.DateTime)
                    .ToArrayAsync();
            foreach (var client in clients)
            {
                var ma = managerActions.FirstOrDefault(m => m.ClientProduct.Client.Id == client.Id && m.Result == ManagerActionResult.Refused);
                if (ma != null && ma.DateTime > DateTime.Now.AddDays(-7))
                {
                    lastClients.Add(client);
                    continue;
                }
                
                ma = managerActions.FirstOrDefault(m => m.ClientProduct.Client.Id == client.Id && m.Result == ManagerActionResult.CallAgain);
                if (ma != null && ma.CallAgainDateTime > DateTime.Now)
                {
                    lastClients.Add(client);
                    continue;
                }
                
                result.Add(client);
            }
            result.AddRange(lastClients);
            
            return result.ToArray();
        }

        public async Task AcceptProduct(int managerId, int[] clientProductIds)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == managerId);
            var acceptedClientProducts = await _dbContext.ClientProduct
                .Include(c => c.Client)
                .Where(c => clientProductIds.Contains(c.Id))
                .ToArrayAsync();
            if (user != null && acceptedClientProducts.Any())
            {
                var allClientProducts = await _dbContext.ClientProduct
                .Where(c => c.Client.Id == acceptedClientProducts[0].Client.Id)
                .ToArrayAsync();
                foreach (var cp in allClientProducts)
                {
                    var managerAction = new ManagerAction
                    {
                        User = user,
                        ClientProduct = cp,
                        DateTime = DateTime.Now
                    };
                    if (acceptedClientProducts.Contains(cp))
                    {
                        managerAction.Result = ManagerActionResult.Accept;
                        cp.Accepted = true;
                        _dbContext.Update(cp);
                    }
                    else
                    {
                        managerAction.Result = ManagerActionResult.Refused;
                    }
                    await _dbContext.ManagerAction.AddAsync(managerAction);
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CallAgainProduct(int managerId, int clientId, DateTime dateTime)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == managerId);
            var clientProducts = await _dbContext.ClientProduct
                .Where(c => c.Client.Id == clientId)
                .ToArrayAsync();
            if (user != null && clientProducts.Any())
            {
                foreach(var cp in clientProducts)
                {
                    var managerAction = new ManagerAction
                    {
                        User = user,
                        ClientProduct = cp,
                        Result = ManagerActionResult.Refused,
                        DateTime = DateTime.Now,
                        CallAgainDateTime = dateTime
                    };
                    await _dbContext.ManagerAction.AddAsync(managerAction);
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RefuseProduct(int managerId, int clientId)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.Id == managerId);
            var clientProducts = await _dbContext.ClientProduct
                .Where(c => c.Client.Id == clientId)
                .ToArrayAsync();
            if (user != null && clientProducts.Any())
            {
                foreach (var cp in clientProducts)
                {
                    var managerAction = new ManagerAction
                    {
                        User = user,
                        ClientProduct = cp,
                        Result = ManagerActionResult.Refused,
                        DateTime = DateTime.Now
                    };
                    await _dbContext.ManagerAction.AddAsync(managerAction);
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<DiagramData>> GetProductTypesProfit(int managerId)
        {
            var result = new List<DiagramData>();

            var clientProducts = await _dbContext.ManagerClient.Join(_dbContext.ClientProduct.Include(c => c.Client).Include(c=>c.Product).ThenInclude(c=>c.ProductType),
                p => p.Client,
                c => c.Client,
                (p, c) => new
                {
                    User = p.User,
                    ClientProduct = c
                })
                .Where(d => d.ClientProduct.Accepted && d.User.Id == managerId)
                .Select(d => d.ClientProduct)
                .Distinct()
                .ToListAsync();

            var clientIds = clientProducts.GroupBy(c => c.Client).Select(c => c.Key.Id);

            var cc = await _dbContext.ClientСriteria
                .Include(c => c.Client)
                .Include(c => c.Criteria)
                .Where(c => clientIds.Contains(c.Client.Id))
                .ToArrayAsync();

            var clientСriterias = cc.GroupBy(c => c.Client).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var cp in clientProducts)
            {
                var paramProfit = await _dbContext.ProductParam.FirstOrDefaultAsync(p => p.ProductParamType.Name == "Стоимость");
                if (paramProfit == null) continue;

                var interpreter = new Interpreter();

                foreach (var clCriteria in clientСriterias[cp.Client])
                {
                    interpreter.SetVariable(clCriteria.Criteria.Name, clCriteria.GetValue());
                }

                var diagramData = result.FirstOrDefault(c => c.Arg == cp.Product.ProductType.Name);
                if (diagramData != null)
                {
                    diagramData.Val += interpreter.Eval<double>(paramProfit.Expression);
                }
                else
                {
                    diagramData = new DiagramData
                    {
                        Arg = cp.Product.ProductType.Name,
                        Val = interpreter.Eval<double>(paramProfit.Expression)
                    };
                    result.Add(diagramData);
                }
            }
            return result;
        }

        public async Task<List<DiagramData>> GetClientWorkingTime(int managerId)
        {
            var result = new List<DiagramData>();

            var m = await _dbContext.ManagerAction
                .Include(c => c.ClientProduct).ThenInclude(c => c.Client)
                .Where(c => c.User.Id == managerId)
                .ToArrayAsync();
            var managerActions = m.GroupBy(m => m.ClientProduct.Client).Select(m => m.First())
                .OrderBy(c => c.DateTime).ToArray();

            var index = 0;
            foreach(var ma in managerActions)
            {
                var diagramData = result.FirstOrDefault(c => c.Arg == ma.ClientProduct.Client.Name);
                if (diagramData == null && index != 0)
                {
                    var previous = managerActions[index - 1];
                    var time = (ma.DateTime - previous.DateTime).TotalSeconds;
                    if (time < 60 * 60 * 4)
                    {
                        diagramData = new DiagramData
                        {
                            Arg = ma.ClientProduct.Client.Name,
                            Val = time
                        };
                        result.Add(diagramData);
                    }
                }
                index++;
            }

            return result;
        }
    }
}
