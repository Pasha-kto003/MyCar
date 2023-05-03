using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Order
    {
        public Order()
        {
            Warehouses = new HashSet<Warehouse>();
        }

        public int Id { get; set; }
        public DateTime? DateOfOrder { get; set; }
        public int? UserId { get; set; }
        public int? ActionTypeId { get; set; }
        public int? StatusId { get; set; }

        public virtual ActionType? ActionType { get; set; }
        public virtual Status? Status { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
