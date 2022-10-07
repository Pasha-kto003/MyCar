using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class CarOrderIn
    {
        public CarOrderIn()
        {
            CarOrderOuts = new HashSet<CarOrderOut>();
        }

        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public int? Count { get; set; }
        public decimal? Price { get; set; }
        public int? Remains { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Car? Product { get; set; }
        public virtual ICollection<CarOrderOut> CarOrderOuts { get; set; }
    }
}
