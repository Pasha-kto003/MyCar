using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MyCar.Desktop.Core
{
    public class StringToImageConverter : BaseValueConverter<StringToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultPath = Environment.CurrentDirectory + "/CarImages/";
            string path = defaultPath;
            if (parameter != null)
            {
                if(Directory.Exists(Environment.CurrentDirectory + $"/{parameter.ToString()}/"))
                    path = Environment.CurrentDirectory + $"/{parameter.ToString()}/";
            }

            if (File.Exists(path + value))
                return UIManager.GetImageFromPath(path + value);
            else
                return UIManager.GetImageFromPath(defaultPath + "picture.png");
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
