using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class SaleCar
    {
        public SaleCar()
        {
            CarPhotos = new HashSet<CarPhoto>();
            Warehouses = new HashSet<Warehouse>();
        }

        public int Id { get; set; }
        public string? Articul { get; set; }
        public int? CarId { get; set; }
        public int? EquipmentId { get; set; }
        public decimal? EquipmentPrice { get; set; }
        public string? ColorCar { get; set; }

        public virtual Car? Car { get; set; }
        public virtual Equipment? Equipment { get; set; }
        public virtual ICollection<CarPhoto> CarPhotos { get; set; }
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
