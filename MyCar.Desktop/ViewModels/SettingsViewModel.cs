using Microsoft.Win32;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ColorValue SelectedColorValue { get; set; }
        public ObservableCollection<ColorValue> ColorValues { get; set; }
        public CustomCommand Exit { get; set; }
        public CustomCommand SaveConfig { get; set; }
        public CustomCommand ExportConfig { get; set; }
        public CustomCommand ImportConfig { get; set; }
        public CustomCommand AddColorValue { get; set; }
        public CustomCommand RemoveColorValue { get; set; }

        public SettingsViewModel()
        {
            Update();
            Exit = new CustomCommand(() =>
            {
                AuthWindow authWindow = new AuthWindow();
                authWindow.Show(); 
                Configuration.CurrentUser = null;
                Configuration.CloseMainWindow();
            });
            SaveConfig = new CustomCommand(() =>
            {
                Configuration.SetConfiguration(new Config { ColorValues = this.ColorValues.ToList() });
                Update();
                UIManager.ShowMessage(new MessageBoxDialogViewModel { Message = "Сохранено." });
            });
            ExportConfig = new CustomCommand(() =>
            {
                Configuration.ExportConfig();
            });
            ImportConfig = new CustomCommand(() =>
            {
                Configuration.ImportConfig();
                Update();
            });
            AddColorValue = new CustomCommand(() =>
            {
                ColorValues.Add(new ColorValue { DownValue = 0, UpValue = 0});
            });
            RemoveColorValue = new CustomCommand(() =>
            {
                if (SelectedColorValue == null) return;
                ColorValues.Remove(SelectedColorValue);
            });
        }
        private void Update()
        {
            if (Configuration.GetConfiguration().ColorValues == null)
                ColorValues = new ObservableCollection<ColorValue>();
            else
             ColorValues = new ObservableCollection<ColorValue>(Configuration.GetConfiguration().ColorValues);
        }
    }
}
