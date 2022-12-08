using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class SaleCarApi : ApiBaseType
    {
        public string? Articul { get; set; }
        public int? CarId { get; set; }
        public int? EquipmentId { get; set; }
        public decimal? EquipmentPrice { get; set; }
        public decimal? FullPrice { get => EquipmentPrice + Car.CarPrice; }
        public int? Count { get; set; } = 0;

        public CarApi Car { get; set; }
        public EquipmentApi Equipment { get; set; }
    }
}
