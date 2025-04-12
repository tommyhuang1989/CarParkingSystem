using Avalonia.Data;
using Avalonia.Data.Converters;
using AvaloniaExtensions.Axaml.Markup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Converters
{
    public class I18nAppendConverter : IValueConverter
    {
        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)return null;

            var key = value as string;
            string result = I18nManager.GetString(key);
            if (parameter != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(result);
                sb.Append(parameter.ToString());
                result = sb.ToString();
            }

            return result;
        }

        object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
