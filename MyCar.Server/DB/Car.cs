using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Car
    {
        public Car()
        {
            CharacteristicCars = new HashSet<CharacteristicCar>();
            SaleCars = new HashSet<SaleCar>();
        }

        public int Id { get; set; }
        public int? ModelId { get; set; }
        public int? TypeId { get; set; }
        public string? Articul { get; set; }
        public string? PhotoCar { get; set; }
        public decimal? CarPrice { get; set; }

        public virtual Model? Model { get; set; }
        public virtual BodyType? Type { get; set; }
        public virtual ICollection<CharacteristicCar> CharacteristicCars { get; set; }
        public virtual ICollection<SaleCar> SaleCars { get; set; }
    }
}
