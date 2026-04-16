using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class OrderProductsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ops = value as ICollection<OrderProducts>;
            if (ops == null) return "";
            return string.Join(", ", ops.Select(op => $"{op.Products?.Name} x{op.Quantity}"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
