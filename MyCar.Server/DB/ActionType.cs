using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class ActionType
    {
        public ActionType()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? ActionTypeName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
