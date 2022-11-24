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
        public string? PhotoCar { get; set; }
        public decimal? CarPrice { get; set; }
        public string CarOptions { get; set; }
        public string CarMark { get; set; }

        public ModelApi Model { get; set; }
        public EquipmentApi Equipment { get; set; }
        public BodyTypeApi BodyType { get; set; }

        public List<CharacteristicCarApi> CharacteristicCars { get; set; }
    }
}
