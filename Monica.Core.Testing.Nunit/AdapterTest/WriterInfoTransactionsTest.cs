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
    public class WriterInfoTransactionsTest : BaseServiceTest<IWriterInfoTransaction>
    {
        [Test]
        public async Task ReaderInfoNormal()
        {
            var file = new MemoryStream(File.ReadAllBytes("D:\\Transactions.xlsx"));
            var res = await Service.Reader(file);
            Assert.IsTrue(res.Any());
        }

        [Test]
        public async Task WriterInfoNormal()
        {
            var file = new MemoryStream(File.ReadAllBytes("D:\\Transactions.xlsx"));
            var res = await Service.Reader(file);
            await Service.WriterInfo(res);
            Assert.IsTrue(res.Any());
        }
    }
}
