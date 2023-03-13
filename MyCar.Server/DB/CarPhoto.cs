using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class CarPhoto
    {
        public int Id { get; set; }
        public string? PhotoName { get; set; }
        public int? SaleCarId { get; set; }

        public virtual SaleCar? SaleCar { get; set; }
    }
}
