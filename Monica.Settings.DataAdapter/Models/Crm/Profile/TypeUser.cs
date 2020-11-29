using System.ComponentModel.DataAnnotations;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Models.Crm.Profile
{
    /// <summary>
    /// Тип пользователя системы.
    /// </summary>
    public class TypeUser : BaseModel
    {
        /// <summary>
        /// Имя типа пользователя. Менеджер, Клиент, Руководитель и т.д.
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Системное имя 
        /// </summary>
        [MaxLength(50)]
        public string Sysname { get; set; }
    }
}
