using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Chiaki;
using Tsukuru.Settings;

namespace Tsukuru.Converters
{
    internal class MapVersioningSettingsVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EMapVersionMode val = value.As<EMapVersionMode>();
            bool invert = parameter == null ? false : bool.Parse(parameter.ToString());

            bool condition = val == EMapVersionMode.VersionedBuildNumber || val == EMapVersionMode.VersionedDateTime;

            if (invert)
            {
                condition = !condition;
            }

            return condition
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}