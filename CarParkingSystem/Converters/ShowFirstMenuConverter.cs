using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CarParkingSystem.ViewModels;
using Material.Icons;

namespace CarParkingSystem.Converters;

public class ShowFirstMenuConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DemoPageBase demoPage = (DemoPageBase)value;
        if (demoPage.Pid == 0 && demoPage.SubPages.Count == 0)
        {
            if(demoPage.ShowEvenOneFoor)
            {
                return true;
            }
            return false;
        }

        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}