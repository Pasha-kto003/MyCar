using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private Window mWindow;

        public string Password { get; set; }
        public string ErrorPassword { get; set; }

        public string UserName { get; set; }
        public string LoginError { get; set; }

        public UserApi User { get; set; }
        private int ErrorCount { get; set; }

        public CustomCommand CloseWindow { get ; set; }
        public CustomCommand Login { get; set; }
        public CustomCommand Registration { get; set; }

        public AuthViewModel(Window window)
        {
            mWindow = window;


            CloseWindow = new CustomCommand(() => {
                mWindow.Close();
            });

            Registration = new CustomCommand(() =>
            {
                RigistrationWindow rigistration = new RigistrationWindow();
                rigistration.ShowDialog();
            });

            Login = new CustomCommand( async() => {
                if (UserName == "" || UserName == null)
                {
                    LoginError = "Вы не ввели никнейм пользователя!";
                    SignalChanged(nameof(LoginError));
                }

                if(Password == "" || Password == null)
                {
                    ErrorPassword = "Вы не ввели пароль пользователя!";
                    SignalChanged(nameof(ErrorPassword));
                }

                Task.Run(Enter);

                if (User == null)
                {
                    LoginError = $"Пользователя {UserName} не существует";
                    SignalChanged(nameof(LoginError));
                    ErrorCount++;
                    if(ErrorCount == 3)
                    {
                        WaitWindow waitWindow = new WaitWindow();
                        waitWindow.ShowDialog();
                        mWindow.Close();
                    }

                    SignalChanged(nameof(UserName));
                    SignalChanged(nameof(Password));
                    return;
                }

                if (User.UserTypeId == 2)
                {
                    MainWindow testWindow = new MainWindow(User);
                    testWindow.ShowDialog();
                    mWindow.Close();
                }
            });
        }

        private async Task Enter()
        {
            User = await Api.Enter<UserApi>(UserName, Password, "Auth");
            
        }

    }
}
