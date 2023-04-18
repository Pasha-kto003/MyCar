using ModelsApi;
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
    public class StringToColorConverter : BaseValueConverter<StringToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = UIManager.Colors.FirstOrDefault(s => s.Name == value.ToString());
            if (color != null)
                return color.HexCode;
            return "#FFFFFF";
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
