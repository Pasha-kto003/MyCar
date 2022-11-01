using ModelsApi;
using MyCar.Desktop.Core;
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
            }
        }

        private List<UserApi> searchResult;
        private List<UserApi> FullUsers;

        public List<UserTypeApi> UserTypes { get; set; } = new List<UserTypeApi>();
        public List<PassportApi> Passports { get; set; } = new List<PassportApi>();
        public List<UserApi> Users { get; set; } = new List<UserApi>();
        public UserApi SelectedUser { get; set; }   

       
        public CustomCommand AddUser { get; set; }

        public CustomCommand EditUser { get; set; }
        public UserViewModel()
        {
            Task.Run(GetUserList);
 
            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Логин", "Фамилия", "Email"});
            selectedSearchType = SearchType.First();

            AddUser = new CustomCommand(() =>
            {
                EditUser edituser = new EditUser();
                edituser.ShowDialog();
            });

            EditUser = new CustomCommand(() =>
            {
                if (SelectedUser == null || SelectedUser.ID == 0) return;
                EditUser edituser = new EditUser(SelectedUser);
                edituser.ShowDialog();
            });
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if(search == "")
                searchResult = await Api.GetListAsync<List<UserApi>>("User");
            else
                searchResult = await Api.SearchAsync<List<UserApi>>(SelectedSearchType, search, "User");
            UpdateList();
        }
        private void UpdateList()
        {
            Users = searchResult;
            SignalChanged(nameof(Users));
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
            }
            SignalChanged(nameof(Users));
        }

    }
}
