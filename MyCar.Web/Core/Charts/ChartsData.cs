using ModelsApi;
using MyCar.Server.DB;
using System.Data;

namespace MyCar.Web.Core.Charts
{
    public class ChartsData
    {
        public static List<OrderApi> Orders = new List<OrderApi>();
        public static List<WareHouseApi> Warehouses = new List<WareHouseApi>();

        private static DateTime dateNow = DateTime.Now;
        public static DateTime DateNow
        {
            get => dateNow;
            set
            {
                dateNow = value;
            }
        }

        private static DateTime dateCompare = DateTime.Now.AddMonths(-1);
        public static DateTime DateCompare
        {
            get => dateCompare;
            set
            {
                dateCompare = value;
            }
        }

        private decimal profitNow;
        public decimal ProfitNow
        {
            get => profitNow;
            set
            {
                profitNow = value;
            }
        }

        public static List<double?> GetPieData(List<OrderApi> Orders)
        {
            var ValidWarehouseOuts = Orders.Where(s => s.Status.StatusName != "Отменен"
            && s.ActionType.ActionTypeName == "Продажа"
            && s.DateOfOrder?.Year == DateNow.Year
            && s.DateOfOrder?.Month == DateNow.Month).SelectMany(w => w.WareHouses).ToList();

            var groupedSales = ValidWarehouseOuts.GroupBy(sale => sale.SaleCar.CarId)
                              .Select(group => new { CarId = group.Key, TotalQuantity = group.Sum(sale => sale.CountChange * -1) })
                              .OrderByDescending(group => group.TotalQuantity)
                              .Take(5);

            List<double?> Counts = new List<double?>();

            foreach (var item in groupedSales)
            {
                Counts.Add((double)item.TotalQuantity);
            }

            return Counts;
        }

        public static List<string?> GetPieDataLabels(List<OrderApi> Orders, List<CarApi> Cars)
        {
            var ValidWarehouseOuts = Orders.Where(s => s.Status.StatusName != "Отменен"
            && s.ActionType.ActionTypeName == "Продажа"
            && s.DateOfOrder?.Year == DateNow.Year
            && s.DateOfOrder?.Month == DateNow.Month).SelectMany(w => w.WareHouses).ToList();

            var groupedSales = ValidWarehouseOuts.GroupBy(sale => sale.SaleCar.CarId)
                              .Select(group => new { CarId = group.Key, TotalQuantity = group.Sum(sale => sale.CountChange * -1) })
                              .OrderByDescending(group => group.TotalQuantity)
                              .Take(5);

            List<string?> Labels = new List<string?>();

            foreach (var item in groupedSales)
            {
                Labels.Add(Cars.FirstOrDefault(s => s.ID == item.CarId).CarName);
            }

            return Labels;
        }

        private static async Task<List<OrderApi>> GetOrder()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            return Orders;
        }

        private static async Task<List<WareHouseApi>> GetWareHouse()
        {
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            return Warehouses;
        }

        public static List<double?> GetProfitChartData()
        {
            List<double?> datas = new List<double?>();
            List<OrderApi> ordersData = GetOrder().Result;
            List<WareHouseApi> wareHouseData = GetWareHouse().Result;

            for (int i = 1; i <= 12; i++)
            {
                double profit = new();// прибыль(убыль) за месяц
                foreach (OrderApi item in ordersData.Where(s=> s.ActionTypeId == 3 && s.StatusId == 2))// заходим в каждый заказ
                {
                    DateTime? date = item.DateOfOrder;// смотрим на дату заказа 

                    if(date.Value.Month == i)// сверяем его с датой нужного нам месяца
                    {// смотрим все сделки по этому заказу
                        var warehouses = wareHouseData.Where(s => s.OrderId == item.ID).ToList();// берем все warehouse, которые связаны с этим заказом
                        foreach (var item1 in warehouses)// проходим каждый warehouse
                        {
                            profit += (double)item1.Price;// считаем profit и приводим к double, ибо в диаграммах принимается только он
                        }
                    }
                }
                datas.Add(profit);// мы прошли месяц и добавляем profit в лист с данными по этому месяцу
            }

            
            return datas;
        }


        public static List<double?> GetProfitCompareMonth(DateTime dateCompare)
        {
            List<double?> datas = new List<double?>();
            double profitNow = new();
            double profitCompare = new();
            List<OrderApi> ordersData = GetOrder().Result;
            List<WareHouseApi> wareHouseData = GetWareHouse().Result;

            foreach (var item in ordersData.Where(s => s.ActionTypeId == 3 && s.StatusId == 2 && s.DateOfOrder.Value.Month == DateNow.Month && s.DateOfOrder.Value.Year == DateNow.Year))
            {
                var warehouses = wareHouseData.Where(s => s.OrderId == item.ID).ToList();
                foreach (var item1 in warehouses)
                {
                    profitNow += (double)item1.Price;
                }
            }
            datas.Add(profitNow);

            foreach (var item in ordersData.Where(s => s.ActionTypeId == 3 && s.StatusId == 2 && s.DateOfOrder.Value.Month == dateCompare.Month && s.DateOfOrder.Value.Year == DateNow.Year))
            {
                var warehouses = wareHouseData.Where(s => s.OrderId == item.ID).ToList();
                foreach (var item1 in warehouses)
                {
                    profitCompare += (double)item1.Price;
                }
            }
            datas.Add(profitCompare);


            return datas;
        }

        public static List<double?> GetProfitCompareMonth1()// это удалю потом
        {
            List<double?> datas = new List<double?>();
            double profitNow = new();
            double profitCompare = new();
            List<OrderApi> ordersData = GetOrder().Result;
            List<WareHouseApi> wareHouseData = GetWareHouse().Result;

            foreach (var item in ordersData.Where(s => s.ActionTypeId == 3 && s.StatusId == 2 && s.DateOfOrder.Value.Month == DateNow.Month && s.DateOfOrder.Value.Year == DateNow.Year))
            {
                var warehouses = wareHouseData.Where(s => s.OrderId == item.ID).ToList();
                foreach (var item1 in warehouses)
                {
                    profitNow += (double)item1.Price;
                }
            }
            datas.Add(profitNow);

            foreach (var item in ordersData.Where(s => s.ActionTypeId == 3 && s.StatusId == 2 && s.DateOfOrder.Value.Month == dateCompare.Month && s.DateOfOrder.Value.Year == DateNow.Year))
            {
                var warehouses = wareHouseData.Where(s => s.OrderId == item.ID).ToList();
                foreach (var item1 in warehouses)
                {
                    profitCompare += (double)item1.Price;
                }
            }
            datas.Add(profitCompare);


            return datas;
        }

    }
}
