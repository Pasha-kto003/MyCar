using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class ModelApi : ApiBaseType
    {
        public string ModelName { get; set; }
        public int? MarkId { get; set; }

        //public MarkCarApi MarkCar { get; set; }
    }
}
