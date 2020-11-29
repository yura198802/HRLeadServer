using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Settings.DataAdapter.Models.Dto;

//using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Monica.Settings.DataAdapter.Models.Crm.Profile
{
    /// <summary>
    /// Пользователи системы
    /// </summary>
    public class User : BaseModel
    {
        /// <summary>
        /// Уникальное имя пользователя
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string Middlename { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email адрес обязательно с подтверждением
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Тип пользователя.Игрок, Менеджер
        /// </summary>
        [ForeignKey("TypeUserId")]
        public TypeUser TypeUser { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public int? TypeUserId { get; set; }
        public int? LevelOrgId { get; set; }
        [ForeignKey("LevelOrgId")]
        public t_levelorg Levelorg { get; set; }

        public static implicit operator UserDto(User user)
        {
            return new UserDto()
            {
                Account = user.Account,
                ShortName = $"{user.Name} {user.Surname}",
                FullName =$"{user.Surname} {user.Name} {user.Middlename}",
                Phone = user.Phone,
                Email =user.Email,
                Id = user.Id,
                Middlename = user.Middlename,
                Name = user.Name,
                Surname = user.Surname
            };
        }
    }
}
