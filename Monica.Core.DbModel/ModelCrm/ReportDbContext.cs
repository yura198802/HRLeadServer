using Microsoft.EntityFrameworkCore;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelCrm.Profile;

namespace Monica.Core.DbModel.ModelCrm
{
    /// <summary>
    /// Контекст для работы с данными кинфигуратора режимов
    /// </summary>
    public class ReportDbContext : BaseDbContext
    {

        /// <summary>
        /// Список режимов 
        /// </summary>
        public DbSet<FormModel> FormModel { get; set; }
        /// <summary>
        /// Спиок полей режима
        /// </summary>
        public DbSet<Field> Field { get; set; }
        /// <summary>
        /// Кнопки действий на форме
        /// </summary>
        public DbSet<ButtonForm> ButtonForm { get; set; }
        /// <summary>
        /// Профили режиму
        /// </summary>
        public DbSet<ProfileForm> ProfileForm { get; set; }
        /// <summary>
        /// Доступ к режиму или ее элементам
        /// </summary>
        public DbSet<AccessForm> AccessForm { get; set; }
        /// <summary>
        /// Тип режима
        /// </summary>
        public DbSet<TypeForm> TypeForm { get; set; }
        /// <summary>
        /// Список фильтров для панели фильтров
        /// </summary>
        public DbSet<FilterFormModel> FilterFormModel { get; set; }
        /// <summary>
        /// Список фильтров для диалоговой панели
        /// </summary>
        public DbSet<DialogButtonForm> DialogButtonForm { get; set; }
        /// <summary>
        /// Список фильтров для панели фильтров для пользователя
        /// </summary>
        public DbSet<FilterUser> FilterUser { get; set; }

        public DbSet<t_levelorg> t_levelorg { get; set; }
        /// <summary>
        /// Получить объекты для валидации данных
        /// </summary>
        public DbSet<ValidationRuleEntity> ValidationRuleEntity { get; set; }

        public DbSet<DocumentReport> s_documentreport { get; set; }
        public ReportDbContext(IDataBaseMain dataBaseMain) : base(dataBaseMain)
        {
        }
    }
}
