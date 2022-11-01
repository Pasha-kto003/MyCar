using ModelsApi;
using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class CarViewModel : BaseViewModel
    {
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

        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<MarkCarApi> MarkCars { get; set; } = new List<MarkCarApi>();
        public List<BodyTypeApi> BodyTypes { get; set; } = new List<BodyTypeApi>();

        private List<CarApi> searchResult;
        private List<CarApi> FullCars;

        public CarViewModel()
        {
            Task.Run(GetCarList);

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Модель", "Артикул", "Марка", "Цена", "Отменить" });
            selectedSearchType = SearchType.First();

            Task.Run(Search);
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            searchResult = await Api.SearchAsync<List<CarApi>>(SelectedSearchType, search, "Car");
            UpdateList();
        }

        private async Task UpdateList()
        {
            Cars = searchResult;
            SignalChanged(nameof(searchResult));
        }

        private async Task GetCarList()
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            MarkCars = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");

            FullCars = Cars;
            SignalChanged(nameof(Cars));
        }
    }
}
