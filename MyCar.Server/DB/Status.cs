using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Status
    {
        public Status()
        {
            OrderOuts = new HashSet<OrderOut>();
        }

        public int Id { get; set; }
        public string? StatusName { get; set; }

        public virtual ICollection<OrderOut> OrderOuts { get; set; }
    }
}
