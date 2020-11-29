using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Monica.Core.ModelParametrs.ModelsArgs
{
    public class ActionArgs
    {
        public int[] Ids { get; set; }
        public IDictionary<string, object> OtherInfo { get; set; }
        public IFormFileCollection FormFileCollection { get; set; }
    }

    public class SaveModelArgs
    {
        public IDictionary<string,object> SaveModel { get; set; }
        public IDictionary<string, object> FilterModel { get; set; }
    }
}
