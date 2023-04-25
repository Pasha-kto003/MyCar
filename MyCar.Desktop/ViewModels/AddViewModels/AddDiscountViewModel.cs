using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddDiscountViewModel : BaseViewModel
    {
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
                SignalChanged(nameof(selectedCar));
            }
        }

        public DiscountApi AddDiscountVM { get; set; }

        public AddDiscountViewModel(DiscountApi discount)
        {
            Task.Run(GetCars).Wait();
            if (discount == null)
            {
                AddDiscountVM = new DiscountApi { DiscountValue = 0, StartDate = DateTime.Now, EndDate = DateTime.Now, Price = 0 };
            }
            else
            {
                AddDiscountVM = new DiscountApi
                {
                    ID = discount.ID,
                    DiscountValue = discount.DiscountValue,
                    EndDate = discount.EndDate,
                    StartDate = discount.StartDate,
                    Price = discount.Price,
                    SaleCarId = discount.SaleCarId
                };

                SelectedCar = DiscountCars.FirstOrDefault(s => s.ID == AddDiscountVM.SaleCarId);
            }

            SaveDiscount = new CustomCommand(async () =>
            {
                AddDiscountVM.SaleCarId = SelectedCar.ID;
                AddDiscountVM.SaleCar = SelectedCar;

                if(AddDiscountVM.DiscountValue == 0 || AddDiscountVM.DiscountValue == null)
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
