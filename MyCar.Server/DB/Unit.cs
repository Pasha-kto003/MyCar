using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Unit
    {
        public Unit()
        {
            Characteristics = new HashSet<Characteristic>();
        }

        public int Id { get; set; }
        public string? UnitName { get; set; }

        public virtual ICollection<Characteristic> Characteristics { get; set; }
    }
}
