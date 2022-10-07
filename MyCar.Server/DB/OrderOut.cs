using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class OrderOut
    {
        public OrderOut()
        {
            CarOrderOuts = new HashSet<CarOrderOut>();
        }

        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? StatusId { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Status? Status { get; set; }
        public virtual ICollection<CarOrderOut> CarOrderOuts { get; set; }
    }
}
