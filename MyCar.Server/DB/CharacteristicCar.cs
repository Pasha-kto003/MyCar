using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class CharacteristicCar
    {
        public int CarId { get; set; }
        public int CharacteristicId { get; set; }
        public string? CharacteristicValue { get; set; }

        public virtual Car Car { get; set; } = null!;
        public virtual Characteristic Characteristic { get; set; } = null!;
    }
}
