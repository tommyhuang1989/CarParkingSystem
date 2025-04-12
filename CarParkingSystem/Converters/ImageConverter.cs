using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                //var val = value.ToString().Split(" ");
                //return $"/Assets/Images/flags/{val[1].ToString()}.png";

                var val = value.ToString().Trim();
                //var result = $"/Assets/Images/flags/{val.ToString()}.png"; //Source="/Assets/Images/flags/cn.png"
                //return result;

                var uri = new Uri($"avares://CarParkingSystem/Assets/Images/flags/{val}.png");
                var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MenuImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return $"/Assets/Images/menus/{value.ToString()}.png";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
