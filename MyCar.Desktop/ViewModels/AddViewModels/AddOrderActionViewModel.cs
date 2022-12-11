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

        public AddOrderActionViewModel(WareHouseApi wareHouse, ActionTypeApi actionType)
        {
            CarTitle = wareHouse.SaleCar.Car.CarName + $" ({wareHouse.SaleCar.Equipment.NameEquipment})";
            if (actionType.ActionTypeName == "Продажа")
            {
                DiscountVisibility = Visibility.Visible;
            }
            if (actionType.ActionTypeName == "Списание")
            {
                PriceVisibility = Visibility.Collapsed;
            }
            Save = new CustomCommand(() =>
            {
                wareHouse.Price = Price;
                if (actionType.ActionTypeName != "Поступление" & Count > wareHouse.SaleCar.Count)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = $"Превышено количество на складе ({wareHouse.SaleCar.Count})!" });
                    return;
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

        private void TotalCalculate()
        {
            Total = Count * (Price - Discount);
            SignalChanged(nameof(Total));
        }
    }
}
