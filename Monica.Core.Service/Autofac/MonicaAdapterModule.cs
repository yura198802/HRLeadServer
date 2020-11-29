using Autofac;
using Monica.Core.Abstraction.Crm;
using Monica.Core.Abstraction.Registration;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.Abstraction.Stimulsoft;
using Monica.Core.Attributes;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.Service.Crm;
using Monica.Core.Service.Crm.ReportEngine;
using Monica.Core.Service.Registration;
using Monica.Core.Service.Stimulsoft;
using Monica.PlatformMain.LoaderModules;

namespace Monica.Core.Service.Autofac
{
    /// <summary>
    /// Модуль автофака
    /// </summary>
    [CommonModule]
    public class MonicaAdapterModule : Module
    {
        /// <summary>
        /// загрузить записимости
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RegistrationUserAdapter>().As<IRegistrationUserAdapter>();
            builder.RegisterType<StimulsoftEngineAdapter>().As<IStimulsoftEngine>();
            //builder.RegisterType<DataBaseMain>().As<IDataBaseMain>();
            //builder.RegisterType<DataBaseIs4>().As<IDataBaseIs4>().InstancePerLifetimeScope();
            builder.RegisterType<ReportDbContext>();

            builder.RegisterType<HrDbContext>();
            builder.RegisterType<HrService>().As<IHrService>();
            builder.RegisterType<ActionCreaterVacancyByRequest>()
                .Named<IActionBtnFormModel>("ActionCreaterVacancyByRequest");
            builder.RegisterType<ActionPublishVacancy>()
                .Named<IActionBtnFormModel>("ActionPublishVacancy");
            
        }
    }
}
