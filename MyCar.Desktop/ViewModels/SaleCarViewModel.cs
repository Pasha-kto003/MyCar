using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows.AddWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class SaleCarViewModel : BaseViewModel
    {
        public string SearchCountRows { get; set; }
        public List<string> ViewCountRows { get; set; }
        public string SelectedViewCountRows
        {
            get => selectedViewCountRows;
            set
            {
                selectedViewCountRows = value;
                paginationPageIndex = 0;
                Pagination();
            }
        }

        private EquipmentApi selectedEquipmentfilter;
        public EquipmentApi SelectedEquipmentFilter
        {
            get => selectedEquipmentfilter;
            set
            {
                selectedEquipmentfilter = value;
                Task.Run(Search);
            }
        }

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

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Task.Run(Search);
            }
        }

        public List<SaleCarApi> SalesCar { get; set; } = new List<SaleCarApi>();
        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<EquipmentApi> EquipmentFilter { get; set; } = new List<EquipmentApi>();

        public SaleCarApi SelectedSale { get; set; }

        public CustomCommand AddSale { get; set; }
        public CustomCommand EditSale { get; set; }
        public CustomCommand DeleteSale { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        private List<SaleCarApi> searchResult;
        private List<SaleCarApi> FullSales;

        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;
        public string Pages { get; set; }

        public SaleCarViewModel()
        {
            Task.Run(GetSales);//.Wait()

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Артикул", "Авто" });
            SelectedSearchType = SearchType.First();

            EquipmentFilter = Equipments;
            EquipmentFilter.Add(new EquipmentApi { NameEquipment = "Все" });
            SelectedEquipmentFilter = EquipmentFilter.Last();

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "5", "Все" });
            selectedViewCountRows = ViewCountRows.Last();


            AddSale = new CustomCommand(() =>
            {
                AddCarSaleWindow addCar = new AddCarSaleWindow();
                addCar.ShowDialog();
                Task.Run(GetSales);
            });

            EditSale = new CustomCommand(() =>
            {
                AddCarSaleWindow editCar = new AddCarSaleWindow(SelectedSale);
                editCar.ShowDialog();
                Task.Run(GetSales);
            });
        }

        public async Task GetSales()
        {
            SalesCar = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            FullSales = SalesCar;
        }

        public void UpdateList()
        {
            SalesCar = searchResult;
            SignalChanged(nameof(SalesCar));
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<SaleCarApi>>(SelectedSearchType, "$", "CarSales", SelectedEquipmentFilter.NameEquipment);
            else
                searchResult = await Api.SearchFilterAsync<List<SaleCarApi>>(SelectedSearchType, search, "CarSales", SelectedEquipmentFilter.NameEquipment);
            UpdateList();
            InitPagination();
            Pagination();
        }

        public void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {FullSales.Count()}";
            paginationPageIndex = 0;
        }

        public void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                SalesCar = searchResult;
            }
            else
            {
                SalesCar = searchResult.Skip(rowsOnPage * paginationPageIndex).Take(rowsOnPage).ToList();
                SignalChanged(nameof(Cars));
            }
            int.TryParse(SelectedViewCountRows, out rows);
            if (rows == 0)
                rows = FullSales.Count;
            CountPages = (searchResult.Count() - 1) / rows;
            Pages = $"{paginationPageIndex + 1} из {CountPages + 1}";
        }
    }
}