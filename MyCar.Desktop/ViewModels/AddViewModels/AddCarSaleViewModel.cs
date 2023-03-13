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

        private CarPhotoApi selectedPhoto { get; set; }
        public CarPhotoApi SelectedPhoto
        {
            get=> selectedPhoto;
            set
            {
                selectedPhoto = value;
                SignalChanged();
            }
        }

        private CarPhotoApi selectedThisPhoto { get; set; }
        public CarPhotoApi SelectedThisPhoto
        {
            get => selectedThisPhoto;
            set
            {
                selectedThisPhoto = value;
                SignalChanged();
            }
        }

        private List<CarApi> searchResult;

        public List<SaleCarApi> SaleCars { get; set; } = new List<SaleCarApi>();
        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        public List<CarApi> FullCars { get; set; } = new List<CarApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<MarkCarApi> Marks { get; set; } = new List<MarkCarApi>();
        public List<CarPhotoApi> CarPhotos { get; set; } = new List<CarPhotoApi>();
        public ObservableCollection<CarPhotoApi> ThisCarPhotos { get; set; } = new ObservableCollection<CarPhotoApi>();
        public CarApi SelectedCar { get; set; }

        public EquipmentApi SelectedEquipment { get; set; }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand AddPhotoCar { get; set; }
        public CustomCommand DeletePhoto { get; set; }

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
                    EquipmentPrice = saleCar.EquipmentPrice,
                    ColorCar = saleCar.ColorCar
                };
                Get();
                SaleCars.RemoveAll(s => s.ID == AddSaleVM.ID);

                var list = CarPhotos.Where(s => s.SaleCarId == AddSaleVM.ID).ToList();
                ThisCarPhotos = new ObservableCollection<CarPhotoApi>(list);
            }

            AddPhotoCar = new CustomCommand(async () =>
            {
                if(SelectedPhoto == null || SelectedPhoto.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрано изображение автомобиля!" });
                    return;
                }
                if (CheckContains(ThisCarPhotos.ToList(), SelectedPhoto))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = $"Модель {SelectedPhoto.PhotoName} уже содержится в машине" });
                    return;
                }
                else
                {
                    ThisCarPhotos.Add(SelectedPhoto);
                    var photoCar = SelectedPhoto;
                    photoCar.SaleCarId = AddSaleVM.ID;
                    await EditPhoto(photoCar);
                }
            });

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
                AddSaleVM.CarPhotos = ThisCarPhotos.ToList();

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

        private bool CheckContains(List<CarPhotoApi> list, CarPhotoApi photo)
        {
            bool result = false;
            foreach (CarPhotoApi item in list)
            {
                if (item.ID == photo.ID)
                {
                    result = true;
                }
            }
            return result;
        }

        private async Task GetList()
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            SaleCars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            CarPhotos = await Api.GetListAsync<List<CarPhotoApi>>("CarPhoto");
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

        private async Task EditPhoto(CarPhotoApi carPhoto)
        {
            var photo = await Api.PutAsync<CarPhotoApi>(carPhoto, "CarPhoto");
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
