using Monica.Core.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto.Users
{
    public class UserRoleDto : BaseModelDto
    {
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// SysId роли
        /// </summary>
        public int SysIsd { get; set; }
        // public string Sysname { get; set; }
    }
}
