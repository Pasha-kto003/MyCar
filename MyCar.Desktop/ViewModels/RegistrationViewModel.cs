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
    public class RegistrationViewModel : BaseViewModel
    {
        private Window mWindow;

        public string Password { get; set; }

        public string UserName { get; set; }

        public string RepeatPassword { get; set; }

        public UserApi User { get; set; }

        public CustomCommand CloseWindow { get; set; }
        public CustomCommand Registration { get; set; }

        public RegistrationViewModel(Window window)
        {
            mWindow = window;


            CloseWindow = new CustomCommand(() => {
                mWindow.Close();
            });

            Registration = new CustomCommand(() =>
            {
                if(RepeatPassword != Password)
                {
                    MessageBox.Show("Не верно введен пароль");
                    RepeatPassword = "";
                    SignalChanged(nameof(RepeatPassword));
                }
                RegistrationUser();
            });

        }

        public async Task RegistrationUser()
        {
            User = await Api.RegistrationAsync<UserApi>(User, "Auth");
        }
    }
}
