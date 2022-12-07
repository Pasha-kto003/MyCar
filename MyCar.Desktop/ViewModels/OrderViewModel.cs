using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public List<OrderApi> Orders { get; set; }  

        private List<OrderApi> searchResult;
        private List<OrderApi> FullOrders;

        private OrderApi selectedOrder { get; set; }
        public OrderApi SelectedOrder
        {
            get => selectedOrder;
            set
            {
                selectedOrder = value;
                SignalChanged();
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

        private ActionTypeApi selectedTypeFilter;
        public ActionTypeApi SelectedTypeFilter
        {
            get=> selectedTypeFilter;
            set
            {
                selectedTypeFilter = value;
                Task.Run(Search);
            }
        }

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

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        public CustomCommand AddOrder { get; set; }
        public CustomCommand EditOrder { get; set; }

        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;
        public string Pages { get; set; }

        public OrderViewModel()
        {
            Task.Run(GetOrders).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Дата заказа", "Статус заказа", "Пользователь" });
            selectedSearchType = SearchType.First();
            SelectedTypeFilter = TypeFilter.First();

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

        private void UpdateOrder()
        {
            Orders = searchResult;
            SignalChanged(nameof(searchResult));
        }

        private async Task GetOrders()
        {
            Orders = await Api.GetListAsync<List<OrderApi>>("Order");
            ActionTypes = await Api.GetListAsync<List<ActionTypeApi>>("ActionType");
            TypeFilter = ActionTypes;
            FullOrders = Orders;
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<OrderApi>>(SelectedSearchType, "$", "Order", SelectedTypeFilter.ActionTypeName);
            else
                searchResult = await Api.SearchFilterAsync<List<OrderApi>>(SelectedSearchType, search, "Order", SelectedTypeFilter.ActionTypeName);
            UpdateOrder();
            InitPagination();
            Pagination();
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
            if (rows > FullOrders.Count)
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
