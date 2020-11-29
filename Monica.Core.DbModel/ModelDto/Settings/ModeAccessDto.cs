using System.Collections.Generic;
using Monica.Core.DbModel.ModelCrm.Settings;

namespace Monica.Core.DbModel.ModelDto.Settings
{
    public class ModeAccessDto
    {
        public IEnumerable<ModeItem> Data { get; set; }
        public IEnumerable<int> Selected { get; set; }
    }
}
