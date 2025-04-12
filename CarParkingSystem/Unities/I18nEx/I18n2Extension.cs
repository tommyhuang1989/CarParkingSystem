using Avalonia.Markup.Xaml;
using AvaloniaExtensions.Axaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities.I18nEx
{
    public class I18n2Extension : MarkupExtension
    {
        public I18n2Extension(object key)
        {
            //Key = key;
            Keys.Add(key);
        }
        public I18n2Extension(object key0, object key1)
        {
            Keys.Add(key0);
            Keys.Add(key1);
        }
        public I18n2Extension(object key0, object key1, object key2)
        {
            Keys.Add(key0);
            Keys.Add(key1);
            Keys.Add(key2);
        }

        //public I18n2Extension(object key, object arg0) : this(key)
        //{
        //    Args.Add(arg0);
        //}

        //public I18n2Extension(object key, object arg0, object arg1) : this(key)
        //{
        //    Args.Add(arg0);
        //    Args.Add(arg1);
        //}

        public I18n2Extension(object key, object arg0, object arg1, object arg2) : this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
        }

        public I18n2Extension(object key, object arg0, object arg1, object arg2, object arg3) : this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
            Args.Add(arg3);
        }

        public I18n2Extension(object key, object arg0, object arg1, object arg2, object arg3, object arg4) : this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
            Args.Add(arg3);
            Args.Add(arg4);
        }

        public I18n2Extension(object key, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5) :
            this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
            Args.Add(arg3);
            Args.Add(arg4);
            Args.Add(arg5);
        }

        public I18n2Extension(object key, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5,
            object arg6) : this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
            Args.Add(arg3);
            Args.Add(arg4);
            Args.Add(arg5);
            Args.Add(arg6);
        }

        public I18n2Extension(object key, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5,
            object arg6, object arg7) : this(key)
        {
            Args.Add(arg0);
            Args.Add(arg1);
            Args.Add(arg2);
            Args.Add(arg3);
            Args.Add(arg4);
            Args.Add(arg5);
            Args.Add(arg6);
            Args.Add(arg7);
        }

        //public object Key { get; }
        public List<object> Keys { get; } = new();
        public List<object> Args { get; } = new();
        public override object ProvideValue(IServiceProvider serviceProvider) => new I18n2Binding(Keys, Args);
    }
}