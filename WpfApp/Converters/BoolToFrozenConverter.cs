using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class BoolToFrozenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? "заблокирован" : "активен";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
