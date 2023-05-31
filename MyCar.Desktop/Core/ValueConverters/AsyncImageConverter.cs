using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    class AsyncImageConverter : BaseValueConverter<AsyncImageConverter>
    {
        private async Task<byte[]> GetImage(string imageName)
        {
            byte[] imageData = await Api.GetImage<byte[]>($"{imageName}", "Image");
            return imageData;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new AsyncTask(() =>
                {
                    return GetImage(value.ToString());
                });
            }
            else
            {
                return new AsyncTask(() =>
                {
                    return GetImage("picture.png");
                });
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public class AsyncTask : INotifyPropertyChanged
        {
            public AsyncTask(Func<Task<byte[]>> valueFunc)
            {
                AsyncValue = "loading async value"; 
                LoadValue(valueFunc);
            }

            private async void LoadValue(Func<Task<byte[]>> valueFunc)
            {
                AsyncValue = await Task.Run(valueFunc);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AsyncValue"));
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public object AsyncValue { get; set; }
        }
    }
}
