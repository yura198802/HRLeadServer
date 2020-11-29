using Autofac;
using Monica.Core.Attributes;
using Monica.Settings.DataAdapter.DataAdapter;
using Monica.Settings.DataAdapter.DataAdapter.Resources;
using Monica.Settings.DataAdapter.Interfaces; 


namespace Monica.Settings.DataAdapter.Autofac
{
    /// <summary>
    /// Модуль IoC контейнера
    /// </summary>
    [CommonModule]
    public class SettingsDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LevelOrgAdapter>().As<ILevelOrgAdapter>();
            builder.RegisterType<RolesAdapter>().As<IRolesAdapter>();
            builder.RegisterType<UsersAdapter>().As<IUsersAdapter>();
            builder.RegisterType<BtnsAdapter>().As<IBtnsAdapter>();
            builder.RegisterType<ModesAdapter>().As<IModesAdapter>();
            builder.RegisterType<FieldsAdapter>().As<IFieldsAdapter>();
        }
    }
}
