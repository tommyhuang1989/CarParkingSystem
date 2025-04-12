using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Converters
{
    public class DateTimeFormatConverter : IValueConverter
    {

        //public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        //{
        //    return value is DateTimeOffset date ? date.ToString("yyyy-MM-dd") : null;
        //}


        //public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        //{
        //    return DateTimeOffset.TryParse((string?)value, out var date) ? date : null;
        //}
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is DateTime dateTime ? dateTime.ToString("yyyy-MM-dd HH:mm:ss") : null;
        }
    }
}
