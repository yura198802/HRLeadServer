using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Monica.Core.Abstraction.Client;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Client;

namespace Monica.Core.Service.Client
{
    public class ProdutDataAdapter : IProductDataAdapter
    {
        private ClientDbContext _clientDbContext;

        public ProdutDataAdapter(ClientDbContext clientDbContext)
        {
            _clientDbContext = clientDbContext;
        }

        public Task<IEnumerable<Product>> GetProducts()
        {
            //var products = _clientDbContext.Client
            return null;
        }

        public Task<IEnumerable<Criteria>> GetCriteria()
        {
            throw new NotImplementedException();
        }
    }
}
