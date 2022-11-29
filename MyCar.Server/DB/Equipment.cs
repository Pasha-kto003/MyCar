using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Equipment
    {
        public Equipment()
        {
            SaleCars = new HashSet<SaleCar>();
        }

        public int Id { get; set; }
        public string? NameEquipment { get; set; }

        public virtual ICollection<SaleCar> SaleCars { get; set; }
    }
}
