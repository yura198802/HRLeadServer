using Autofac;
using Monica.Core.Abstraction.Profile;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.Attributes;
using Monica.Core.Service.Crm.ReportEngine;
using Monica.Core.Service.Profile;
using Monica.Core.Service.ReportEngine;

namespace Monica.Core.Service.Autofac
{
    /// <summary>
    /// Модуль IoC контейнера
    /// </summary>
    [CommonModule]
    public class MonicaDataBaseWorkingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Загрузить работу с профилями пользователей
            builder.RegisterType<ManagerProfile>().As<IManagerProfile>();
            // Загрузить работу с доступом
            builder.RegisterType<AccessManager>().As<IAccessManager>();
            // Загрузить работу с данными
            builder.RegisterType<ReportData>().As<IReportData>();
            //Менеджер управления режимами
            builder.RegisterType<ReportManager>().As<IReportManager>();
            // Загрузить генератор данных под структуру MySql
            builder.RegisterType<GenerateFieldMySql>().Named<IGenerateField>(nameof(GenerateFieldMySql));
            //Менеджер получение объектов БД
            builder.RegisterType<ConnectorManager>().As<IConnectorManager>();
            //Регистрация сервиса формированрия данных для конструктора по умолчанию
            builder.RegisterType<ReportEngineDefaultData>().Named<IReportEngineData>(nameof(ReportEngineDefaultData));
            //Регистрация генератора колонок по умолчанию
            builder.RegisterType<ColumnCreater>().As<IColumnCreater>();
            //Регистрация генератора кнопок по умолчанию
            builder.RegisterType<ButtonCreater>().As<IButtonCreater>();
            //Регистрация генератора панели фильтров
            builder.RegisterType<FilterPanelPropertysCreater>().As<IFilterPanelPropertysCreater>();
            //Регистрация работы модуля редактирования формы
            builder.RegisterType<SingleForm>().As<ISingleForm>();

            builder.RegisterType<RequestEngine>().Named<IReportEngineData>(nameof(RequestEngine));


        }
    }
}
