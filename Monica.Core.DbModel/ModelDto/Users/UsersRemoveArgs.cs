using System.Collections.Generic;
using Frgo.Dohod.DbModel.ModelDto.Core;

namespace Monica.Core.DbModel.ModelDto.Users
{
    public class UsersRemoveArgs : BaseRemoveArgs
    {
        public int RoleId { get; set; }
        public IEnumerable<string> Accounts { get; set; }
    }
}
