using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public CustomCommand Exit { get; set; }

        public SettingsViewModel()
        {
            Exit = new CustomCommand(() =>
            {
                AuthWindow authWindow = new AuthWindow();
                authWindow.Show(); 
                Configuration.CurrentUser = null;
                Configuration.CloseMainWindow();
            });
        }
    }
}
