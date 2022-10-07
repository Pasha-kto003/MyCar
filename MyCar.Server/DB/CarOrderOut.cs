using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class CarOrderOut
    {
        public int CarOrderInId { get; set; }
        public int OrderOutId { get; set; }
        public int? Count { get; set; }
        public decimal? Price { get; set; }
        public double? Discount { get; set; }

        public virtual CarOrderIn CarOrderIn { get; set; } = null!;
        public virtual OrderOut OrderOut { get; set; } = null!;
    }
}
