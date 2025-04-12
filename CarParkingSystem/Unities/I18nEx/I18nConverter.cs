using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Data.Converters;
using AvaloniaExtensions.Axaml.Markup;

// ReSharper disable once CheckNamespace
namespace CarParkingSystem.Unities.I18nEx;

public class I18n2Converter(I18n2Binding owner) : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is not CultureInfo _)
        {
            return default;
        }

        var value = values[1];
        if (owner.KeyConverter.Convert(value, null, null, culture) is not string key)
        {
            return value;
        }

        //value = I18nManager.GetObject(key) ?? key;
        //if (value is string format)
        //{
        //    value = string.Format(format, owner.Args.Indexes
        //        .Select(item => item.IsBinding ? values[item.Index] : owner.Args[item.Index])
        //        .ToArray());
        //}

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 1; i < values.Count; i++)
        {
            var tmpKey = values[i].ToString();
            var tmpValue = I18nManager.GetObject(tmpKey) ?? key;
            stringBuilder.Append(tmpValue);
        }

        value = stringBuilder.ToString();

        return owner.ValueConverter.Convert(value, null, null, culture);
    }
}
