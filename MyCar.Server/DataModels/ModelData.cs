using ModelsApi;
using MyCar.Server.DB;

namespace MyCar.Server.DataModels
{
    public static class ModelData 
    {
        public static CarApi GetCar(Car car, MyCar_DBContext dbContext)
        {
            var result = (CarApi)car;
            var characteristicsCar = dbContext.CharacteristicCars.Where(t => t.CarId == car.Id).Select(t => (CharacteristicCarApi)t).ToList();
            foreach (var characeristicCar in characteristicsCar)
            {
                characeristicCar.Characteristic = (CharacteristicApi)dbContext.Characteristics.FirstOrDefault(s => s.Id == characeristicCar.CharacteristicId);
                characeristicCar.Characteristic.Unit = (UnitApi)dbContext.Units.FirstOrDefault(s => s.Id == characeristicCar.Characteristic.UnitId);

            }
            result.CharacteristicCars = characteristicsCar;
            var model = dbContext.Models.FirstOrDefault(t => t.Id == car.ModelId);
            result.Model = (ModelApi)model;
            var mark = dbContext.MarkCars.FirstOrDefault(i => i.Id == model.MarkId);
            result.CarMark = mark.MarkName;
            //result.Model.MarkCar = (MarkCarApi)mark;
            var body = dbContext.BodyTypes.FirstOrDefault(b => b.Id == car.TypeId);
            result.BodyType = (BodyTypeApi)body;
            foreach (var characteristic in dbContext.CharacteristicCars.Where(s => s.CarId == car.Id).ToList())
            {
                characteristic.Characteristic = dbContext.Characteristics.FirstOrDefault(s => s.Id == characteristic.CharacteristicId);
                result.CarOptions += $"{characteristic.Characteristic.CharacteristicName} {characteristic.CharacteristicValue} \n";
            }
            return result;
        }

        public static SaleCarApi SaleGet(SaleCar saleCar, MyCar_DBContext dbContext)
        {
            var result = (SaleCarApi)saleCar;
            var equipment = dbContext.Equipment.FirstOrDefault(s => s.Id == saleCar.EquipmentId);
            result.Equipment = (EquipmentApi)equipment;
            var car = dbContext.Cars.FirstOrDefault(s => s.Id == saleCar.CarId);
            foreach (var item in dbContext.Warehouses.Where(s=>s.SaleCarId== saleCar.Id))
            {
                result.Count += item.CountChange;
            }
            result.Car = GetCar(car, dbContext);
            return result;
        }

        public static WareHouseApi WarehouseGet(Warehouse wareHouse, MyCar_DBContext dbContext)
        {
            var result = (WareHouseApi)wareHouse;
            var sale = dbContext.SaleCars.FirstOrDefault(s => s.Id == result.SaleCarId);
            result.SaleCar = SaleGet(sale, dbContext);
            return result;
        }

        public static OrderApi OrderGet(Order orderIn, MyCar_DBContext dbContext)
        {
            var result = (OrderApi)orderIn;
            var user = dbContext.Users.FirstOrDefault(x => x.Id == orderIn.UserId);
            result.User = (UserApi)user;
            var status = dbContext.Statuses.FirstOrDefault(x => x.Id == orderIn.StatusId);
            result.Status = (StatusApi)status;
            var actionType = dbContext.ActionTypes.FirstOrDefault(x => x.Id == orderIn.ActionTypeId);
            result.ActionType = (ActionTypeApi)actionType;
            result.WareHouses = dbContext.Warehouses.Where(s => s.OrderId == orderIn.Id).Select(t => (WareHouseApi)t).ToList();
            var warehouses = dbContext.Warehouses.ToList().Select(s =>
            {
                return WarehouseGet(s, dbContext);
            });
            result.WareHouses = warehouses.Select(s=> (WareHouseApi)s).ToList();
            return result;
        }
    }
}
