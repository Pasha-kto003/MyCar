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
    public class MultiDiscountDateToColorConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#00FFFFFF");
            SolidColorBrush transparent = new SolidColorBrush(color);

            if (values[0] == null || values[1] == null)
                return transparent;
            DateTime start = (DateTime)values[0];
            DateTime end = (DateTime)values[1];
            if (start <= DateTime.Now && end >= DateTime.Now)
            {
                color = (Color)ColorConverter.ConvertFromString("#8bc34a");
                SolidColorBrush brush = new SolidColorBrush(color);
                return brush;
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
