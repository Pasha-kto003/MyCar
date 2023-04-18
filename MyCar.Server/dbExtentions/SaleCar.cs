using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class SaleCar
    {
        public static explicit operator SaleCarApi(SaleCar saleCar)
        {
            return new SaleCarApi
            {
                ID = saleCar.Id,
                Articul = saleCar.Articul,
                CarId = saleCar.CarId,
                EquipmentId = saleCar.EquipmentId,
                EquipmentPrice = saleCar.EquipmentPrice,
                ColorCar = saleCar.ColorCar,
                MinCount = saleCar.MinCount
            };
        }

        public static explicit operator SaleCar(SaleCarApi saleCar)
        {
            return new SaleCar
            {
                Id = saleCar.ID,
                Articul = saleCar.Articul,
                CarId = saleCar.CarId,
                EquipmentId = saleCar.EquipmentId,
                EquipmentPrice = saleCar.EquipmentPrice,
                ColorCar = saleCar.ColorCar,
                MinCount = saleCar.MinCount
            };
        }
    }
}
