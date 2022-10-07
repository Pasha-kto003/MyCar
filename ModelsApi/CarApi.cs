using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsApi
{
    public class CarApi : ApiBaseType
    {
        public int? ModelId { get; set; }
        public int? TypeId { get; set; }
        public int? EquipmentId { get; set; }
        public string? Articul { get; set; }
        public byte[]? PhotoCar { get; set; }
        public decimal? CarPrice { get; set; }

        public List<CharacteristicCarApi> CharacteristicCars { get; set; }
    }
}
