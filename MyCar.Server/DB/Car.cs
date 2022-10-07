using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Car
    {
        public Car()
        {
            CarOrderIns = new HashSet<CarOrderIn>();
            CharacteristicCars = new HashSet<CharacteristicCar>();
        }

        public int Id { get; set; }
        public int? ModelId { get; set; }
        public int? TypeId { get; set; }
        public int? EquipmentId { get; set; }
        public string? Articul { get; set; }
        public byte[]? PhotoCar { get; set; }
        public decimal? CarPrice { get; set; }

        public virtual Equipment? Equipment { get; set; }
        public virtual Model? Model { get; set; }
        public virtual BodyType? Type { get; set; }
        public virtual ICollection<CarOrderIn> CarOrderIns { get; set; }
        public virtual ICollection<CharacteristicCar> CharacteristicCars { get; set; }
    }
}
