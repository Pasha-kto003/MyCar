using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ModelsApi;
using MyCar.Desktop.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private DateTime dateNow = DateTime.Now;
        public DateTime DateNow
        {
            get => dateNow;
            set
            {
                dateNow = value;
                DataSet();
                SignalChanged();
            }
        }
        private DateTime dateCompare = DateTime.Now.AddMonths(-1);
        public DateTime DateCompare
        {
            get => dateCompare;
            set
            {
                dateCompare = value;
                DataSet();
                SignalChanged();
            }
        }

        private decimal profitNow;
        public decimal ProfitNow
        {
            get => profitNow;
            set
            {
                profitNow = value;
                SignalChanged();
            }
        }
        private decimal profitCompare;
        public decimal ProfitCompare
        {
            get => profitCompare;
            set
            {
                profitCompare = value;
                SignalChanged();
            }
        }
        private string profitDifference = "0";
        public string ProfitDifference
        {
            get => profitDifference;
            set
            {
                profitDifference = value;
                SignalChanged();
            }
        }
        private decimal averageCheckNow;
        public decimal AverageCheckNow
        {
            get => averageCheckNow;
            set
            {
                averageCheckNow = value;
                SignalChanged();
            }
        }
        private decimal averageCheckCompare;
        public decimal AverageCheckCompare
        {
            get => averageCheckCompare;
            set
            {
                averageCheckCompare = value;
                SignalChanged();
            }
        }
        private string averageCheckDifference = "0";
        public string AverageCheckDifference
        {
            get => averageCheckDifference;
            set
            {
                averageCheckDifference = value;
                SignalChanged();
            }
        }
        public string ProfitColor { get; set; } = "#000000";
        public string AverageCheckColor { get; set; } = "#000000";
        public string OrdersCountColor { get; set; } = "#000000";
        public string OrdersCountDifference { get; set; }
        public int OrdersCountNow { get; set; }
        public int OrdersCountCompare { get; set; }

        List<StatusApi> Statuses = new List<StatusApi>();
        List<OrderApi> Orders = new List<OrderApi>();
        List<ActionTypeApi> ActionTypes = new List<ActionTypeApi>();
        List<WareHouseApi> Warehouses = new List<WareHouseApi>();
        List<CountChangeHistoryApi> CountChangeHistories= new List<CountChangeHistoryApi>();

        public DashboardViewModel()
        {
            Task.Run(GetList).Wait();
        }

        private async Task GetList()
        {
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Warehouses = Orders.Where(s => s.Status.StatusName != "Отменен").SelectMany(w => w.WareHouses).ToList();
            CountChangeHistories = await Api.GetListAsync<List<CountChangeHistoryApi>>("CountChangeHistory");
            DataSet();
        }

        private void DataSet()
        {
            List<OrderApi> FullOrdersOut = new List<OrderApi>(Orders.Where(s=>s.ActionType.ActionTypeName == "Продажа"));
            List<OrderApi> OrdersOutNow = new List<OrderApi>();
            List<OrderApi> OrdersOutCompare = new List<OrderApi>();
            
            foreach (var orderOut in FullOrdersOut)
            {
                if (orderOut.Status.StatusName == "Отменен")
                    continue;

                var date = (DateTime)orderOut.DateOfOrder;
                 
                if (date.Year == DateNow.Year && date.Month == DateNow.Month)
                {
                      OrdersOutNow.Add(orderOut);
                }
                if (date.Year == DateCompare.Year && date.Month == DateCompare.Month)
                {
                      OrdersOutCompare.Add(orderOut);
                }
            }
            Calculate(OrdersOutNow, OrdersOutCompare);
        }
        private void Calculate(List<OrderApi> orderOutsNow, List<OrderApi> orderOutsCompare)
        {
            ProfitNow = ProfitCalc(orderOutsNow);
            ProfitCompare = ProfitCalc(orderOutsCompare);

            if (ProfitCompare != 0)
            {
                var a = ProfitNow / (ProfitCompare / 100M);
                a -= 100;
                a = Math.Round(a, 2);
                if (a > 0)
                    ProfitColor = "Green";
                else
                    ProfitColor = "Red";
                ProfitDifference = a.ToString() + "%";
            }
            else
            {
                ProfitDifference = "0%";
                ProfitColor = "#000000";
            }

            OrdersCountNow = orderOutsNow.Count;
            OrdersCountCompare = orderOutsCompare.Count;
            if (OrdersCountCompare != 0)
            {
                var a = OrdersCountNow / (OrdersCountCompare / 100M);
                a -= 100;
                a = Math.Round(a, 2);
                if (a > 0)
                    OrdersCountColor = "Green";
                else
                    OrdersCountColor = "Red";
                OrdersCountDifference = a.ToString() + "%";
            }
            else
            {
                OrdersCountDifference = "0%";
                OrdersCountColor = "#000000";
            }

            if (orderOutsNow.Count != 0)
            {
                var res = ProfitNow / orderOutsNow.Count;
                AverageCheckNow = Math.Round(res, 4);
            }
            else
                AverageCheckNow = 0;

            if (orderOutsCompare.Count != 0)
            {
                var res = ProfitCompare / orderOutsCompare.Count;
                AverageCheckCompare = Math.Round(res, 4);
            }
            else
                AverageCheckCompare = 0;
            if (AverageCheckCompare != 0)
            {
                var b = AverageCheckNow / (AverageCheckCompare / 100M);
                b -= 100;
                b = Math.Round(b, 2);
                if (b > 0)
                    AverageCheckColor = "Green";
                else
                    AverageCheckColor = "Red";
                AverageCheckDifference = b.ToString() + "%";
            }
            else
            {
                AverageCheckDifference = "0%";
                AverageCheckColor = "#000000";
            }

        }

        private decimal ProfitCalc(List<OrderApi> orderlList)
        {
            decimal profit = 0;
            foreach (OrderApi orderOut in orderlList)
            {
                foreach (WareHouseApi warehouseOut in orderOut.WareHouses)
                {
                    CountChangeHistoryApi countHistory = CountChangeHistories.First(s => s.WarehouseOutId == warehouseOut.ID);
                    WareHouseApi warehouseIn = Warehouses.First(w => w.ID == countHistory.WarehouseInId);

                    profit += (decimal)((decimal)(warehouseOut.Price - warehouseOut.Discount - (decimal)(warehouseIn.Price - warehouseIn.Discount)) * countHistory.Count);
                }
            }
            return profit;
        }

    }
}
