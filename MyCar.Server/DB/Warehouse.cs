using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Warehouse
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        public int? OrderId { get; set; }
        public int? CountChange { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        public virtual Car? Car { get; set; }
        public virtual Order? Order { get; set; }
    }
}
