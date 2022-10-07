using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class EquipmentApi : ApiBaseType
    {
        public string? NameEquipment { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
