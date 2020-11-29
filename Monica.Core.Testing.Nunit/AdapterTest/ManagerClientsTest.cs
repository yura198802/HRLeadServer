using Module.Testing.Nunit;
using Monica.Core.Abstraction.Crm;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Monica.Core.Testing.Nunit.AdapterTest
{
    public class ManagerClientsTest : BaseServiceTest<IManagerClients>
    {
        [Test]
        public async Task AddProductPostProcess()
        {
            try
            {
                await Service.AddProductPostProcess(3);
            }
            catch(Exception ex)
            {

            }
        }

        [Test]
        public async Task GetOrderedManagerClients()
        {
            try
            {
                await Service.GetOrderedManagerClients(1);
            }
            catch(Exception ex)
            {

            }
            
        }

        [Test]
        public async Task GetOrderedClientProducts()
        {
            try
            {
                await Service.GetOrderedClientProducts(1);
            }
            catch(Exception ex)
            {

            }
        }

    }
}
