using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class AddUserViewModel : BaseViewModel
    {
        public UserTypeApi SelectedUserType { get; set; }

        public List<UserTypeApi> UserTypes { get; set; }
        public List<UserApi> Users { get; set; }

        public UserApi EditUser { get; set; }
        public PassportApi EditPassport { get; set; }

        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }

        public string OldUserName = "";

        public AddUserViewModel(UserApi editUser)
        {
            Task.Run(GetList);

            if (editUser == null)
            {
                EditUser = new UserApi { UserTypeId = 1 };
                EditUser.Passport = new PassportApi();
            }
            else
            {
                OldUserName = editUser.UserName;
                EditUser = new UserApi
                {
                    ID = editUser.ID,
                    PassportId = editUser.PassportId,
                    Email = editUser.Email,
                    UserName = editUser.UserName,
                    UserTypeId = editUser.UserTypeId,
                    PasswordHash = editUser.PasswordHash,
                    SaltHash = editUser.SaltHash,
                    Passport = editUser.Passport,
                    UserType = editUser.UserType
                };
            }

            Save = new CustomCommand(async () =>
            {
                if (SelectedUserType == null || SelectedUserType.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран тип пользователя!" });
                    return;
                }
                if (EditUser.UserName == null || EditUser.UserName == "")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Логин - обязательное поле!" });
                    return;
                }
                if (EditUser.UserName != OldUserName & !CheckUniqueUserName(EditUser))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Пользователь с таким логином уже существует!" });
                    return;
                }
                EditUser.UserType = SelectedUserType;
                EditUser.UserTypeId = SelectedUserType.ID;
                if (EditUser.ID == 0)
                {
                    await CreateUser(EditUser);
                }
                else
                {
                    await ChangeUser(EditUser, EditUser.Passport);
                } 
                UIManager.CloseWindow(this);
            });

            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }  

        private async Task CreateUser(UserApi userApi)
        {
            userApi.Passport = EditUser.Passport;
            var user = await Api.PostAsync<UserApi>(userApi, "User");
        }

        private async Task ChangeUser(UserApi userApi, PassportApi passportapi)
        {
            var user = await Api.PutAsync<UserApi>(userApi, "User");
            var passport = await Api.PutAsync<PassportApi>(passportapi, "Passport");
        }

        private bool CheckUniqueUserName(UserApi user)
        {
            if (Users.Exists(s=>s.UserName == user.UserName))
                return false;
            else
                return true;
        }

        private async Task GetList()
        {
            Users = await Api.GetListAsync<List<UserApi>>("User");
            UserTypes = await Api.GetListAsync<List<UserTypeApi>>("UserType");
            SelectedUserType = UserTypes.FirstOrDefault(s => s.ID == EditUser.UserTypeId);
            SignalChanged(nameof(UserTypes));
            SignalChanged(nameof(SelectedUserType));
        }
    }
}
