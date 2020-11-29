using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    /// <summary>
    /// Компоненты для фильтра основного компонента
    /// </summary>
    public class FilterFormModel : BaseModel, IFilterForm
    {
        /// <summary>
        /// Тип компонента
        /// </summary>
        public TypeFilter TypeFilter { get; set; }
        /// <summary>
        /// Ссылка на сущность для которой делается фильтр
        /// </summary>
        public FormModel FormModel { get; set; }
        /// <summary>
        /// Условие, которое будет формироваться для фильтрации списка
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// Системное имя фильтра
        /// </summary>
        public string Sysname { get; set; }
        /// <summary>
        /// Признак, что фильтр виден на экране
        /// </summary>
        public bool? IsVisibleFilter { get; set; }
        /// <summary>
        /// Запрос для получения дополнительных значений 
        /// </summary>
        public string SqlData { get; set; }

        public string Caption { get; set; }
        public int? Order { get; set; }
        public int? ParentId { get; set; }
    }

    public interface IFilterForm
    {
        public int Id { get; set; }
        public TypeFilter TypeFilter { get; set; }
        public string Sysname { get; set; }
        public string SqlData { get; set; }
        public string Caption { get; set; }
        public int? Order { get; set; }
        public int? ParentId { get; set; }
    }
}
