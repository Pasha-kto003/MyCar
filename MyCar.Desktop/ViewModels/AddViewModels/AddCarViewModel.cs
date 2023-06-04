using Microsoft.Win32;
using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows.AddWindows;
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
        public List<CarApi> Cars { get; set; }

        public List<ModelApi> FullModels { get; set; }
        public List<ModelApi> Models { get; set; }
        public List<BodyTypeApi> BodyTypes { get; set; }
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
                UpdateMark(selectedModel);
            }
        }
        public MarkCarApi selectedMark;
        public MarkCarApi SelectedMark
        {
            get => selectedMark;
            set
            {
                selectedMark = value;
                UpdateModels(selectedMark);
            }
        }
        public ModelApi SelectedCarModel { get; set; }
        public BodyTypeApi SelectedBodyType { get; set; }
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
        public CustomCommand Cancel { get; set; }
        public CustomCommand DeleteCharacteristic { get; set; }

        public AddCarViewModel(CarApi car)
        {
            Task.Run(GetList).Wait();

            if (car == null)
            {
                AddCarVM = new CarApi{};
            }
            else
            {
                AddCarVM = new CarApi
                {
                    ID = car.ID,
                    CarPrice = car.CarPrice,
                    PhotoCar = car.PhotoCar,
                    ModelId = car.ModelId,
                    Model = car.Model,
                    BodyType = car.BodyType,
                    TypeId = car.TypeId,
                    CharacteristicCars = car.CharacteristicCars
                };
                Cars.RemoveAll(car=>car.ID == AddCarVM.ID);
                SelectedCarModel = AddCarVM.Model;
                GetInfo();
            }
            ImageCar = AddCarVM.PhotoCar;

            AddCharacteristic = new CustomCommand(() =>
            {
                if (SelectedCharacteristic == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбранна характеристика авто" });
                    return;
                }
                if (!IsNonNegativeNumber(CharacteristicValue))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не верное число!" });
                    return;
                }
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

            AddImage = new CustomCommand(async() =>
            {
                MethodResult result = await UIManager.AddImageAsync();
                if (result.IsSuccess)
                {
                    ImageCar = result.Data.ToString();
                    AddCarVM.PhotoCar = result.Data.ToString();
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
                if (SelectedModel == null || SelectedBodyType == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Заполнены не все поля!" });
                    return;
                }
                if (Cars.Exists(s=>s.ModelId == SelectedModel.ID))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Выбранная модель уже используется!" });
                    return;
                }
                if (AddCarVM.CarPrice < 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Цена не может быть отрицательной!" });
                    return;
                }
                AddCarVM.ModelId = SelectedModel.ID;
                AddCarVM.Model = SelectedModel;
                AddCarVM.BodyType = SelectedBodyType;
                AddCarVM.TypeId = SelectedBodyType.ID;
                
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
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }
        private bool IsNonNegativeNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            if (decimal.TryParse(input, out decimal result))
            {
                if (result >= 0)
                    return true;
            }

            return false;
        }
        private async Task GetList()
        {
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            FullModels = Models;
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
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

        private void UpdateMark(ModelApi model)
        {
            if (model != null)
                SelectedMark = Marks.FirstOrDefault(s => s.ID == model.MarkId);
        }
        private void UpdateModels(MarkCarApi mark)
        {
            if (mark != null)
                Models = FullModels.Where(s => s.MarkId == mark.ID).ToList();
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
