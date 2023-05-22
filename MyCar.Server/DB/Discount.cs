using System;
using System.Collections.Generic;

namespace MyCar.Server.DB
{
    public partial class Discount
    {
        public int Id { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SaleCarId { get; set; }

        public virtual SaleCar? SaleCar { get; set; }
    }
}
