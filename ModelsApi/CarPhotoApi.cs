using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class CarPhotoApi : ApiBaseType
    {
        public string? PhotoName { get; set; }
        public int? SaleCarId { get; set; }

        public SaleCarApi? SaleCar { get; set; }
    }
}
