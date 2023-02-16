using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class MarkCarApi : ApiBaseType
    {
        public string? MarkName { get; set; }
        public string? MarkPhoto { get; set; }

        public List<ModelApi>? Models { get;set; }
        public ModelApi? SelectedModel { get; set; }
    }
}
