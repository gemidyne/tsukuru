using System;
using System.Globalization;
using System.Windows.Data;

namespace Tsukuru.Converters
{
    internal class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as bool?;

            return !flag.GetValueOrDefault();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as bool?;

            return flag.GetValueOrDefault();
        }
    }
}