﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private Window mWindow;


        public bool LoginIsRunning { get; set; } = false;
        public string Password { get; set; }
        public string ErrorPassword { get; set; }

        public string UserName { get; set; }
        public string LoginError { get; set; }

        public UserApi User { get; set; }
        private int ErrorCount { get; set; }

        public CustomCommand CloseWindow { get; set; }
        public CustomCommand Login { get; set; }
        public CustomCommand Registration { get; set; }

        private PasswordBox PasswordBox;

        public AuthViewModel(Window window, PasswordBox passwordtxt)
        {
            mWindow = window;
            PasswordBox = passwordtxt;
            CloseWindow = new CustomCommand(() => {
                mWindow.Close();
            });
            
            

            Login = new CustomCommand(async()  => {
                await RunCommandAsync(() => this.LoginIsRunning, async () =>
                {
                    Password = passwordtxt.Password;
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

        private void ShowWindow()
        {
            if (User != null && User.ID != 0 && (User.UserType.TypeName == "Сотрудник" || User.UserType.TypeName == "Администратор"))
            {
                Configuration.CurrentUser = User;
                MainWindow testWindow = new MainWindow();
                testWindow.Show();
                mWindow.Close();
            }
            else
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Неправильный логин или пароль!" });
                PasswordBox.Password = "";
                UserName = "";
            }    
        }
    }
}
