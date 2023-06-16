﻿using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
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

        private MarkCarApi selectedMarkFilter;
        public MarkCarApi SelectedMarkFilter
        {
            get => selectedMarkFilter;
            set
            {
                selectedMarkFilter = value;
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
        public string CarOptions { get; set; } = "";
        public CarApi SelectedCar { get; set; }
        public List<CarApi> Cars { get; set; } = new List<CarApi>();
        public List<MarkCarApi> MarkFilter { get; set; } = new List<MarkCarApi>();
        public List<MarkCarApi> MarkCars { get; set; } = new List<MarkCarApi>();
        public List<BodyTypeApi> BodyTypes { get; set; } = new List<BodyTypeApi>();
        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public List<SaleCarApi> SaleCars { get; set; } = new List<SaleCarApi>();

        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;
        public string Pages { get; set; }


        public CustomCommand EditCar { get; set; }
        public CustomCommand DeleteCar { get; set; }
        public CustomCommand AddCar { get; set; }

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        private List<CarApi> searchResult;
        private List<CarApi> FullCars;

        public CarViewModel()
        {
            Task.Run(GetCarList).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Модель", "Цена", "Название" });
            selectedSearchType = SearchType.First();

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "5", "Все" });
            selectedViewCountRows = ViewCountRows.Last();

            MarkFilter = MarkCars;
            MarkFilter.Add(new MarkCarApi { MarkName = "Все" });
            SelectedMarkFilter = MarkFilter.Last();

            BackPage = new CustomCommand(() => {
                if (searchResult == null)
                    return;
                if (paginationPageIndex > 0)
                    paginationPageIndex--;
                Pagination();

            });

            ForwardPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                int.TryParse(SelectedViewCountRows, out int rowsOnPage);
                if (rowsOnPage == 0)
                    return;
                int countPage = searchResult.Count / rowsOnPage;
                CountPages = countPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();
            });

            AddCar = new CustomCommand(() =>
            {
                AddCarWindow window = new AddCarWindow();
                window.ShowDialog();
                Task.Run(GetCarList);
            });

            EditCar = new CustomCommand(() =>
            {
                if (SelectedCar == null || SelectedCar.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран автомобиль для редактирования!" });
                    return;
                }
                AddCarWindow window = new AddCarWindow(SelectedCar);
                window.ShowDialog();
                Task.Run(GetCarList);
            });
            DeleteCar = new CustomCommand(async() =>
            {
                if (SelectedCar == null || SelectedCar.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран автомобиль!" });
                    return;
                }
                if (SaleCars.Any(s=>s.CarId == SelectedCar.ID))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Авто связано с другими сущностями!" });
                    return;
                }
                if (SelectedCar.CharacteristicCars.Count != 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Авто связано с другими сущностями!" });
                    return;
                }
                await DeleteCarAsync(SelectedCar);
                Task.Run(GetCarList);
            });
        }

        private void UpdateList()
        {
            Cars = searchResult;
            SignalChanged(nameof(searchResult));
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<CarApi>>(SelectedSearchType, "$", "Car", SelectedMarkFilter.MarkName);
            else
                searchResult = await Api.SearchFilterAsync<List<CarApi>>(SelectedSearchType, search, "Car", SelectedMarkFilter.MarkName);
            UpdateList();
            InitPagination();
            Pagination();
        }

        private async Task GetCarList()
        {
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            MarkCars = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            SaleCars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
            FullCars = Cars;
        }
        private async Task DeleteCarAsync(CarApi car)
        {
            var ch = await Api.DeleteAsync<CarApi>(car, "Car");
        }
       
        public void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {FullCars.Count()}";
            paginationPageIndex = 0;
        }

        public void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Cars = searchResult;
            }
            else
            {
                Cars = searchResult.Skip(rowsOnPage * paginationPageIndex).Take(rowsOnPage).ToList();
                SignalChanged(nameof(Cars));
            }
            int.TryParse(SelectedViewCountRows, out rows);
            if (rows == 0)
                rows = FullCars.Count;
            if (rows > FullCars.Count)
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Превышено количество объектов" });
                SelectedViewCountRows = ViewCountRows.Last();
                SignalChanged(nameof(SelectedViewCountRows));
                return;
            }
            CountPages = (searchResult.Count() - 1) / rows;
            Pages = $"{paginationPageIndex + 1} из {CountPages + 1}";
        }
    }
}
