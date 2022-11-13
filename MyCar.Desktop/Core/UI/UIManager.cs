using MyCar.Desktop.Controls;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
