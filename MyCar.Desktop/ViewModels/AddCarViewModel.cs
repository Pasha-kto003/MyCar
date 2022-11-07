using ModelsApi;
using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
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
        public List<ModelApi> Models { get; set; }
        public List<BodyTypeApi> BodyTypes { get; set; }
        public List<EquipmentApi> Equipments { get; set; }
        public List<CharacteristicCarApi> CharacteristicsCar { get; set; }

        public Image ImageCar { get; set; }

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

        public CustomCommand SaveCar { get; set; }

        public AddCarViewModel(CarApi car)
        {
            Models = new List<ModelApi>();
            Marks = new List<MarkCarApi>();
            CharacteristicsCar = new List<CharacteristicCarApi>();

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

            GetModels(AddCarVM.Model.MarkCar.MarkName); //SelectedMark.MarkName

            SaveCar = new CustomCommand(() =>
            {
                if(AddCarVM.ID == 0)
                {
                    if(SelectedMark == null || SelectedMark.ID == 0)
                    {
                        MessageBox.Show("Не введена марка авто");
                        return;
                    }
                    else if(SelectedModel == null || SelectedModel.ID == 0)
                    {
                        MessageBox.Show("Не выбрана модель авто");
                        return;
                    }

                    AddCarVM.ModelId = SelectedModel.ID;
                    AddCarVM.Model.MarkId = SelectedMark.ID;
                    AddCarVM.EquipmentId = SelectedEquipment.ID;
                    AddCarVM.TypeId = SelectedBodyType.ID;

                    PostCar(AddCarVM);

                    MessageBox.Show($"Машина {AddCarVM.Model.ModelName} успешно создана");
                    return;
                }

                else
                {
                    AddCarVM.ModelId = SelectedModel.ID;
                    AddCarVM.Model.MarkId = SelectedMark.ID;
                    AddCarVM.EquipmentId = SelectedEquipment.ID;
                    AddCarVM.TypeId = SelectedBodyType.ID;

                    EditCar(AddCarVM);

                    MessageBox.Show($"Машина {AddCarVM.Model.ModelName} успешно изменена");
                    return;
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        CloseWindow(window);
                    }
                }
                SignalChanged("Products");
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

        public async Task GetCars(CarApi carApi)
        {
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            CharacteristicsCar = await Api.GetListAsync<List<CharacteristicCarApi>>("CharacteristicCar");
            if(carApi == null)
            {
                SelectedMark = Marks.FirstOrDefault();
                SelectedModel = Models.FirstOrDefault();
                SelectedEquipment = Equipments.FirstOrDefault();
            }
            SelectedModel = carApi.Model;
            SignalChanged(nameof(SelectedModel));
            SelectedMark = carApi.Model.MarkCar;
            SignalChanged(nameof(SelectedMark));
            SelectedEquipment = carApi.Equipment;
            SignalChanged(nameof(SelectedEquipment));
            SelectedBodyType = carApi.BodyType;
            SignalChanged(nameof(SelectedBodyType));
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
