using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Models.Crm.Profile
{
    /// <summary>
    /// Роли пользователей. По умолчанию будет обычный пользователь и функциональный администратор
    /// для которого будет возможность редактирования всех режимов
    /// </summary>
    public class UserRole : BaseModel
    {
        /// <summary>
        /// Имя роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Системное имя роли
        /// </summary>
        public string Sysname { get; set; }
    }
}
