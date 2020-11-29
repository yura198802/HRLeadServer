using System.Collections.Generic;

namespace Monica.Settings.DataAdapter.Models.Dto
{
    public class ModeAccessDto
    {
        public IEnumerable<ModeItem> Data { get; set; }
        public IEnumerable<int> Selected { get; set; }
    }
}
