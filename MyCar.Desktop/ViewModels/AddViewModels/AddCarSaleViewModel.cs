using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddCarSaleViewModel : BaseViewModel
    {
        private MarkCarApi selectedMark;
        public MarkCarApi SelectedMark
        {
            get => selectedMark;
            set
            {
                selectedMark = value;
                SelectedMarkChanged(selectedMark);
            }
        }

        private List<CarApi> searchResult;

        public List<SaleCarApi> SaleCars { get; set; } = new List<SaleCarApi>();
        public List<CarApi> Cars { get; set; } = new List<CarApi>();  
        public List<CarApi> FullCars { get; set; } = new List<CarApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<MarkCarApi> Marks { get; set; } = new List<MarkCarApi>();
        public CarApi SelectedCar { get; set; }

        public EquipmentApi SelectedEquipment { get; set; }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }

        public SaleCarApi AddSaleVM { get; set; }

        public AddCarSaleViewModel(SaleCarApi saleCar)
        {
            Task.Run(GetList).Wait();
            if (saleCar == null)
            {
                AddSaleVM = new SaleCarApi { };
            }
            else
            {
                AddSaleVM = new SaleCarApi
                {
                    ID = saleCar.ID,
                    Articul = saleCar.Articul,
                    CarId = saleCar.CarId,
                    Car = saleCar.Car,
                    EquipmentId = saleCar.EquipmentId,
                    Equipment = saleCar.Equipment,
                    EquipmentPrice = saleCar.EquipmentPrice
                };
                Get();
                SaleCars.RemoveAll(s => s.ID == AddSaleVM.ID);
            }

            Save = new CustomCommand(async () =>
            {
                if (SelectedCar == null || SelectedCar.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран автомобиль!" });
                    return;
                }
                if (SelectedEquipment == null || SelectedEquipment.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана комплектация!" });
                    return;
                }

                if (!CheckUnique())
                    return;

                AddSaleVM.EquipmentId = SelectedEquipment.ID;
                AddSaleVM.Equipment = SelectedEquipment;
                AddSaleVM.CarId = SelectedCar.ID;
                AddSaleVM.Car = SelectedCar;

                if (AddSaleVM.ID == 0)
                {
                    await AddSale(AddSaleVM);
                }
                else
                {
                    await EditSale(AddSaleVM);
                }
                UIManager.CloseWindow(this);
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }

        private bool CheckUnique()
        {
            if (SaleCars.Exists(s=>s.Articul == AddSaleVM.Articul))
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Артикул должен быть уникальным!" });
                return false;
            }
            if (SaleCars.Exists(s =>s.CarId == SelectedCar.ID & s.EquipmentId == SelectedEquipment.ID))
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Автомобиль с выбранной комплектацией уже существует!" });
                return false;
            }
            return true;
        }

        private async Task GetList()
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            SaleCars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            Marks.Add(new MarkCarApi { MarkName = "Все" });
            FullCars = Cars;
        }

        private void Get()
        {
            SelectedEquipment = Equipments.FirstOrDefault(s => s.ID == AddSaleVM.EquipmentId);
            SelectedCar = Cars.FirstOrDefault(s => s.ID == AddSaleVM.CarId);
            SelectedMark = Marks.FirstOrDefault(s => s.ID == AddSaleVM.Car.Model.MarkId);
        }

        private async Task EditSale(SaleCarApi saleCar)
        {
            var sale = await Api.PutAsync<SaleCarApi>(saleCar, "CarSales");
        }

        public async Task AddSale(SaleCarApi saleCar)
        {
            var sale = await Api.PostAsync<SaleCarApi>(saleCar, "CarSales");
        }
        private void SelectedMarkChanged(MarkCarApi mark)
        {
            if (mark.ID == 0)
                Cars = FullCars;
            else
            Cars = FullCars.Where(s => s.Model.MarkId == mark.ID).ToList();
        }
    }
}
