using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Equipment
    {
        public Equipment()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string? NameEquipment { get; set; }
        public decimal? MinPrice { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
