using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.Core
{
    public class CountToColorConverter : BaseValueConverter<CountToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (ColorValue colorValue in Configuration.GetConfiguration().ColorValues)
            {
                if ((int)value >= colorValue.DownValue && (int)value <= colorValue.UpValue)
                    return colorValue.Color;  
            }
            return Color.Transparent;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
