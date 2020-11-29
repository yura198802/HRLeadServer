using Autofac;
using Frgo.Dohod.DbModel.DataAdapter.Settings;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.Abstraction.Testing.Vacancy;
using Monica.Core.Attributes;
using Monica.Core.Service.Crm.Settings;
using Monica.Core.Service.Crm.Settings.Resources;
using Monica.Core.Service.Testing.Candidates; //using Frgo.Dohod.DbModel.DataAdapter.LevelOrg;


namespace Monica.Core.Service.Autofac
{
    /// <summary>
    /// Модуль IoC контейнера
    /// </summary>
    [CommonModule]
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LevelOrgAdapter>().As<ILevelOrgAdapter>();
            builder.RegisterType<RolesAdapter>().As<IRolesAdapter>();
            builder.RegisterType<TypeLevelAdapter>().As<ITypeLevelAdapter>();
            builder.RegisterType<UsersAdapter>().As<IUsersAdapter>();
            builder.RegisterType<BtnsAdapter>().As<IBtnsAdapter>();
            builder.RegisterType<ModesAdapter>().As<IModesAdapter>();
            builder.RegisterType<FieldsAdapter>().As<IFieldsAdapter>();
            builder.RegisterType<VacancyTestingAdapter>().As<IVacancyTestingAdapter>();
        }
    }
}
