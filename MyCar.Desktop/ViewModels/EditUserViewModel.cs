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
    public class EditUserViewModel : BaseViewModel
    {
        public UserTypeApi SelectedUserType { get; set; }

        public List<UserTypeApi> UserTypes { get; set; }

        public UserApi EditUser { get; set; }


        public CustomCommand Cancel { get; set; }

        public EditUserViewModel(UserApi editUser)
        {
            Task.Run(GetList);

            if (editUser == null)
            {
                EditUser = new UserApi {UserTypeId = 1 };
            }
            else
            {
                EditUser = new UserApi
                {
                   ID = editUser.ID,
                   PassportId = editUser.PassportId,
                   Email = editUser.Email,
                   UserName = editUser.UserName,
                   UserTypeId = editUser.UserTypeId,
                   SaltHash = editUser.SaltHash,
                   PasswordHash = editUser.PasswordHash,
                   Passport = editUser.Passport,
                   UserType = editUser.UserType
                };

               
            }

            Cancel = new CustomCommand(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                    }
                }
            });
        }  

        private async Task GetList()
        {
            UserTypes = await Api.GetListAsync<List<UserTypeApi>>("UserType");
            SelectedUserType = UserTypes.FirstOrDefault(s => s.ID == EditUser.UserTypeId);
            SignalChanged(nameof(UserTypes));
            SignalChanged(nameof(SelectedUserType));
        }
    }
}
