using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.Utils;
using Moq;

namespace Module.Testing.Nunit
{
    /// <summary>
    /// Базовый класс для построения интеграционных тестов
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class BaseServiceTest<TService>
    {
        protected TService Service;

        protected BaseServiceTest()
        {
            var mockDataBaseMain = new Mock<IDataBaseMain>();
            var mockDataBaseIs4 = new Mock<IDataBaseIs4>();
            var logger = new Mock<ILogger>();

            var configiguration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var connectionString = configiguration.ConnectionStrings.ConnectionStrings["MySqlDatabase"].ConnectionString;
            var connectionStringIs4 = configiguration.ConnectionStrings.ConnectionStrings["MySqlDatabaseIS4"].ConnectionString;

            mockDataBaseMain.Setup(main => main.ConntectionString).Returns(connectionString);
            mockDataBaseIs4.Setup(main => main.ConntectionString).Returns(connectionStringIs4);
            var services = new ServiceCollection();
            services.AddSingleton(mockDataBaseMain.Object);
            services.AddSingleton(mockDataBaseIs4.Object);
            services.AddDbContextCore();
            services.AddDbContext<ClientDbContext>(ServiceLifetime.Transient);
            //services.AddDbContext<HrDbContext>(ServiceLifetime.Transient);


            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (var file in files)
            {
                if (file.IndexOf("testhost.dll", StringComparison.Ordinal) > -1)
                {
                    continue;
                }

                try
                {
                    System.Runtime.Loader.AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(file);
                }
                catch(Exception ex) { Console.WriteLine(ex.Message); }
            }

            AutoFac.Init(DataBaseName.MySql, builder =>
            {
                builder.Populate(services);
                builder.RegisterInstance(logger.Object);
                builder.RegisterInstance(mockDataBaseMain.Object);

            });
            Service = AutoFac.Resolve<TService>();
        }
    }
}
