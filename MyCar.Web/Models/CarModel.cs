using ModelsApi;

namespace MyCar.Web.Models
{
    public class CarModel
    {
        public int? ModelId { get; set; }
        public int? TypeId { get; set; }
        public string? Articul { get; set; }
        public string? PhotoCar { get; set; }
        public decimal? CarPrice { get; set; }
        public string? CarOptions { get; set; }
        public string? CarMark { get; set; }

        public string? CarName { get; set; }

        public ModelApi Model { get; set; }
        public BodyTypeApi BodyType { get; set; }

        public List<CharacteristicCarApi> CharacteristicCars { get; set; }
    }
}
