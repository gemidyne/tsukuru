using System;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Tsukuru.Converters;

internal class DirectoryInfoNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
        {
            return DependencyProperty.UnsetValue;
        }

        string directory = value.ToString();

        if (string.IsNullOrWhiteSpace(directory))
        {
            return "(unknown)";
        }

        var directoryInfo = new DirectoryInfo(directory);

        return directoryInfo.Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return null;
    }
}