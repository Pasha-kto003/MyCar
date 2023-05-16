using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddOrderActionViewModel : BaseViewModel
    {
        public string LastPriceInfo { get; set; }

        List<DiscountApi> Discounts = new List<DiscountApi>();

        List<OrderApi> Orders = new List<OrderApi>();
        public Visibility DiscountVisibility { get; set; } = Visibility.Collapsed;

        public Visibility PriceVisibility { get; set; } = Visibility.Visible;
        public string CarTitle { get; set; }

        private int count = 1;
        public int Count
        {
            get => count;
            set
            {
                if (value != count)
                {
                    count = value;
                    SignalChanged(nameof(Count));
                    TotalCalculate();
                }
            }
        }
        private decimal? price = 0;
        public decimal? Price
        {
            get => price;
            set
            {
                if (value != price)
                {
                    price = value;
                    SignalChanged(nameof(Count));
                    TotalCalculate();
                }

            }
        }
        private decimal? discount = 0;
        public decimal? Discount
        {
            get => discount;
            set
            {
                if (value != discount)
                {
                    discount = value;
                    SignalChanged(nameof(Discount));
                    TotalCalculate();
                }

            }
        }
        public decimal? Total { get; set; } = 0;
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand AddOne { get; set; }

        public AddOrderActionViewModel(WareHouseApi wareHouse, ActionTypeApi actionType, List<CountChangeHistoryApi> countChangeHistories)
        {
            Task.Run(GetList).Wait();

            CarTitle = wareHouse.SaleCar.Car.CarName + $" ({wareHouse.SaleCar.Equipment.NameEquipment})";
            if (actionType.ActionTypeName == "Продажа")
            {
                DiscountVisibility = Visibility.Visible;
                Price = wareHouse.SaleCar.FullPrice;
                var dis = Discounts.FirstOrDefault(d => d.SaleCarId == wareHouse.SaleCarId && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)?.DiscountValue;
                Discount = dis ?? 0;
            }
            if (actionType.ActionTypeName == "Поступление")
            {
                List<WareHouseApi> thisWareIns = Orders.OrderByDescending(s=>s.DateOfOrder).Where(o => o.Status.StatusName != "Отменен" && o.ActionType.ActionTypeName == "Поступление")
                    .SelectMany(w => w.WareHouses)
                    .Where(s=>s.SaleCarId == wareHouse.SaleCarId).ToList();
                if (thisWareIns.Count == 0)
                    LastPriceInfo = $"Последняя цена поступления: (Нет данных) ";
                else
                    LastPriceInfo = $"Последняя цена поступления: {thisWareIns.First().Price} ";


            }
            if (actionType.ActionTypeName == "Списание")
            {
                PriceVisibility = Visibility.Collapsed;
            }
            Save = new CustomCommand(() =>
            {
                if (Count <= 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Необходимо выбрать количество!" });
                    return;
                }

                wareHouse.Price = Price;

                if (actionType.ActionTypeName != "Поступление" & Count > wareHouse.SaleCar.Count)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = $"Превышено количество на складе ({wareHouse.SaleCar.Count})!" });
                    return;
                }

                //если не поступление, значит рассчитываем из какой поставки будет продажа/списание
                if (actionType.ActionTypeName != "Поступление")
                {
                    //выбираем не отмененные заказы
                    List<OrderApi> orders = Orders.Where(o => o.Status.StatusName != "Отменен").ToList();

                    //сортируем что бы сначала стояли старые заказы (по методу FIFO(первым пришел - первым ушел) https://ru.wikipedia.org/wiki/FIFO)
                    List<OrderApi> thisOrders = orders.OrderBy(s => s.DateOfOrder).ToList();

                    //выбираем все вейрхаусы в поставках
                    List<WareHouseApi> WareHouseIns = thisOrders.Where(s => s.ActionType.ActionTypeName == "Поступление").SelectMany(w => w.WareHouses).ToList();

                    //выбираем нужные вейрхаусы которые еще не закончились
                    List<WareHouseApi> ThisWareHouseIns = WareHouseIns.Where(
                       s => s.SaleCarId == wareHouse.SaleCar.ID &&
                       s.CountRemains > 0).ToList();

                    //переводим в массив
                    ThisWareHouseIns.ToArray();

                    //запоминаем количество которое надо забрать (countRemains - количество которое нам нужно)
                    int countRemains = Count;

                    //заходим в цикл (пока не возьмем количество которое нам нужно)
                    for (int i = 0; countRemains > 0; i++)
                    {

                        //запоминаем количество до вычитания
                        int countRemainsBefore = countRemains;

                        //вычитаем из того сколько нам надо значение остатка первой поставки
                        countRemains -= ThisWareHouseIns[i].CountRemains;

                        //проверяем если количества в поставке хватило
                        if (countRemains <= 0)
                        {
                            //если хватило записываем countRemainsBefore (сколько надо было)
                            countChangeHistories.Add(new CountChangeHistoryApi { Count = countRemainsBefore, WarehouseInId = ThisWareHouseIns[i].ID, WarehouseIn = ThisWareHouseIns[i] });
                        }
                        else
                        {
                            //если нет то ThisWareHouseIns[i].CountRemains (сколько было в поставке)
                            countChangeHistories.Add(new CountChangeHistoryApi { Count = ThisWareHouseIns[i].CountRemains, WarehouseInId = ThisWareHouseIns[i].ID, WarehouseIn = ThisWareHouseIns[i] });
                        }
                        //повторяем
                    }
                }
                wareHouse.CountChange = Count;
                wareHouse.Discount = Discount;

                UIManager.CloseWindow(this);
            });
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
            AddOne = new CustomCommand(() =>
            {
                Count++;
            });
        }
        private async Task GetList()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            Discounts = await Api.GetListAsync<List<DiscountApi>>("Discount");
        }

        private void TotalCalculate()
        {
            Total = Count * (Price - Discount);
            SignalChanged(nameof(Total));
        }
    }
}
