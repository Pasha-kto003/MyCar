using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Car
    {
        public static explicit operator CarApi(Car car)
        {
            return new CarApi
            {
                ID = car.Id,
                Articul = car.Articul,
                CarPrice = car.CarPrice,
                ModelId = car.ModelId,
                PhotoCar = car.PhotoCar,
                TypeId = car.TypeId
            };
        }

        public static explicit operator Car(CarApi car)
        {
            return new Car
            {
                Id = car.ID,
                Articul = car.Articul,
                CarPrice = car.CarPrice,
                ModelId = car.ModelId,
                PhotoCar = car.PhotoCar,
                TypeId = car.TypeId
            };
        }
    }
}
