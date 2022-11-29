using MyCar.Desktop.Controls;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyCar.Desktop.Core.UI
{
    public static class UIManager
    {
        public static Task ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await new DialogMessageBox().ShowDialog(viewModel);
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }
        public static Task ShowErrorMessage(MessageBoxDialogViewModel viewModel)
        {
            var tcs = new TaskCompletionSource<bool>();
            viewModel.Title = "Oшибка!";
            viewModel.OkText = "OK";

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await new DialogMessageBox().ShowDialog(viewModel);
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }
        public static Task ShowMessageYesNo(MessageBoxDialogViewModel viewModel)
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await new YesNoMessageBox().ShowDialog(viewModel);
                }
                finally
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }

        public static void CloseWindow(object obj)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == obj)
                {
                    window.Close();
                }
            }
        }

        public static BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = fs;
                img.EndInit();
            }
            return img;
        }
    }
}
