using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class ReportViewModel
    {
        public DateTime DateStart { get; set; } = DateTime.Now;
        public DateTime DateFinish { get; set; } = DateTime.Now;

        public string Articul { get; set; } = "";

        public bool IsOrderIns { get; set; } = false;
        public bool IsOrderOuts { get; set; } = true;
        public bool IsOrderOffs { get; set; } = false;

        public bool IsCanceled { get; set; } = false;
        public bool IsNew { get; set; } = true;
        public bool IsFinished { get; set; } = true;


        public CustomCommand MakeReport { get; set; }

        private List<OrderApi> ValidOrders { get; set; } = new List<OrderApi>();

        public List<CountChangeHistoryApi> CountChangeHistories = new List<CountChangeHistoryApi>();

        public List<OrderApi> Orders = new List<OrderApi>();
        public ReportViewModel()
        {
            Task.Run(GetList).Wait();

            MakeReport = new CustomCommand(() =>
            {
                bool isConvert = Int32.TryParse(Articul, out int res);

                if (Articul.Length != 0 && !isConvert)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Неверный артикул" });
                    return;
                }

                if (Articul.Length != 0 && !Orders.SelectMany(s => s.WareHouses).Select(r => r.SaleCar).ToList().Any(d=>d.Articul == Articul))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Авто с таким артикулом не существует!" });
                    return;
                }

                if (IsCanceled == false && IsFinished == false && IsNew == false)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран ни один тип заказов" });
                    return;
                }

                PrepareReport();
            });

        }

        private void PrepareReport()
        {
            Task.Run(GetList).Wait();
            ValidOrders.Clear();
            ValidOrders = new List<OrderApi>(Orders.Where(s => s.DateOfOrder >= DateStart && s.DateOfOrder <= DateFinish));

            if (!IsCanceled)
            {
                ValidOrders.RemoveAll(s => s.Status.StatusName == "Отменен");
            }
            if (!IsNew)
            {
                ValidOrders.RemoveAll(s => s.Status.StatusName == "Новый");
            }
            if (!IsFinished)
            {
                ValidOrders.RemoveAll(s => s.Status.StatusName == "Завершен");
            }

            if (IsOrderIns)
            {
                ValidOrders.RemoveAll(s => s.ActionType.ActionTypeName != "Поступление");
            }
            else if (IsOrderOuts)
            {
                ValidOrders.RemoveAll(s => s.ActionType.ActionTypeName != "Продажа");
            }
            else if (IsOrderOffs)
            {
                ValidOrders.RemoveAll(s => s.ActionType.ActionTypeName != "Списание");
            }

            List<OrderApi> FinalList = new List<OrderApi>();

            if (Articul.Length != 0)
                FinalList = FilterByArticul(ValidOrders, Articul);
            else
                FinalList = new List<OrderApi>(ValidOrders);

            if (!IsOrderIns)
            {
                foreach (WareHouseApi ware in FinalList.SelectMany(s => s.WareHouses).ToList())
                {
                    ware.CountChange *= -1;
                }
            }

            GenerateReport(FinalList);
        }

        private void GenerateReport(List<OrderApi> Orders)
        {
            var TotalCount = 0;
            decimal TotalPurchasePrice = 0;
            decimal TotalRawSalePrice = 0;
            decimal TotalSalePrice = 0;
            decimal TotalProfit = 0;

            var workbook = new Workbook();
            var sheet = workbook.Worksheets[0];

            if (IsOrderIns)
                sheet.Range["A1"].Value = "ПОСТУПЛЕНИЯ";
            if (IsOrderOuts)
                sheet.Range["A1"].Value = "ПРОДАЖИ";
            if (IsOrderOffs)
                sheet.Range["A1"].Value = "СПИСАНИЯ";

            sheet.Range["A1:B1"].Merge();
            sheet.Range["D1"].Value = "С";
            sheet.Range["E1"].Value = DateStart.ToShortDateString();
            sheet.Range["F1"].Value = "ПО";
            sheet.Range["G1"].Value = DateFinish.ToShortDateString();
            sheet.Range["D1:L1"].Style.HorizontalAlignment = HorizontalAlignType.Center;

            sheet.Range["A3"].Value = "Артикул";
            sheet.Range["B3"].Value = "Дата";
            sheet.Range["C3"].Value = "Наименование";
            sheet.Range["D3"].Value = "Кол-во";
            sheet.Range["E3"].Value = "Закупочная цена";
            sheet.Range["F3"].Value = "Цвет";
            sheet.Range["G3"].Value = "Комплектация";
            sheet.Range["H3"].Value = "Цена комплектации";
            sheet.Range["I3"].Value = "Цена продажи";
            sheet.Range["J3"].Value = "Cкидка";
            sheet.Range["K3"].Value = "Сумма со скидкой";
            sheet.Range["L3"].Value = "Прибыль";

            var index = 4;

            foreach (OrderApi order in Orders)
            {
                System.Drawing.Color color = ColorTranslator.FromHtml(Configuration.GetConfiguration().OrderColors.First(s => s.Status == order.Status.StatusName.ToString()).Color);
                if (color == null)
                {
                    color = System.Drawing.Color.Transparent;
                }
                sheet.Range[$"A{index}:L{index}"].Style.Color = color;

                foreach (WareHouseApi ware in order.WareHouses)
                {
                    decimal purchase = 0;
                   
                    sheet.Range[$"A{index}"].Value = ware.SaleCar.Articul.ToString();
                    sheet.Range[$"B{index}"].Value = order.DateOfOrder.ToString().Substring(0, order.DateOfOrder.ToString().Length - 8);
                    sheet.Range[$"C{index}"].Value = ware.SaleCar.Car.CarName.ToString();

                    sheet.Range[$"D{index}"].Value = ware.CountChange.ToString();
                    TotalCount += (int)ware.CountChange;

                    if (IsOrderIns)
                    {
                        sheet.Range[$"E{index}"].Value = ware.Price.ToString();
                        sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";
                        TotalPurchasePrice += (decimal)(ware.Price * ware.CountChange);
                    }
                    if (IsOrderOuts)
                    {
                        purchase = (decimal)this.Orders.SelectMany(s => s.WareHouses).First(s => s.ID == CountChangeHistories.First(s => s.WarehouseOutId == ware.ID).WarehouseInId).Price;
                        sheet.Range[$"E{index}"].Value = purchase.ToString();
                        sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";
                        TotalPurchasePrice += (decimal)(purchase * ware.CountChange);
                    }

                    sheet.Range[$"F{index}"].Value = ware.SaleCar.ColorCar.ToString();
                    sheet.Range[$"G{index}"].Value = ware.SaleCar.Equipment.NameEquipment.ToString();
                    sheet.Range[$"H{index}"].Value = ware.SaleCar.EquipmentPrice.ToString();
                    sheet.Range[$"H{index}"].NumberFormat = "0.00 ₽";

                    if (IsOrderIns)
                    {
                        sheet.Range[$"I{index}:L{index}"].Value = "-----";
                    }
                    if (IsOrderOuts)
                    {
                        sheet.Range[$"I{index}"].Value = ware.Price.ToString();
                        sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";
                        TotalRawSalePrice += (decimal)(ware.Price * ware.CountChange);

                        sheet.Range[$"J{index}"].Value = ware.Discount.ToString();
                        sheet.Range[$"J{index}"].NumberFormat = "0.00 ₽";


                        sheet.Range[$"K{index}"].Value = ((ware.Price - ware.Discount) * ware.CountChange).ToString();
                        sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";
                        TotalSalePrice += (decimal)((ware.Price - ware.Discount) * ware.CountChange);


                        sheet.Range[$"L{index}"].Value = ((ware.Price - ware.Discount - purchase) * ware.CountChange).ToString();
                        sheet.Range[$"L{index}"].NumberFormat = "0.00 ₽";
                        TotalProfit += (decimal)((ware.Price - ware.Discount - purchase) * ware.CountChange);
                    }
                    if (IsOrderOffs)
                    {
                        sheet.Range[$"I{index}:L{index}"].Value = "-----";
                    }
                    index++;
                }
            }

            sheet.Range[$"A{index}"].Value = "Всего";
            sheet.Range[$"A{index}:C{index}"].Merge();
            sheet.Range[$"D{index}"].Value = TotalCount.ToString();

            sheet.Range[$"E{index}"].Value = TotalPurchasePrice.ToString();
            sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";

            sheet.Range[$"F{index}:H{index}"].Style.Color = System.Drawing.Color.Gray;

            sheet.Range[$"I{index}"].Value = TotalRawSalePrice.ToString();
            sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";

            sheet.Range[$"J{index}"].Style.Color = System.Drawing.Color.Gray;

            sheet.Range[$"K{index}"].Value = TotalSalePrice.ToString();
            sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";

            sheet.Range[$"L{index}"].Value = TotalProfit.ToString();
            sheet.Range[$"L{index}"].NumberFormat = "0.00 ₽";

            sheet.Range[$"A3:L{index}"].BorderInside(LineStyleType.Thin);
            sheet.Range[$"A3:L{index}"].BorderAround(LineStyleType.Medium);

            sheet.AllocatedRange.AutoFitColumns();

            try
            {
                workbook.SaveToFile("text.xls");
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "/text.xls")
                {
                    UseShellExecute = true
                };
                p.Start();
            }
            catch (Exception ex)
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = ex.Message });
                return;
            }
        }

        private List<OrderApi> FilterByArticul(List<OrderApi> orders, string articul)
        {
            List<OrderApi> finalList = new List<OrderApi>();

            foreach (OrderApi order in orders)
            {
                List<WareHouseApi> filteredWares = new List<WareHouseApi>();

                foreach (WareHouseApi wareHouse in order.WareHouses)
                {
                    if (wareHouse.SaleCar.Articul == articul)
                    {
                        filteredWares.Add(wareHouse);
                    }
                }

                if (filteredWares.Count > 0)
                {
                    order.WareHouses = filteredWares;
                    finalList.Add(order);
                }
            }

            return finalList;
        }
        private async Task GetList()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            CountChangeHistories = await Api.GetListAsync<List<CountChangeHistoryApi>>("CountChangeHistory");
        }


    }
}
