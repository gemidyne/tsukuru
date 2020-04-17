using System;
using System.Globalization;
using System.Windows.Data;

namespace Tsukuru.Converters
{
    internal class BooleanToPackingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as bool?;

            if (flag.GetValueOrDefault())
            {
                return "Only files used by the map";
            }
            else
            {
                return "All files";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, "Only used files by the map");
        }
    }
}