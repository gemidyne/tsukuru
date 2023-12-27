using System;
using System.Globalization;
using System.Windows.Data;

namespace Tsukuru.Converters;

public class BooleanToPackingModeConverter : IValueConverter
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
        string stringValue = value?.ToString() ?? string.Empty;
        
        return stringValue == "Only files used by the map";
    }
}