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
        private ObservableCollection<CarApi> searchResult;

        public ObservableCollection<CarApi> Cars { get; set; } = new ObservableCollection<CarApi>();
        public ObservableCollection<CarApi> CarSales { get; set; } = new ObservableCollection<CarApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<ModelApi> Models { get; set; } = new List<ModelApi>();

        private CarApi selectedCar { get; set; }
        public CarApi SelectedCar
        {
            get => selectedCar;
            set
            {
                selectedCar = value;
                SignalChanged();
            }
        }

        private CarApi selectedCarSale { get; set; }
        public CarApi SelectedCarSale
        {
            get => selectedCarSale;
            set
            {
                selectedCarSale = value;
                SignalChanged();
            }
        }

        private EquipmentApi selectedEquipment { get; set; }
        public EquipmentApi SelectedEquipment
        {
            get => selectedEquipment;
            set
            {
                selectedEquipment = value;
                SignalChanged();
            }
        }

        public CustomCommand Save { get; set; }
        public CustomCommand AddCar { get; set; }
        public CustomCommand Cancel { get; set; }

        public SaleCarApi AddSaleVM { get; set; }

        public AddCarSaleViewModel(SaleCarApi saleCar)
        {
            Task.Run(GetList);
            if (saleCar == null)
            {
                AddSaleVM = new SaleCarApi { EquipmentPrice = 10000000, Articul = "111" };
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
                
            }

            Save = new CustomCommand(() =>
            {
                AddSaleVM.EquipmentId = SelectedEquipment.ID;
                AddSaleVM.Equipment = SelectedEquipment;
                AddSaleVM.Car = CarSales.Last();
                if (AddSaleVM.ID == 0)
                {
                    AddSale(AddSaleVM);
                    UIManager.ShowMessage(new MessageBoxDialogViewModel { Message = "Создан заказ" });
                    UIManager.CloseWindow(this);
                }
                else
                {
                    EditSale(AddSaleVM);
                    UIManager.ShowMessage(new MessageBoxDialogViewModel { Message = "Изменен заказ" });
                    UIManager.CloseWindow(this);
                }
            });

            AddCar = new CustomCommand(() =>
            {
                if (SelectedCar == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрано авто" });
                    return;
                }

                if (CheckContains(CarSales.ToList(), SelectedCar))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = $"Машина {SelectedCar.Model.ModelName} уже содержится на складе" });
                    return;
                }

                else
                {                   
                    if (AddSaleVM.CarId != 0 || AddSaleVM.CarId != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Вы точно желаете заменить свойство?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            CarSales.Clear();
                            SignalChanged(nameof(CarSales));
                        }
                        else
                        {
                            return;
                        }
                    }   
                    CarSales.Add(SelectedCar);
                    SignalChanged(nameof(CarSales));
                    EditSale(AddSaleVM);
                    AddSaleVM.Car = CarSales.Last();
                    AddSaleVM.CarId = CarSales.Last().ID;
                }
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }

        private async Task GetList()
        {
            var list = await Api.GetListAsync<List<CarApi>>("Car");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            Cars = new ObservableCollection<CarApi>(list);
            SignalChanged(nameof(Cars));
        }
        private void Get()
        {
            SelectedEquipment = Equipments.FirstOrDefault(s=> s.ID == AddSaleVM.EquipmentId);
            SelectedCarSale = AddSaleVM.Car;
        }

        private async Task EditSale(SaleCarApi saleCar)
        {
            var sale = await Api.PutAsync<SaleCarApi>(saleCar, "CarSales");
        }

        public async Task AddSale(SaleCarApi saleCar)
        {
            var sale = await Api.PostAsync<SaleCarApi>(saleCar, "CarSales");
        }

        private bool CheckContains(List<CarApi> list, CarApi car)
        {
            bool result = false;
            foreach (CarApi item in list)
            {
                if (item.ID == car.ID)
                {
                    result = true;
                }
            }
            return result;
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.GetListAsync<ObservableCollection<CarApi>>("Car");
            else
                searchResult = await Api.SearchAsync<ObservableCollection<CarApi>>("Модель", search, "Car");
            UpdateList();
        }

        private void UpdateList()
        {
            Cars = searchResult;
            SignalChanged(nameof(Cars));
        }
    }
}
