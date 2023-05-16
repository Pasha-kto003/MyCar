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
        public string? ColorCar { get; set; }
        public int? Count { get; set; } = 0;
        public int? MinCount { get; set; }
        public CarApi Car { get; set; }
        public string? ColorCarForXaml { get => "#" + ColorCar; }
        public string? HexColor { get; set; }
        public string? Description { get; set; }
        public decimal? FullPrice { get => EquipmentPrice + Car.CarPrice; }
        //public string? MainPhotoCar { get => CarPhotos.FirstOrDefault(s=>s.); set; }

        /// <summary>
        /// Свойство для поиска на сайте 
        /// </summary>
        public string? FullName
        {
            get => Car.CarName;
        }

        /// <summary>
        /// Свойство для подсчета на сайте 
        /// </summary>
        public int? CountChange { get; set; } = 0;

        public string? MainPhotoCar { get; set; }
        public EquipmentApi Equipment { get; set; }
        public List<CarPhotoApi> CarPhotos { get; set; }
        
    }
}
