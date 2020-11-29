using Monica.Core.DbModel.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Monica.Core.Service.Identity.Models
{
    /// <summary>
    /// Модель для получения списка пользователей с IS
    /// </summary>
    public class IdentityUsersDto
    {
        public IEnumerable<UserDto> Users { get; set; }
    }
}
