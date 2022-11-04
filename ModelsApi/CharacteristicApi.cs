using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class CharacteristicApi : ApiBaseType
    {
        public string? CharacteristicName { get; set; }
        public int? UnitId { get; set; }

        public UnitApi Unit { get; set; }
    }
}
