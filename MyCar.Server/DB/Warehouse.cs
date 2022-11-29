﻿using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Warehouse
    {
        public int Id { get; set; }
        public int? SaleCarId { get; set; }
        public int? OrderId { get; set; }
        public int? CountChange { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        public virtual Order? Order { get; set; }
        public virtual SaleCar? SaleCar { get; set; }
    }
}
