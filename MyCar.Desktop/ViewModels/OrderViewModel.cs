using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Windows.AddWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class OrderViewModel : BaseViewModel
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

        private ActionTypeApi selectedActionTypeFilter;
        public ActionTypeApi SelectedActionTypeFilter
        {
            get => selectedActionTypeFilter;
            set
            {
                selectedActionTypeFilter = value;
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

        private List<OrderApi> searchResult;

        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;
        public string Pages { get; set; }

        public OrderApi SelectedOrder { get; set; }
        public List<ActionTypeApi> ActionTypeFilter { get; set; }
        public List<ActionTypeApi> ActionTypes { get; set; }
        public List<OrderApi> Orders { get; set; }
        public List<OrderApi> FullOrders { get; set; }
        public CustomCommand EditOrder { get; set; }
        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        public OrderViewModel()
        {
            Task.Run(GetOrders).Wait();

            ActionTypeFilter = ActionTypes;
            ActionTypeFilter.Add(new ActionTypeApi { ActionTypeName = "Все" });
            SelectedActionTypeFilter = ActionTypeFilter.Last();

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "5", "Все" });
            selectedViewCountRows = ViewCountRows.Last();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Дата", "Заказчик", "№ Заказа" });
            SelectedSearchType = SearchType.First();

            EditOrder = new CustomCommand(() => {
                if (SelectedOrder == null || SelectedOrder.ID == 0) return;
                AddOrderWindow addOrder = new AddOrderWindow(SelectedOrder);
                addOrder.ShowDialog();
                Task.Run(GetOrders);
            });

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
        }
        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<OrderApi>>(SelectedSearchType, "$", "Order", SelectedActionTypeFilter.ActionTypeName);
            else
                searchResult = await Api.SearchFilterAsync<List<OrderApi>>(SelectedSearchType, search, "Order", SelectedActionTypeFilter.ActionTypeName);
            UpdateList();
            InitPagination();
            Pagination();
        }
        private async Task GetOrders()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");

            FullOrders = Orders;
        }
        public void UpdateList()
        {
            Orders = searchResult;
            SignalChanged(nameof(Orders));
        }

        public void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {FullOrders.Count()}";
            paginationPageIndex = 0;
        }

        public void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Orders = searchResult;
            }
            else
            {
                Orders = searchResult.Skip(rowsOnPage * paginationPageIndex).Take(rowsOnPage).ToList();
                SignalChanged(nameof(Orders));
            }
            int.TryParse(SelectedViewCountRows, out rows);
            if (rows == 0)
                rows = FullOrders.Count;
            CountPages = (searchResult.Count() - 1) / rows;
            Pages = $"{paginationPageIndex + 1} из {CountPages + 1}";
        }
    }
}
