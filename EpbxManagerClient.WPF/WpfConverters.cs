using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EpbxManagerClient
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    internal class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(System.Windows.Visibility)) throw new InvalidOperationException("The target must be a boolean or Visibility");

            if (targetType == typeof(bool))
                return !(bool)value;
            else
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
