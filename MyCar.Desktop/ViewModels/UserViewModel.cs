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

        public UserViewModel()
        {
            Task.Run(GetUserList);
 
            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Логин", "Фамилия", "Email", "Тип", "Отменить" });
            selectedSearchType = SearchType.First();

            Search();    
        }

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
            SignalChanged(nameof(Users));
        }

    }
}
