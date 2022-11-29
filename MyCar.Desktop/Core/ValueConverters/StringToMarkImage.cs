using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyCar.Desktop.Core
{
    public class StringToMarkImage : BaseValueConverter<StringToMarkImage>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultPath = Environment.CurrentDirectory + "/MarkImages/";
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
