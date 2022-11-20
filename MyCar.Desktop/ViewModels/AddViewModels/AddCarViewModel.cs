using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyCar.Desktop.ViewModels
{
    public class AddCarViewModel : BaseViewModel
    {
        public CarApi AddCarVM { get; set; }

        public List<MarkCarApi> Marks { get; set; }
        public List<MarkCarApi> CarMarks { get; set; }
        public List<ModelApi> Models { get; set; }
        public List<ModelApi> CarModels { get; set; }
        public List<BodyTypeApi> BodyTypes { get; set; }
        public List<EquipmentApi> Equipments { get; set; }
        public ObservableCollection<CharacteristicCarApi> CharacteristicsCar { get; set; }
        public List<CharacteristicApi> Characteristics { get; set; }

        public Image ImageCar { get; set; }

        public string CharacteristicValue { get; set; }

        private ModelApi selectedModel { get; set; }
        public ModelApi SelectedModel
        {
            get => selectedModel;
            set
            {
                selectedModel = value;
                SignalChanged();
            }
        }

        public ModelApi SelectedCarModel { get; set; }

        private MarkCarApi selectedMark { get; set; }
        public MarkCarApi SelectedMark
        {
            get => selectedMark;
            set
            {
                selectedMark = value;
                SignalChanged();
            }
        }

        public MarkCarApi SelectedCarMark { get; set; }

        private BodyTypeApi selectedBodyType { get; set; }
        public BodyTypeApi SelectedBodyType
        {
            get => selectedBodyType;
            set
            {
                selectedBodyType = value;
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

        private CharacteristicApi selectedCharacteristic { get; set; }
        public CharacteristicApi SelectedCharacteristic
        {
            get=> selectedCharacteristic;
            set
            {
                selectedCharacteristic = value;
                SignalChanged();
            }
        }

        public CustomCommand Save { get; set; }
        public CustomCommand AddCharacteristic { get; set; }

        public AddCarViewModel(CarApi car)
        {
            Models = new List<ModelApi>();
            Marks = new List<MarkCarApi>();
            CharacteristicsCar = new ObservableCollection<CharacteristicCarApi>();

            if (car == null)
            {
                AddCarVM = new CarApi
                {
                    Articul = "1234",
                    CarPrice = 1000000,

                };
                GetCars(car);
            }

            else
            {
                GetCars(car);
                AddCarVM = new CarApi
                {
                    ID = car.ID,
                    Articul = car.Articul,
                    CarPrice = car.CarPrice,
                    PhotoCar = car.PhotoCar,
                    ModelId = car.ModelId,
                    Model = car.Model,
                    BodyType = car.BodyType,
                    TypeId = car.TypeId,
                    EquipmentId = car.EquipmentId
                };
            }

            GetCars(AddCarVM);

            //GetModels(AddCarVM.Model.MarkCar.MarkName); //SelectedMark.MarkName

            AddCharacteristic = new CustomCommand(() =>
            {
                CharacteristicCarApi characteristic = new CharacteristicCarApi
                {
                    CarId = AddCarVM.ID,
                    CharacteristicValue = CharacteristicValue.ToString(),
                    Characteristic = SelectedCharacteristic,
                    CharacteristicId = SelectedCharacteristic.ID
                };

                CharacteristicsCar.Add(characteristic);
                SignalChanged(nameof(CharacteristicsCar));
            });

            Save = new CustomCommand(() =>
            {
                AddCarVM.ModelId = SelectedModel.ID;
                AddCarVM.Model = SelectedModel;
                AddCarVM.Model.MarkId = SelectedMark.ID;
                AddCarVM.EquipmentId = SelectedEquipment.ID;
                AddCarVM.Equipment = SelectedEquipment;
                AddCarVM.TypeId = SelectedBodyType.ID;
                AddCarVM.BodyType = SelectedBodyType;

                if (SelectedMark == null || SelectedMark.ID == 0)
                {
                    SendMessage("Не выбрана марка");

                }
                else if (SelectedModel == null || SelectedModel.ID == 0)
                {
                    SendMessage("Не выбрана модель");

                }

                else if (SelectedBodyType == null || SelectedBodyType.ID == 0)
                {
                    SendMessage("Не выбран тип кузова");

                }

                else if (SelectedEquipment == null || SelectedEquipment.ID == 0)
                {
                    SendMessage("Не выбрана коплектация");

                }

                if (AddCarVM.ID == 0)
                {
                    AddCarVM.CharacteristicCars = CharacteristicsCar.ToList();
                    PostCar(AddCarVM);
                    SendMessage($"Машина {AddCarVM.Model.ModelName} успешно создана");
                }

                else
                {
                    EditCar(AddCarVM);
                    SendMessage($"Машина {AddCarVM.Model.ModelName} успешно изменена");
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        CloseWindow(window);
                    }
                }
                SignalChanged(nameof(CharacteristicsCar));
            });
        }

        public async Task GetCar(int id)
        {
            AddCarVM = await Api.GetAsync<CarApi>(id, "Car");
        }

        public async Task GetModels(string markName)
        {
            Models = await Api.GetModelApi<List<ModelApi>>(markName, "Model");
        }

        public async Task PostCar(CarApi carApi)
        {
            var car = await Api.PostAsync<CarApi>(carApi, "Car");
        }

        public async Task EditCar(CarApi carApi)
        {
            var car = await Api.PutAsync<CarApi>(carApi, "Car");
        }

        public void SendMessage(string message)
        {
            UIManager.ShowMessage(new Dialogs.MessageBoxDialogViewModel
            {
                Message = message,
                OkText = "ОК",
                Title = "Успех!"
            });
            return;
        }

        public async Task GetCars(CarApi carApi)
        {
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");

            if (carApi == null)
            {
                SelectedModel = Models.FirstOrDefault();
                SelectedMark = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId);
                SignalChanged(nameof(SelectedMark));
                SelectedEquipment = Equipments.FirstOrDefault();
            }

            else
            {
                var car = await Api.GetAsync<CarApi>(carApi.ID, "Car");
                var list = await Api.GetListAsync<List<CharacteristicCarApi>>("CharacteristicCar");
                car.CharacteristicCars = list.Where(s => s.CarId == car.ID).ToList();
                CharacteristicsCar = new ObservableCollection<CharacteristicCarApi>(car.CharacteristicCars);

                foreach (var characteristic in CharacteristicsCar)
                {
                    characteristic.Characteristic = Characteristics.FirstOrDefault(s => s.ID == characteristic.CharacteristicId);
                }
                SelectedModel = Models.FirstOrDefault(s => s.ID == carApi.ModelId);
                SelectedMark = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId);
                SignalChanged(nameof(SelectedMark));
                SignalChanged(nameof(SelectedModel));
                SelectedEquipment = Equipments.FirstOrDefault(s => s.ID == carApi.EquipmentId);
                SignalChanged(nameof(SelectedEquipment));
                SelectedBodyType = BodyTypes.FirstOrDefault(s => s.ID == carApi.TypeId);
            } 
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
