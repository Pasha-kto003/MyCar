using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public bool LoginIsRunning { get; set; } = false;
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


            Login = new CustomCommand(async () => {

                await RunCommandAsync(() => this.LoginIsRunning, async () =>
                {
                    Task task = Task.Run(Enter);
                    await task;
                    ShowWindow();
                });

            });
        }

        private async Task Enter()
        {
            User = await Api.Enter<UserApi>(UserName, Password, "Auth");
        }
        private async Task LogIn()
        {

            await RunCommandAsync(() => this.LoginIsRunning, async () =>
            {
               
                Task task = Task.Run(Enter);
                task.Wait();
                ShowWindow();
               
            });
        }
        private void ShowWindow()
        {
            if (User != null && User.ID != 0)
            {
                 MainWindow testWindow = new MainWindow(User);
                 testWindow.Show();
                 mWindow.Close();
            }
        }

    }
}
