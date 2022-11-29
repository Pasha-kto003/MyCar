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
        public string ImageCar { get; set; }
        public List<MarkCarApi> Marks { get; set; }
        public List<ModelApi> Models { get; set; }
        public List<BodyTypeApi> BodyTypes { get; set; }
        public List<EquipmentApi> Equipments { get; set; }
        public ObservableCollection<CharacteristicCarApi> CharacteristicsCar { get; set; }
            = new ObservableCollection<CharacteristicCarApi>();

        public List<CharacteristicApi> Characteristics { get; set; }

        public string CharacteristicValue { get; set; }
        public string MarkText { get; set; }

        public ModelApi selectedModel;
        public ModelApi SelectedModel
        {
            get => selectedModel;
            set
            {
                selectedModel = value;
                GetMarkText(selectedModel);
            }
        }
        public ModelApi SelectedCarModel { get; set; }
        public BodyTypeApi SelectedBodyType { get; set; }
        public EquipmentApi SelectedEquipment { get; set; }
        public CharacteristicApi SelectedCharacteristic { get; set; }

        private CharacteristicCarApi selectedCharacteristicCar;
        public CharacteristicCarApi SelectedCharacteristicCar
        {
            get => selectedCharacteristicCar;
            set
            {
                selectedCharacteristicCar = value;
                SelectedCharacteristicChanged(selectedCharacteristicCar);
            }
        }

        public CustomCommand Save { get; set; }
        public CustomCommand AddCharacteristic { get; set; }
        public CustomCommand AddImage { get; set; }
        public CustomCommand DeleteCharacteristic { get; set; }

        public AddCarViewModel(CarApi car)
        {
            Task.Run(GetList).Wait();

            if (car == null)
            {
                AddCarVM = new CarApi
                {
                    Articul = "1234",
                    CarPrice = 1000000,
                };
            }
            else
            {
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
                    CharacteristicCars = car.CharacteristicCars
                };
                SelectedCarModel = AddCarVM.Model;
                GetInfo();
            }
            ImageCar = AddCarVM.PhotoCar;

            AddCharacteristic = new CustomCommand(() =>
            {
                bool match = false;
                foreach (var item in CharacteristicsCar)
                {
                    if (item.CharacteristicId == SelectedCharacteristic.ID)
                    {
                        item.CharacteristicValue = CharacteristicValue;
                        CharacteristicsCar.Remove(item);
                        CharacteristicsCar.Add(item);
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    CharacteristicCarApi characteristicCar = new CharacteristicCarApi
                    {
                        CarId = AddCarVM.ID,
                        CharacteristicValue = CharacteristicValue.ToString(),
                        Characteristic = SelectedCharacteristic,
                        CharacteristicId = SelectedCharacteristic.ID
                    };
                    CharacteristicsCar.Add(characteristicCar);
                }
            });

            string dir = Environment.CurrentDirectory;
            AddImage = new CustomCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = dir + @"\CarImages\";
                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(openFileDialog.FileName);
                        var newPath = Environment.CurrentDirectory + @"\CarImages\" + info.Name;
                        if (!File.Exists(newPath))
                            File.Copy(openFileDialog.FileName, newPath);
                        ImageCar = info.Name;
                        AddCarVM.PhotoCar = info.Name;
                    }

                    catch (Exception e)
                    {
                        UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                    }
                }
            });

            DeleteCharacteristic = new CustomCommand(() =>
            {
                if (SelectedCharacteristicCar == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбранна характеристика авто" });
                    return;
                }
                else
                {
                    MessageBoxDialogViewModel result = new MessageBoxDialogViewModel { Title = "Подтверждение", Message = $"Вы точно хотите удалить параметр?" };
                    UIManager.ShowMessageYesNo(result);
                    if (result.Result)
                    {
                        CharacteristicsCar.Remove(SelectedCharacteristicCar);
                    }
                }
            });

            Save = new CustomCommand(async () =>
            {
                AddCarVM.ModelId = SelectedModel.ID;
                AddCarVM.Model = SelectedModel;
                AddCarVM.BodyType = SelectedBodyType;
                AddCarVM.TypeId = SelectedBodyType.ID;
                AddCarVM.BodyType = SelectedBodyType;
                AddCarVM.CharacteristicCars = CharacteristicsCar.ToList();

                foreach (var characteristic in AddCarVM.CharacteristicCars)
                {
                    characteristic.Characteristic = Characteristics.FirstOrDefault(s => s.ID == characteristic.CharacteristicId);
                    AddCarVM.CarOptions += $"{characteristic.Characteristic.CharacteristicName} {characteristic.CharacteristicValue} \n";
                }

                if (SelectedModel == null || SelectedModel.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана модель" });
                    return;
                }
                else if (SelectedBodyType == null || SelectedBodyType.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран тип кузова" });
                    return;
                }

                else if (SelectedEquipment == null || SelectedEquipment.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана комплектация" });
                    return;
                }

                if (AddCarVM.ID == 0)
                {
                    await PostCar(AddCarVM);
                }
                else
                {
                    await EditCar(AddCarVM);
                }

                UIManager.CloseWindow(this);
            });
        }
        private async Task GetList()
        {
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
        }
        private void GetInfo()
        {
            CharacteristicsCar = new ObservableCollection<CharacteristicCarApi>(AddCarVM.CharacteristicCars);
            SelectedModel = Models.FirstOrDefault(s => s.ID == AddCarVM.ModelId);
            SelectedBodyType = BodyTypes.FirstOrDefault(s => s.ID == AddCarVM.TypeId);
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

        private void GetMarkText(ModelApi model)
        {
            if (model != null)
                MarkText = Marks.FirstOrDefault(s => s.ID == model.MarkId).MarkName;
        }
        private void SelectedCharacteristicChanged(CharacteristicCarApi selectedCharacteristicCar)
        {
            if (selectedCharacteristicCar != null)
            {
                SelectedCharacteristic = Characteristics.FirstOrDefault(s => s.ID == selectedCharacteristicCar.CharacteristicId);
                CharacteristicValue = selectedCharacteristicCar.CharacteristicValue;
            }
        }
    }
}
