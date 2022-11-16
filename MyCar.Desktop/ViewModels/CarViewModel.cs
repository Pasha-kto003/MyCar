using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class CarViewModel : BaseViewModel
    {

        private ModelApi selectedModelFilter;
        public ModelApi SelectedModelFilter
        {
            get => selectedModelFilter;
            set
            {
                selectedModelFilter = value;
                Search();
            }
        }
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Search();
            }
        }

        private string carOptions { get; set; } = "";
        public string CarOptions
        {
            get => carOptions;
            set
            {
                carOptions = value;
                SignalChanged();
            }
        }

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

        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<ModelApi> ModelFilter { get; set; }
        public List<MarkCarApi> MarkCars { get; set; } = new List<MarkCarApi>();
        public List<BodyTypeApi> BodyTypes { get; set; } = new List<BodyTypeApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<CharacteristicCarApi> CharacteristicCars { get; set; } = new List<CharacteristicCarApi>();
        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();

        public CustomCommand EditCar { get; set; }
        public CustomCommand DeleteCar { get; set; }
        public CustomCommand AddCar { get; set; }

        private List<CarApi> searchResult;
        private List<CarApi> FullCars;

        public CarViewModel()
        {
            Task.Run(GetCarList);

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Модель", "Артикул", "Марка", "Цена", "Отменить" });
            selectedSearchType = SearchType.First();


            AddCar = new CustomCommand(() =>
            {
                AddCarWindow window = new AddCarWindow();
                window.ShowDialog();
                GetCarList();
            });

            EditCar = new CustomCommand(() =>
            {
                AddCarWindow window = new AddCarWindow(SelectedCar);
                window.ShowDialog();
                GetCarList();
            });
        }

        private async Task UpdateList()
        {
            Cars = searchResult;
            SignalChanged(nameof(searchResult));
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<CarApi>>(SelectedSearchType, "$", "Car", SelectedModelFilter.ModelName);
            else
                searchResult = await Api.SearchFilterAsync<List<CarApi>>(SelectedSearchType, search, "Car", SelectedModelFilter.ModelName);
            UpdateList();
        }

        private async Task GetCarList()
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            MarkCars = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            CharacteristicCars = await Api.GetListAsync<List<CharacteristicCarApi>>("CharacteristicCar");
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");

            ModelFilter = await Api.GetListAsync<List<ModelApi>>("Model");
            ModelFilter.Add(new ModelApi { ModelName = "Все" });
            SelectedModelFilter = ModelFilter.Last();

            FullCars = Cars;
            SignalChanged(nameof(Cars));
        }
    }
}
