using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Module.Testing.Nunit;
using Monica.Core.Abstraction.LoadInfo;
using NUnit.Framework;

namespace Monica.Core.Testing.Nunit.AdapterTest
{
    public class WriterInfoClientTest : BaseServiceTest<IWriterInfoClient>
    {
        [Test]
        public async Task ReaderInfoClientNormal()
        {
            var file = new MemoryStream(File.ReadAllBytes("D:\\Clients.xlsx"));
            var res = await Service.ReaderClient(file);
            Assert.IsTrue(res.Any());
        }

        [Test]
        public async Task WriterInfoClientNormal()
        {
            var file = new MemoryStream(File.ReadAllBytes("D:\\Clients.xlsx"));
            var res = await Service.ReaderClient(file);
            await Service.WriterInfo(res);
            Assert.IsTrue(res.Any());
        }
    }
}
