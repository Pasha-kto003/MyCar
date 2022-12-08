using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
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
        public UserApi SelectedUser { get; set; }
        public ActionTypeApi SelectedActionType { get; set; }
        public StatusApi SelectedStatus { get; set; } 
        public ObservableCollection<WareHouseApi> ThisWarehouses { get; set; } = new ObservableCollection<WareHouseApi>();  
        public List<WareHouseApi> Warehouses { get; set; }
        public List<StatusApi> Statuses { get; set; }
        public List<UserApi> Users { get; set; }
        public List<ActionTypeApi> ActionTypes { get; set; }
        public OrderApi AddOrderVM { get; set; }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }

        public AddOrderViewModel(OrderApi order)
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
                };
                GetInfo();
            }

            Save = new CustomCommand(async() =>
            {
                AddOrderVM.StatusId = SelectedStatus.ID; 
                AddOrderVM.Status = SelectedStatus;
                AddOrderVM.UserId = SelectedUser.ID;
                AddOrderVM.User = SelectedUser;
                AddOrderVM.ActionTypeId = SelectedActionType.ID;
                AddOrderVM.ActionType = SelectedActionType;

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
        private void GetInfo()
        {
            ThisWarehouses = new ObservableCollection<WareHouseApi>(AddOrderVM.WareHouses);
            SelectedUser = Users.FirstOrDefault(s => s.ID == AddOrderVM.UserId);
            SelectedStatus = Statuses.FirstOrDefault(s => s.ID == AddOrderVM.StatusId);
            SelectedActionType = ActionTypes.FirstOrDefault(s => s.ID == AddOrderVM.ActionTypeId);
        }
        private async Task GetList()
        {
            Warehouses = await Api.GetListAsync<List<WareHouseApi>>("Warehouse");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            Statuses = await Api.GetListAsync<List<StatusApi>>("Status");
            Users = await Api.GetListAsync<List<UserApi>>("User");
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
