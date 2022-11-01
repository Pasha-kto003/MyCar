using ModelsApi;
using MyCar.Desktop.Core;
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

        private List<UserApi> searchResult;
        private List<UserApi> FullUsers;

        public List<UserTypeApi> UserTypes { get; set; } = new List<UserTypeApi>();
        public List<PassportApi> Passports { get; set; } = new List<PassportApi>();
        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public UserApi SelectedUser { get; set; }   

        public CustomCommand SearchStart { get; set; }

        public UserViewModel()
        {
            Task.Run(GetUserList);
 
            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Логин", "Фамилия", "Email", "Тип", "Отменить" });
            selectedSearchType = SearchType.First();

            Search();

            //SearchStart = new CustomCommand(() =>
            //{
            //    if (SelectedSearchType == "Логин" && SearchText != "")
            //    {
            //        int i = 1;
            //        Search(i, SearchText);
            //        Users = searchResult;
            //        if (Users == null)
            //        {
            //            MessageBox.Show("Пользователь не найден");
            //            SearchText = "";
            //            SignalChanged("SearchText");
            //            GetUserList();
            //            Users = FullUsers;
            //            SignalChanged("Users");
            //        }
            //    }
            //    else if (SelectedSearchType == "Фамилия")
            //    {
            //        int i = 3;
            //        Search(i, SearchText);
            //        Users = searchResult;
            //        if (Users == null)
            //        {
            //            MessageBox.Show("Пользователь не найден");
            //            SearchText = "";
            //            SignalChanged("SearchText");
            //            GetUserList();
            //            Users = FullUsers;
            //            SignalChanged("Users");
            //        }
            //    }
            //    else if (SelectedSearchType == "Email")
            //    {
            //        int i = 5;
            //        Search(i, SearchText);
            //        Users = searchResult;
            //        if (Users == null)
            //        {
            //            MessageBox.Show("Пользователь не найден");
            //            SearchText = "";
            //            SignalChanged("SearchText");
            //            GetUserList();
            //            Users = FullUsers;
            //            SignalChanged("Users");
            //        }
            //    }
            //});

        }

        //public async Task Search(int id, string? text)
        //{
        //    if (SelectedSearchType == "Логин")
        //    {
        //        id = 1;
        //        var usersSearch = await Api.SearchAsync<List<UserApi>>(id, text, "User");
        //        searchResult = usersSearch;
        //        Users = searchResult;
        //        SignalChanged("Users");
        //    }
        //    else if (SelectedSearchType == "Фамилия")
        //    {
        //        id = 3;
        //        var usersSearch = await Api.SearchAsync<List<UserApi>>(id, text, "User");
        //        searchResult = usersSearch;
        //        Users = searchResult;
        //        SignalChanged("Users");
        //    }
        //    else if (SelectedSearchType == "Email")
        //    {
        //        id = 5;
        //        var usersSearch = await Api.SearchAsync<List<UserApi>>(id, text, "User");
        //        searchResult = usersSearch;
        //        Users = searchResult;
        //        SignalChanged("Users");
        //    }
        //    else if (SelectedSearchType == "Тип")
        //    {
        //        id = 3;
        //        var usersSearch = await Api.SearchAsync<List<UserApi>>(id, text, "User");
        //        searchResult = usersSearch;
        //        Users = searchResult;
        //        SignalChanged("Users");
        //    }
        //    else if (SelectedSearchType == "Отменить")
        //    {
        //        var users = await Api.GetListAsync<List<UserApi>>("User");
        //        searchResult = users;
        //        Users = searchResult;
        //        SignalChanged("Users");
        //    }
        //}

        public async Task Search()
        {
            var search = SearchText.ToLower();
            searchResult = await Api.SearchAsync<List<UserApi>>(SelectedSearchType, search, "User");
            UpdateList();

        }

        private void UpdateList()
        {
            Users = searchResult;
        }

        private async Task GetUserList()
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            UserTypes = await Api.GetListAsync<List<UserTypeApi>>("UserType");
            Passports = await Api.GetListAsync<List<PassportApi>>("Passport");
            FullUsers = Users;
            foreach(var user in Users)
            {
                user.UserType = UserTypes.FirstOrDefault(s => s.ID == user.UserTypeId);
                user.Passport = Passports.FirstOrDefault(s => s.ID == user.PassportId);
            }
            SignalChanged(nameof(Users));
        }

    }
}
