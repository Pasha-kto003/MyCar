using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Warehouse
    {
        public static explicit operator WareHouseApi(Warehouse warehouse)
        {
            return new WareHouseApi
            {
                ID = warehouse.Id,
                CarId = warehouse.CarId,
                OrderId = warehouse.OrderId,
                Price = warehouse.Price,
                CountChange = warehouse.CountChange,
                Discount = warehouse.Discount
            };
        }
        public static explicit operator Warehouse(WareHouseApi warehouse)
        {
            return new Warehouse
            {
                Id = warehouse.ID,
                CarId = warehouse.CarId,
                OrderId = warehouse.OrderId,
                Price = warehouse.Price,
                CountChange = warehouse.CountChange,
                Discount = warehouse.Discount
            };
        }
    }
}
