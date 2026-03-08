using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class HpToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return 0.0;
            if (!int.TryParse(values[0]?.ToString(), out int current)) return 0.0;
            if (!int.TryParse(values[1]?.ToString(), out int max) || max == 0) return 0.0;

            double maxWidth = parameter != null && double.TryParse(parameter.ToString(), out double p) ? p : 200.0;
            return Math.Max(0.0, (double)current / max * maxWidth);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
