using Monica.CrmDbModel.ModelDto.Core;

namespace Monica.Settings.DataAdapter.Models.Dto
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