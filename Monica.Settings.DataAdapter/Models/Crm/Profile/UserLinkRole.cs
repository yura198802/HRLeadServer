using System.ComponentModel.DataAnnotations.Schema;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.Models.Crm.Profile
{
    /// <summary>
    /// Связь многие ко многим между пользователем и ролью
    /// </summary>
    public class UserLinkRole : BaseModel
    {
        [ForeignKey("UserId")]
        public User user { get; set; }
        [ForeignKey("UserRoleId")]
        public UserRole UserRole { get; set; }
        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Ссылка на роль
        /// </summary>
        public int UserRoleId { get; set; }
    }
}
