using System;
using System.Globalization;
using System.Windows.Data;

namespace Tsukuru.Converters
{
	internal class WatchButtonTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var flag = value as bool?;

			if (flag.GetValueOrDefault())
			{
				return "Stop watching";
			}
			else
			{
				return "Watch";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
