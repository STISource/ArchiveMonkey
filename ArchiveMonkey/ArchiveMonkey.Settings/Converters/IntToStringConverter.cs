using System;
using System.Globalization;
using System.Windows.Data;

namespace ArchiveMonkey.Settings.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                return ((int)value).ToString();
            }
            else if (value is int?)
            {
                var intValue = (int?)value;
                return intValue.HasValue ? intValue.Value.ToString() : string.Empty;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string)
            {
                var stringValue = (string)value;
                return string.IsNullOrWhiteSpace(stringValue) ? (int?)null : int.Parse(stringValue);
            }

            return null;
        }
    }
}
