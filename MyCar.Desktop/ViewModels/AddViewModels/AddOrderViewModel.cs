using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows.OrderActions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddOrderViewModel : BaseViewModel
    {
        public decimal? Total { get; set; } = 0;
        public bool IsActionTypeEnabled { get; set; } = true;

        private EquipmentApi selectedEquipmentfilter;
        public EquipmentApi SelectedEquipmentFilter
        {
            get => selectedEquipmentfilter;
            set
            {
                selectedEquipmentfilter = value;
                Task.Run(Search);
            }
        }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Task.Run(Search);
            }
        }

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Task.Run(Search);
            }
        }
        private List<SaleCarApi> searchResult;
        public SaleCarApi SelectedSaleCar { get; set; }
        public ActionTypeApi SelectedActionType { get; set; }
        public UserApi SelectedUser { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<ActionTypeApi> ActionTypes { get; set; }
        public ObservableCollection<SaleCarApi> SaleCars { get; set; }
        public List<EquipmentApi> Equipments { get; set; }
        public List<EquipmentApi> EquipmentFilter { get; set; }
        public ObservableCollection<WareHouseApi> Warehouses { get; set; } = new ObservableCollection<WareHouseApi>();
        public WareHouseApi SelectedWarehouse { get; set; }
        public List<UserApi> Users { get; set; }
        public List<StatusApi> Statuses { get; set; }

        public List<CountChangeHistoryApi> CountChangeHistories { get; set; } = new List<CountChangeHistoryApi>();

        public CustomCommand ConfirmOrder { get; set; }
        public CustomCommand DeleteWarehouse { get; set; }
        public CustomCommand AddSaleCar { get; set; }

        public AddOrderViewModel()
        {
            Task.Run(GetList).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Артикул", "Авто" });
            SelectedSearchType = SearchType.First();

            EquipmentFilter = Equipments;
            EquipmentFilter.Add(new EquipmentApi { NameEquipment = "Все" });
            SelectedEquipmentFilter = EquipmentFilter.Last();


            AddSaleCar = new CustomCommand(() =>
            {
                if (!IsValidate())
                    return;
                WareHouseApi wareHouse = new WareHouseApi { SaleCar = SelectedSaleCar, SaleCarId = SelectedSaleCar.ID, CountChange = 0};
                AddOrderActionWindow addOrderIn = new AddOrderActionWindow(wareHouse, SelectedActionType, CountChangeHistories);//передаем истории в окно добавления
                addOrderIn.ShowDialog();
                if (wareHouse.CountChange != 0)
                    Warehouses.Add(wareHouse);
                Update();
            });

            ConfirmOrder = new CustomCommand(async () =>
            {
                if (SelectedActionType == null || SelectedUser == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Заполнены не все данные!" });
                    return;
                }
                if (Warehouses.Count == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Заказ пуст!" });
                    return;
                }
                if (SelectedActionType.ActionTypeName != "Поступление")
                {
                    foreach (var item in Warehouses)
                    {  
                        item.CountChange *= -1;
                        //назначаем истории связанные с выбранным WH
                        item.CountChangeHistories = CountChangeHistories.Where(s => s.WarehouseIn.SaleCarId == item.SaleCarId).ToList();
                    }
                }

                OrderApi order = new OrderApi();
                order.DateOfOrder = OrderDate;
                order.UserId = SelectedUser.ID;
                order.User = SelectedUser;
                order.ActionType = SelectedActionType;
                order.ActionTypeId = SelectedActionType.ID;
                order.Status = Statuses.FirstOrDefault(s => s.StatusName == "Новый");
                order.StatusId = Statuses.FirstOrDefault(s => s.StatusName == "Новый").ID;
                order.WareHouses = Warehouses.ToList();
                order.SumOrder = order.WareHouses.Sum(s => s.Price);
                await CreateOrder(order);
            });

            DeleteWarehouse = new CustomCommand(() =>
            {
                if (SelectedWarehouse == null) return;
                //убераем истории связанные с выбранным WH
                if (SelectedActionType.ActionTypeName != "Поступление")
                    CountChangeHistories.RemoveAll(s => s.WarehouseIn.SaleCarId == SelectedWarehouse.SaleCarId);
                Warehouses.Remove(SelectedWarehouse);
                Update();
            });
        }

        private void ClearPage(int res)
        {
            if (res != -1)
                UIManager.ShowMessage(new MessageBoxDialogViewModel { Message = "Заказ оформлен" });
            Task.Run(GetList).Wait();
            Warehouses.Clear();
            CountChangeHistories.Clear();
            OrderDate = DateTime.Now;
            Update();
        }

        private bool IsValidate()
        {
            if (SelectedActionType == null)
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран тип операции!" });
                return false;
            }
            if (SelectedSaleCar == null || SelectedSaleCar.ID == 0) return false;
            foreach (var item in Warehouses)
            {
                if (SelectedSaleCar.ID == item.SaleCarId)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Заказ уже содержит выбранную позицию!" });
                    return false;
                }
            }
            return true;
        }
        public void UpdateList()
        {
            SaleCars = new ObservableCollection<SaleCarApi>(searchResult);
            SignalChanged(nameof(SaleCars));
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<SaleCarApi>>(SelectedSearchType, "$", "CarSales", SelectedEquipmentFilter.NameEquipment);
            else
                searchResult = await Api.SearchFilterAsync<List<SaleCarApi>>(SelectedSearchType, search, "CarSales", SelectedEquipmentFilter.NameEquipment);
            UpdateList();
        }
        private void Update()
        {
            if (Warehouses.Count != 0)
                IsActionTypeEnabled = false;
            else
                IsActionTypeEnabled = true;
            foreach (var item in Warehouses)
            {
                Total += item.CountChange * (item.Price - item.Discount);
            }
        }
        public async Task CreateOrder(OrderApi orderApi)
        {
            var order = await Api.PostAsync<OrderApi>(orderApi, "Order");
            ClearPage(order);
        }

        private async Task CreateCountChange(CountChangeHistoryApi countChange)
        {
            var count = await Api.PostAsync<CountChangeHistoryApi>(countChange, "CountChangeHistory");
        }

        private async Task EditCountChange(CountChangeHistoryApi countChange)
        {
            var count = await Api.PutAsync<CountChangeHistoryApi>(countChange, "CountChangeHistory");
        }

        private async Task GetList()
        {
            var sales = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            SaleCars = new ObservableCollection<SaleCarApi>(sales);
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            Users = await Api.GetListAsync<List<UserApi>>("User");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
        }
    }
}
