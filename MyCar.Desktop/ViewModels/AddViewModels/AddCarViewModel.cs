using Microsoft.Win32;
using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyCar.Desktop.ViewModels
{
    public class AddCarViewModel : BaseViewModel
    {
        public CarApi AddCarVM { get; set; }

        private BitmapImage imageCar { get; set; }
        public BitmapImage ImageCar
        {
            get=> imageCar;
            set
            {
                imageCar = value;
                SignalChanged();
            }
        }

        public List<MarkCarApi> Marks { get; set; }
        public List<MarkCarApi> CarMarks { get; set; }
        public List<ModelApi> Models { get; set; }
        public ObservableCollection<ModelApi> CarModels { get; set; }
        public List<BodyTypeApi> BodyTypes { get; set; }
        public List<EquipmentApi> Equipments { get; set; }
        public ObservableCollection<CharacteristicCarApi> CharacteristicsCar { get; set; }
        public List<CharacteristicApi> Characteristics { get; set; }

        public string CharacteristicValue { get; set; }

        public string MarkText { get; set; }

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

        private CharacteristicCarApi selectedCharacteristicCar { get; set; }
        public CharacteristicCarApi SelectedCharacteristicCar
        {
            get => selectedCharacteristicCar;
            set
            {
                selectedCharacteristicCar = value;
                SignalChanged();
            }
        }

        public CustomCommand Save { get; set; }
        public CustomCommand AddCharacteristic { get; set; }
        public CustomCommand AddModel { get; set; }
        public CustomCommand AddImage { get; set; }
        public CustomCommand DeleteCharacteristic { get; set; }

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
                    PhotoCar = @"/CarImages/picture.png"
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
                SelectedCarModel = AddCarVM.Model;
            }

            if (AddCarVM.PhotoCar == null)
            {
                ImageCar = GetImageFromPath(Environment.CurrentDirectory + "//" + @"/CarImages/picture.png");
            }

            else
            {
                ImageCar = GetImageFromPath(Environment.CurrentDirectory + "//" + AddCarVM.PhotoCar);
            }

            GetCars(AddCarVM);

            AddModel = new CustomCommand(() =>
            {
                if(SelectedModel == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Вы не выбрали модель" });
                    return;
                }
                else
                {
                    AddCarVM.Model = SelectedCarModel;
                    CarModels.Add(SelectedModel);
                    SignalChanged(nameof(CarModels));
                    MarkText = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId).MarkName;
                    AddCarVM.ModelId = SelectedModel.ID;
                    EditCar(AddCarVM);
                    if (CarModels.Count > 1)
                    {
                        GetModel();
                    }
                }
            });

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

            string dir = Environment.CurrentDirectory;
            AddImage = new CustomCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if(openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(openFileDialog.FileName);
                        ImageCar = GetImageFromPath(openFileDialog.FileName);
                        AddCarVM.PhotoCar = $"/CarImages/{info.Name}";
                        var newParh = Environment.CurrentDirectory + AddCarVM.PhotoCar;
                        File.Copy(openFileDialog.FileName, newParh);
                    }

                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            DeleteCharacteristic = new CustomCommand(() =>
            {
                if(SelectedCharacteristicCar == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбранна характеристика авто" });
                    return;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Вы точно желаете удалить свойство?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        CharacteristicsCar.Remove(SelectedCharacteristicCar);
                        SignalChanged(nameof(CharacteristicsCar));
                        AddCarVM.CharacteristicCars = CharacteristicsCar.ToList();
                        EditCar(AddCarVM);
                    }
                }
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
                AddCarVM.CharacteristicCars = CharacteristicsCar.ToList();
                AddCarVM.CarMark = MarkText;

                foreach (var characteristic in AddCarVM.CharacteristicCars)
                {
                    characteristic.Characteristic = Characteristics.FirstOrDefault(s => s.ID == characteristic.CharacteristicId);
                    AddCarVM.CarOptions += $"{characteristic.Characteristic.CharacteristicName} {characteristic.CharacteristicValue} \n";
                }
                if (SelectedMark == null || SelectedMark.ID == 0)
                {
                    SendMessage("Не выбрана марка");

                }
                else if (SelectedModel == null || SelectedModel.ID == 0)
                {
                    SendMessage("Не выбрана модель");
                }
                else if (SelectedModel.MarkId != SelectedMark.ID)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Данная марка не содержит выбранную модель" });
                    return;
                }
                else if (SelectedBodyType == null || SelectedBodyType.ID == 0)
                {
                    SendMessage("Не выбран тип кузова");
                }

                else if (SelectedEquipment == null || SelectedEquipment.ID == 0)
                {
                    SendMessage("Не выбрана коплектация");
                }

                else if (CharacteristicsCar == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана характеристика авто" });
                    return;
                }

                if (AddCarVM.ID == 0)
                {
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

        public async Task GetModels(CarApi car)
        {
            var list = await Api.GetListAsync<List<ModelApi>>("Model");
            var l = list.Where(s=> s.ID == car.ModelId).ToList();
            CarModels = new ObservableCollection<ModelApi>(l);
            SignalChanged(nameof(CarModels));
        }

        public async Task PostCar(CarApi carApi)
        {
            carApi.CharacteristicCars = CharacteristicsCar.ToList();
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

        public async Task GetModel()
        {
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            MessageBoxResult result = MessageBox.Show("Вы точно желаете заменить свойство?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CarModels.Clear();
                AddCarVM.Model = SelectedCarModel;
                CarModels.Add(SelectedModel);
                SignalChanged(nameof(CarModels));
                MarkText = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId).MarkName;
                AddCarVM.ModelId = SelectedModel.ID;
                EditCar(AddCarVM);
            }
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
                CarModels = new ObservableCollection<ModelApi>();
                SelectedCarModel = CarModels.FirstOrDefault();
            }

            else
            {
                var car = await Api.GetAsync<CarApi>(carApi.ID, "Car");
                var list = await Api.GetListAsync<List<CharacteristicCarApi>>("CharacteristicCar");
                car.CharacteristicCars = list.Where(s => s.CarId == car.ID).ToList();
                CharacteristicsCar = new ObservableCollection<CharacteristicCarApi>(car.CharacteristicCars);
                GetModels(carApi);
                foreach (var characteristic in CharacteristicsCar)
                {
                    characteristic.Characteristic = Characteristics.FirstOrDefault(s => s.ID == characteristic.CharacteristicId);
                }
                SelectedModel = Models.FirstOrDefault(s => s.ID == carApi.ModelId);
                SelectedMark = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId);
                SignalChanged(nameof(SelectedMark));
                SignalChanged(nameof(SelectedModel));

                MarkText = Marks.FirstOrDefault(s => s.ID == SelectedModel.MarkId).MarkName;
                SelectedEquipment = Equipments.FirstOrDefault(s => s.ID == carApi.EquipmentId);
                SignalChanged(nameof(SelectedEquipment));
                SelectedBodyType = BodyTypes.FirstOrDefault(s => s.ID == carApi.TypeId);
            } 
        }

        private BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(url, UriKind.Absolute);
            img.EndInit();
            return img;
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
