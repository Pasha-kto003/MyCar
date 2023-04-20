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
    public class MultiCountToColorConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#00FFFFFF");
            SolidColorBrush transparent = new SolidColorBrush(color);

            if (values[0] == null || values[1] == null)
                return transparent;
            if (!double.TryParse(values[0].ToString(), out var count) || !double.TryParse(values[1].ToString(), out var мinCount))
                return transparent;
            double koef = (double)мinCount / (double)count;
            double result = 100 / koef;
            foreach (ColorValue colorValue in Configuration.GetConfiguration().ColorValues)
            {
                if ((int)result >= colorValue.DownValue && (int)result <= colorValue.UpValue)
                {
                    Color color1 = (Color)ColorConverter.ConvertFromString(colorValue.Color);
                    SolidColorBrush brush = new SolidColorBrush(color1);
                    return brush;
                }
            }
            return transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
