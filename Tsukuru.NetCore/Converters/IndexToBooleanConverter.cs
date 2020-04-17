using System;
using System.Globalization;
using System.Windows.Data;

namespace Tsukuru.Converters
{
    internal class IndexToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as int?;

            return flag.GetValueOrDefault(0) == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as bool?;

            return flag.GetValueOrDefault() ? 1 : 0;
        }
    }
}