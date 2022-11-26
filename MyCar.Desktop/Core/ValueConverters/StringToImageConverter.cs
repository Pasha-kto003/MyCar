using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core.ValueConverters
{
    public class StringToImageConverter : BaseValueConverter<StringToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultPath = Environment.CurrentDirectory + "/CarImages/";
            if (File.Exists(defaultPath + value))
                return UIManager.GetImageFromPath(defaultPath + value);
            else
                return UIManager.GetImageFromPath(defaultPath + "picture.png");
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
