﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class WareHouseApi : ApiBaseType
    {
        public int? SaleCarId { get; set; }
        public int? OrderId { get; set; }
        public int? CountChange { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int CountRemains { get; set; } = 0;

        public List<CountChangeHistoryApi> CountChangeHistories { get; set; } = new List<CountChangeHistoryApi>();
        public SaleCarApi SaleCar { get; set; }
    }
}
