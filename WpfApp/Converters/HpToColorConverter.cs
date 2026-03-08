using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class HpToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return System.Windows.Media.Brushes.Green;
            if (!int.TryParse(values[0]?.ToString(), out int current)) return System.Windows.Media.Brushes.Green;
            if (!int.TryParse(values[1]?.ToString(), out int max) || max == 0) return System.Windows.Media.Brushes.Green;

            double ratio = (double)current / max;

            if (ratio > 0.6) return System.Windows.Media.Brushes.LimeGreen;
            if (ratio > 0.3) return System.Windows.Media.Brushes.Orange;
            return System.Windows.Media.Brushes.Crimson;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
