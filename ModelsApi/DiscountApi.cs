using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class DiscountApi : ApiBaseType
    {
        public decimal? DiscountValue { get; set; }
        public decimal PercentValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SaleCarId { get; set; }

        public SaleCarApi? SaleCar { get; set; }
    }
}
