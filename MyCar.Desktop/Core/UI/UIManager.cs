using Microsoft.Win32;
using ModelsApi;
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
        public static List<Color> Colors = new List<Color>();
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

        public static async Task<BitmapImage> GetImageFromServer(string imageName)
        {
            byte[] imageData = await Api.GetImage<byte[]>($"{imageName}", "Image");

            BitmapImage img = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
            }
            return img;
        }

        public static BitmapImage GetImageFromByteArray(byte[] byteArray)
        {
            BitmapImage img = new BitmapImage();
            using (var ms = new MemoryStream(byteArray))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
            }
            return img;
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
        public static async Task<MethodResult> AddImageAsync()
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var file = new FileInfo(openFileDialog.FileName);
                    fileName = file.Name;
                    string savedFileName = await Api.SaveImage(file, "Image");
                }
                catch (Exception e)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                }
                return new MethodResult { IsSuccess = true, Data = fileName };
            }
            return new MethodResult { IsSuccess = false, Data = "" };
        }

            public static MethodResult AddImage(string dir)
            {
            string fileName = "";
            if (!Directory.Exists(Environment.CurrentDirectory + @$"\{dir}\"))
                Directory.CreateDirectory(Environment.CurrentDirectory + @$"\{dir}\");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + @$"\{dir}\";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var info = new FileInfo(openFileDialog.FileName);
                    fileName = info.Name;
                    var newPath = Environment.CurrentDirectory + @$"\{dir}\" + info.Name;
                    if (File.Exists(newPath))
                    {
                        MessageBoxDialogViewModel result = new MessageBoxDialogViewModel
                        { Title = "Подтверждение", Message = $"Файл с именем {info.Name} уже содержится в папке назначения\n                  Назначить уже существующий файл?" };
                        UIManager.ShowMessageYesNo(result);
                        if (!result.Result)
                        {
                            return new MethodResult { IsSuccess = false };
                        }
                    }
                    else
                    {
                        File.Copy(openFileDialog.FileName, newPath);
                    }
                }
                catch (Exception e)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                }
                return new MethodResult { IsSuccess = true, Data = fileName };
            }
            return new MethodResult { IsSuccess = false, Data = "" };
        }
    }
    public class MethodResult 
    {
        public bool IsSuccess { get; set; }   
        public object? Data { get; set; }
    }
}
