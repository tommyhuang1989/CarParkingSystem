using Avalonia.Data;
using Avalonia.Data.Converters;
using AvaloniaExtensions.Axaml.Converters;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.I18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities.I18nEx
{
    public class I18n2Binding : MultiBindingExtensionBase
    {
        public I18n2Binding(List<object> keys) 
        {
            Mode = BindingMode.OneWay;
            Converter = new I18n2Converter(this);
            KeyConverter = new I18nKeyConverter();
            ValueConverter = new I18nValueConverter();
            Args = new ArgCollection(this);

            var cultureBinding = new Binding { Source = I18nManager.Instance, Path = nameof(I18nManager.Culture) };
            Bindings.Add(cultureBinding);

            //Key = key;
            //if (Key is not BindingBase keyBinding)
            //{
            //    keyBinding = new Binding { Source = key };
            //}

            Keys = keys;
            foreach (var key in Keys)
            {
                if (key is not BindingBase keyBinding)
                {
                    keyBinding = new Binding { Source = key };
                    Bindings.Add(keyBinding);
                }
            }

        }

        public I18n2Binding(List<object> keys, List<object> args) : this(keys)
        {
            if (args is not { Count: > 0 })
            {
                return;
            }

            foreach (object arg in args)
            {
                Args.Add(arg);
            }
        }

        //public object Key { get; }
        public List<object> Keys { get; }

        public ArgCollection Args { get; }

        public IValueConverter KeyConverter { get; set; }

        public IValueConverter ValueConverter { get; set; }
    }

}
