using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Characteristic
    {
        public Characteristic()
        {
            CharacteristicCars = new HashSet<CharacteristicCar>();
        }

        public int Id { get; set; }
        public string? CharacteristicName { get; set; }
        public int? UnitId { get; set; }

        public virtual Unit? Unit { get; set; }
        public virtual ICollection<CharacteristicCar> CharacteristicCars { get; set; }
    }
}
