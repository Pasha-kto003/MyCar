using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Order
    {
        public Order()
        {
            CarOrderIns = new HashSet<CarOrderIn>();
            OrderOuts = new HashSet<OrderOut>();
        }

        public int Id { get; set; }
        public DateTime? DateOfOrder { get; set; }
        public int? ClientId { get; set; }
        public int? ActionTypeId { get; set; }

        public virtual ActionType? ActionType { get; set; }
        public virtual User? Client { get; set; }
        public virtual ICollection<CarOrderIn> CarOrderIns { get; set; }
        public virtual ICollection<OrderOut> OrderOuts { get; set; }
    }
}
