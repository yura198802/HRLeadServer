using System.Collections.Generic;

namespace Monica.Core.DbModel.ModelDto.Roles
{
    public class RoleLinkArgs
    {
        public IEnumerable<int> IdUsers { get; set; }
        public int IdRole { get; set; }
    }
}
