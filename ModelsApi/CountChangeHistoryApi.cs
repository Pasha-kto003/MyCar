using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class CountChangeHistoryApi : ApiBaseType
    {
        public int? WarehouseOutId { get; set; }
        public int? WarehouseInId { get; set; }
        public int? Count { get; set; }

        public WareHouseApi? WarehouseIn { get; set; }
        public WareHouseApi? WarehouseOut { get; set; }
    }
}
