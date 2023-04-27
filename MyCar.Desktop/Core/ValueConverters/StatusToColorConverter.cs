using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace MyCar.Desktop.Core
{
    public class StatusToColorConverter : BaseValueConverter<StatusToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#00FFFFFF");
            SolidColorBrush transparent = new SolidColorBrush(color);

            Color color1 = (Color)ColorConverter.ConvertFromString(Configuration.GetConfiguration().OrderColors.First(s => s.Status == value.ToString()).Color);
            if (color1 != null)
            {
                return new SolidColorBrush(color1);
            }
            return transparent;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
