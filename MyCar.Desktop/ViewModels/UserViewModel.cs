using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class UserViewModel : BaseViewModel
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
        public List<UserTypeApi> UserTypeFilter { get; set; }

        private UserTypeApi selectedUserTypeFilter;
        public UserTypeApi SelectedUserTypeFilter
        {
            get => selectedUserTypeFilter;
            set
            {
                selectedUserTypeFilter = value;
                Task.Run(Search);
            }
        }

        public List<string> SearchType { get; set; }

        public string SelectedSearchType { get; set; }

        private List<UserApi> searchResult;
        private List<UserApi> FullUsers;

        public List<UserTypeApi> UserTypes { get; set; } = new List<UserTypeApi>();
        public List<PassportApi> Passports { get; set; } = new List<PassportApi>();
        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public UserApi SelectedUser { get; set; }

        public Visibility MenuVisibility { get; set; } = Visibility.Collapsed;

        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;
        public string Pages { get; set; }

        public CustomCommand AddUser { get; set; }
        public CustomCommand EditUser { get; set; }
        public CustomCommand EditPassword { get; set; }
        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }

        public UserViewModel()
        {
            GetVisibility();

            Task.Run(GetUserList).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Логин", "Фамилия", "Email" });
            SelectedSearchType = SearchType.First();

            UserTypeFilter = UserTypes;
            UserTypeFilter.Add(new UserTypeApi { TypeName = "Все" });
            SelectedUserTypeFilter = UserTypeFilter.Last();

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "5", "Все" });
            selectedViewCountRows = ViewCountRows.Last();

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

            AddUser = new CustomCommand(() =>
            {
                if (Configuration.CurrentUser.UserType.TypeName != "Администратор")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "У вас недостаточно прав" });
                    return;
                }
                AddUser edituser = new AddUser();
                edituser.ShowDialog();
                Task.Run(GetUserList);
            });

            EditUser = new CustomCommand(() =>
            {
                if (Configuration.CurrentUser.UserType.TypeName != "Администратор")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "У вас недостаточно прав" });
                    return;
                }    
                if (SelectedUser == null || SelectedUser.ID == 0) return;
                AddUser edituser = new AddUser(SelectedUser);
                edituser.ShowDialog();
                Task.Run(GetUserList);
            });

            EditPassword = new CustomCommand(() =>
            {
                if (Configuration.CurrentUser.UserType.TypeName != "Администратор")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "У вас недостаточно прав" });
                    return;
                }
                if (SelectedUser == null || SelectedUser.ID == 0) return;
                EditPasswordWindow editpass = new EditPasswordWindow(SelectedUser);
                editpass.ShowDialog();
                Task.Run(GetUserList);
            });
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<UserApi>>(SelectedSearchType, "$", "User", SelectedUserTypeFilter.TypeName);
            else
                searchResult = await Api.SearchFilterAsync<List<UserApi>>(SelectedSearchType, search, "User", SelectedUserTypeFilter.TypeName);
            UpdateList();
            InitPagination();
            Pagination();
        }
        private void UpdateList()
        {
            Users = searchResult;
            SignalChanged(nameof(Users));
        }

        private void GetVisibility()
        {
            if (Configuration.CurrentUser.UserType.TypeName == "Сотрудник")
            {
                MenuVisibility = Visibility.Hidden;
            }
            else
            {
                MenuVisibility = Visibility.Visible;
            }
        }

        private async Task GetUserList()
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            UserTypes = await Api.GetListAsync<List<UserTypeApi>>("UserType");
            Passports = await Api.GetListAsync<List<PassportApi>>("Passport"); 
            FullUsers = Users;
        }

        public void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {FullUsers.Count()}";
            paginationPageIndex = 0;
        }

        public void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Users = searchResult;
            }
            else
            {
                Users = searchResult.Skip(rowsOnPage * paginationPageIndex).Take(rowsOnPage).ToList();
                SignalChanged(nameof(Users));
            }
            int.TryParse(SelectedViewCountRows, out rows);
            if (rows == 0)
                rows = FullUsers.Count;
            CountPages = (searchResult.Count() - 1) / rows;
            Pages = $"{paginationPageIndex + 1} из {CountPages + 1}";
        }
    }
}
