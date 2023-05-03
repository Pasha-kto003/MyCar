using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class CountChangeHistory
    {
        public int Id { get; set; }
        public int? WarehouseOutId { get; set; }
        public int? WarehouseInId { get; set; }
        public int? Count { get; set; }

        public virtual Warehouse? WarehouseIn { get; set; }
        public virtual Warehouse? WarehouseOut { get; set; }
    }
}
