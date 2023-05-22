using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class EditOrderViewModel : BaseViewModel
    {
        public bool IsStatusEnable { get; set; } = true;
        public UserApi SelectedUser { get; set; }
        public ActionTypeApi SelectedActionType { get; set; }

        private StatusApi selectedStatus;
        public StatusApi SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;
            }
        }
        public ObservableCollection<WareHouseApi> ThisWarehouses { get; set; } = new ObservableCollection<WareHouseApi>();  
        public List<WareHouseApi> Warehouses { get; set; }
        public List<CountChangeHistoryApi> CountChangeHistories { get; set; } = new List<CountChangeHistoryApi>();
        public List<StatusApi> Statuses { get; set; }
        public List<UserApi> Users { get; set; }
        public List<OrderApi> Orders { get; set; }
        public List<ActionTypeApi> ActionTypes { get; set; }
        public OrderApi AddOrderVM { get; set; }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand PrintOrder { get; set; }

        public EditOrderViewModel(OrderApi order)
        {
            Task.Run(GetList).Wait();

            if (order == null)
            {
                AddOrderVM = new OrderApi();
            }
            else
            {
                AddOrderVM = new OrderApi
                {
                    ID = order.ID,
                    DateOfOrder = order.DateOfOrder,
                    ActionTypeId = order.ActionTypeId,
                    StatusId = order.StatusId,
                    WareHouses = order.WareHouses,
                    UserId = order.UserId,
                    ActionType = order.ActionType,
                    Status = order.Status,
                    User = order.User,
                };
                GetInfo();
                if (SelectedStatus.StatusName == "Отменен")
                    IsStatusEnable = false;
            }

            PrintOrder = new CustomCommand(() =>
            {
                GenerateReport();
            });

            Save = new CustomCommand(async () =>
            {
                 AddOrderVM.StatusId = SelectedStatus.ID;
                 AddOrderVM.Status = SelectedStatus;
                 AddOrderVM.UserId = SelectedUser.ID;
                 AddOrderVM.User = SelectedUser;
                 AddOrderVM.ActionTypeId = SelectedActionType.ID;
                 AddOrderVM.ActionType = SelectedActionType;

                 if (AddOrderVM.Status.StatusName == "Отменен" && AddOrderVM.ActionType.ActionTypeName == "Поступление")
                 {
                      foreach (var WH in ThisWarehouses)
                      {
                          var countChange = CountChangeHistories.FirstOrDefault(s => s.WarehouseInId == WH.ID);
                             if (countChange == null)
                                 continue;
                             if (Orders.First(s => s.WareHouses.Any(w => w.ID == countChange.WarehouseOutId)).Status.StatusName == "Отменен")
                                 continue;
                            UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Нельзя отменить заказ который участвовал в других заказах!" });
                            return;
                      }
                 }
              
                 AddOrderVM.WareHouses = ThisWarehouses.ToList();
             
                 if (AddOrderVM.ID == 0)
                 {
                     await CreateOrder(AddOrderVM);
                 }
                 else
                 {
                     await EditOrder(AddOrderVM);
                 }
                 UIManager.CloseWindow(this);
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }

        private void GenerateReport()
        {
            int TotalCount = 0;
            decimal TotalPurchasePrice = 0;
            decimal TotalRawSalePrice = 0;
            decimal TotalSalePrice = 0;
            decimal TotalProfit = 0;

            var workbook = new Workbook();
            var sheet = workbook.Worksheets[0];

            sheet.Range["A1"].Value = AddOrderVM.ActionType.ActionTypeName;
            sheet.Range["A1:B1"].Merge();
            sheet.Range["D1"].Value = "Дата:";
            sheet.Range["E1"].Value = ((DateTime)AddOrderVM.DateOfOrder).ToShortDateString();
            sheet.Range["D1:L1"].Style.HorizontalAlignment = HorizontalAlignType.Center;
            sheet.Range["G1"].Value = "Заказчик";
            sheet.Range["H1"].Value = AddOrderVM.User.UserName;

            System.Drawing.Color color = ColorTranslator.FromHtml(Configuration.GetConfiguration().OrderColors.First(s => s.Status == AddOrderVM.Status.StatusName.ToString()).Color);
            if (color == null)
            {
                color = System.Drawing.Color.Transparent;
            }
            sheet.Range[$"A1:L1"].Style.Color = color;

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

            foreach (WareHouseApi ware in AddOrderVM.WareHouses)
            {
                 decimal purchase = 0;
            
                 sheet.Range[$"A{index}"].Value = ware.SaleCar.Articul.ToString();
                 sheet.Range[$"B{index}"].Value = AddOrderVM.DateOfOrder.ToString().Substring(0, AddOrderVM.DateOfOrder.ToString().Length - 8);
                 sheet.Range[$"C{index}"].Value = ware.SaleCar.Car.CarName.ToString();
                 
                 if (AddOrderVM.ActionType.ActionTypeName == "Поступление")
                 {
                     sheet.Range[$"D{index}"].Value = ware.CountChange.ToString();
                     TotalCount += (int)ware.CountChange;
                 }
                 else
                 {
                     sheet.Range[$"D{index}"].Value = (ware.CountChange * -1).ToString();
                     TotalCount += (int)ware.CountChange * -1;
                 }

                if (AddOrderVM.ActionType.ActionTypeName == "Поступление")
                 {
                     sheet.Range[$"E{index}"].Value = ware.Price.ToString();
                     sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";
                     TotalPurchasePrice += (decimal)(ware.Price * ware.CountChange);
                 }
                 if (AddOrderVM.ActionType.ActionTypeName == "Продажа")
                 {
                     purchase = (decimal)Orders.SelectMany(s => s.WareHouses).First(s => s.ID == CountChangeHistories.First(s => s.WarehouseOutId == ware.ID).WarehouseInId).Price;
                     sheet.Range[$"E{index}"].Value = purchase.ToString();
                     sheet.Range[$"E{index}"].NumberFormat = "0.00 ₽";
                     TotalPurchasePrice += (decimal)(purchase * ware.CountChange * -1);
                 }
            
                 sheet.Range[$"F{index}"].Value = ware.SaleCar.ColorCar.ToString();
                 sheet.Range[$"G{index}"].Value = ware.SaleCar.Equipment.NameEquipment.ToString();
                 sheet.Range[$"H{index}"].Value = ware.SaleCar.EquipmentPrice.ToString();
                 sheet.Range[$"H{index}"].NumberFormat = "0.00 ₽";

                if (AddOrderVM.ActionType.ActionTypeName == "Поступление")
                 {
                     sheet.Range[$"I{index}:L{index}"].Value = "-----";
                 }
                 if (AddOrderVM.ActionType.ActionTypeName == "Продажа")
                 {
                     sheet.Range[$"I{index}"].Value = ware.Price.ToString();
                     sheet.Range[$"I{index}"].NumberFormat = "0.00 ₽";
                     TotalRawSalePrice += (decimal)(ware.Price * ware.CountChange * -1);
            
                     sheet.Range[$"J{index}"].Value = ware.Discount.ToString();
                     sheet.Range[$"J{index}"].NumberFormat = "0.00 ₽";
            
                     sheet.Range[$"K{index}"].Value = ((ware.Price - ware.Discount) * ware.CountChange * -1).ToString();
                     sheet.Range[$"K{index}"].NumberFormat = "0.00 ₽";
                     TotalSalePrice += (decimal)((ware.Price - ware.Discount) * ware.CountChange * -1);
            
                     sheet.Range[$"L{index}"].Value = ((ware.Price - ware.Discount - purchase) * ware.CountChange).ToString();
                     sheet.Range[$"L{index}"].NumberFormat = "0.00 ₽";
                     TotalProfit += (decimal)((ware.Price - ware.Discount - purchase) * ware.CountChange * -1);
                 }
                 if (AddOrderVM.ActionType.ActionTypeName == "Списание")
                 {
                     sheet.Range[$"I{index}:L{index}"].Value = "-----";
                 }
                 index++;
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
                workbook.SaveToFile("text1.xls");
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "/text1.xls")
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

        private void GetInfo()
        {
            ThisWarehouses = new ObservableCollection<WareHouseApi>(AddOrderVM.WareHouses);
            SelectedUser = Users.FirstOrDefault(s => s.ID == AddOrderVM.UserId);
            SelectedStatus = Statuses.FirstOrDefault(s => s.ID == AddOrderVM.StatusId);
            SelectedActionType = ActionTypes.FirstOrDefault(s => s.ID == AddOrderVM.ActionTypeId);
        }
        private async Task GetList()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Warehouses = Orders.SelectMany(o => o.WareHouses).ToList();
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            CountChangeHistories = await Api.GetListAsync<List<CountChangeHistoryApi>>("CountChangeHistory");
        }
        public async Task CreateOrder(OrderApi orderApi)
        {
            var order = await Api.PostAsync<OrderApi>(orderApi, "Order");
        }

        public async Task EditOrder(OrderApi orderApi)
        {
            var order = await Api.PutAsync<OrderApi>(orderApi, "Order");
        }
    }
}
