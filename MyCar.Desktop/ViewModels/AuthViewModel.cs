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

        public string UserName { get; set; }

        public UserApi User { get; set; }
        public CustomCommand CloseWindow { get ; set; }
        public CustomCommand Login { get; set; }

        public AuthViewModel(Window window)
        {
            mWindow = window;


            CloseWindow = new CustomCommand(() => {
                mWindow.Close();
            });


            Login = new CustomCommand(() => {
                Task.Run(Enter);
                if(User != null)
                {
                    MainWindow testWindow = new MainWindow();
                    testWindow.ShowDialog();
                }
                
            });
        }

        private async Task Enter()
        {
            User = await Api.Enter<UserApi>(UserName, Password, "Auth");
            
        }

    }
}
