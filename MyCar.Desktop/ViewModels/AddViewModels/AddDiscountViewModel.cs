using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddDiscountViewModel : BaseViewModel
    {
        public decimal FinalPrice { get; set; }


        private decimal discountValue;
        public decimal DiscountValue
        {
            get => discountValue;
            set
            {
                discountValue = value;
                CalculatePercentValue();
                SignalChanged(nameof(discountValue));
            }
        }

        private decimal percentValue;
        public decimal PercentValue
        {
            get => percentValue;
            set
            {
                percentValue = value;
                CalculateDiscountValue();
                SignalChanged(nameof(percentValue));
            }
        }
        public CustomCommand SaveDiscount { get; set; }
        public CustomCommand Cancel { get; set; }

        public List<SaleCarApi> DiscountCars { get; set; }

        private SaleCarApi selectedCar { get;set; }
        public SaleCarApi SelectedCar
        {
            get => selectedCar;
            set
            {
                selectedCar = value;
                CalculatePercentValue();
                UpdateFullPrice();
                SignalChanged(nameof(selectedCar));
            }
        }

        public CustomCommand ChooseCars { get; set; }
        public DiscountApi AddDiscountVM { get; set; }

        public AddDiscountViewModel(DiscountApi discount)
        {
            Task.Run(GetCars).Wait();
            if (discount == null)
            {
                AddDiscountVM = new DiscountApi { DiscountValue = 0, StartDate = DateTime.Now, EndDate = DateTime.Now, };
            }
            else
            {
                AddDiscountVM = new DiscountApi
                {
                    ID = discount.ID,
                    DiscountValue = discount.DiscountValue,
                    EndDate = discount.EndDate,
                    StartDate = discount.StartDate,
                    SaleCarId = discount.SaleCarId
                };
                SelectedCar = DiscountCars.FirstOrDefault(s => s.ID == AddDiscountVM.SaleCarId);
                DiscountValue = AddDiscountVM.DiscountValue ?? 0;
            }
            ChooseCars = new CustomCommand(() =>
            {
                ChooseSaleCarWindow chooseSaleCarWindow = new ChooseSaleCarWindow();
                chooseSaleCarWindow.ShowDialog();
            });
            SaveDiscount = new CustomCommand(async () =>
            {
                AddDiscountVM.SaleCarId = SelectedCar.ID;
                AddDiscountVM.SaleCar = SelectedCar;
                AddDiscountVM.DiscountValue = DiscountValue;
                //AddDiscountVM.PercentValue = PercentValue;
                if (AddDiscountVM.DiscountValue == 0 || AddDiscountVM.DiscountValue == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не введено значение скидки" });
                    return;
                }
                if (AddDiscountVM.StartDate > AddDiscountVM.EndDate)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Ошибка в дате" });
                    return;
                }
                if (AddDiscountVM.StartDate == null || AddDiscountVM.EndDate == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Дата не может быть пустой" });
                    return;
                }
                if(AddDiscountVM.SaleCar == null || AddDiscountVM.SaleCar.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрано авто" });
                    return;
                }

                if (AddDiscountVM.ID == 0)
                {
                    await AddDiscount(AddDiscountVM);
                }
                else
                {
                    await EditDiscount(AddDiscountVM);
                }
                UIManager.CloseWindow(this);
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }
        
        private void CalculateDiscountValue()
        {
            DiscountValue = (PercentValue / 100) * (decimal)SelectedCar.FullPrice;
            UpdateFullPrice();
        }
        private void CalculatePercentValue()
        {
            if (SelectedCar.FullPrice != 0)
                PercentValue = Math.Round((DiscountValue / (decimal)SelectedCar.FullPrice) * 100, 2);
            else
                PercentValue = 0;
            UpdateFullPrice();
        }
        private void UpdateFullPrice()
        {
            FinalPrice = Math.Round((decimal)SelectedCar.FullPrice - DiscountValue, 2);
        }
        public async Task AddDiscount(DiscountApi discount)
        {
            var discountApi = await Api.PostAsync<DiscountApi>(discount, "Discount");
        }

        public async Task EditDiscount(DiscountApi discount)
        {
            var discountApi = await Api.PutAsync<DiscountApi>(discount, "Discount");
        }

        public async Task GetCars()
        {
            DiscountCars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
        }

    }
}
