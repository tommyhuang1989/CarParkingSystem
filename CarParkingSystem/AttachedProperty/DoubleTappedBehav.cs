using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarParkingSystem.AttachedProperty
{
    /// <summary>
    /// 附加属性的容器类。必须继承自<see cref="AvaloniaObject"/>。
    /// 添加双击就出发命令的附加属性
    /// </summary>
    public class DoubleTappedBehav : AvaloniaObject
    {
        static DoubleTappedBehav()
        {
            CommandProperty.Changed.AddClassHandler<Interactive>(HandleCommandChanged);
        }

        /// <summary>
        /// 标识<seealso cref="CommandProperty"/> avalonia附加属性。
        /// </summary>
        /// <value>提供一个派生自<see cref="ICommand"/>的对象或绑定。</value>
        public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<DoubleTappedBehav, Interactive, ICommand>(
            "Command", default(ICommand), false, BindingMode.OneTime);

        /// <summary>
        /// 标识<seealso cref="CommandParameterProperty"/> avalonia附加属性。
        /// 用作<see cref="CommandProperty"/>的参数。
        /// </summary>
        /// <value>任何类型为<see cref="object"/>的值。</value>
        public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<DoubleTappedBehav, Interactive, object>(
            "CommandParameter", default(object), false, BindingMode.OneWay, null);


        /// <summary>
        /// <see cref="CommandProperty"/>的变化事件处理程序。
        /// </summary>
        private static void HandleCommandChanged(Interactive interactElem, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.NewValue is ICommand commandValue)
            {
                // 添加非空值
                interactElem.AddHandler(InputElement.DoubleTappedEvent, Handler);
            }
            else
            {
                // 删除之前的值
                interactElem.RemoveHandler(InputElement.DoubleTappedEvent, Handler);
            }
            // 本地处理函数
            static void Handler(object s, RoutedEventArgs e)
            {
                if (s is Interactive interactElem)
                {
                    // 这是如何从GUI元素中获取参数的方法。
                    object commandParameter = interactElem.GetValue(CommandParameterProperty);
                    ICommand commandValue = interactElem.GetValue(CommandProperty);
                    if (commandValue?.CanExecute(commandParameter) == true)
                    {
                        commandValue.Execute(commandParameter);
                    }
                }
            }
        }


        /// <summary>
        /// 附加属性<see cref="CommandProperty"/>的访问器。
        /// </summary>
        public static void SetCommand(AvaloniaObject element, ICommand commandValue)
        {
            element.SetValue(CommandProperty, commandValue);
        }

        /// <summary>
        /// 附加属性<see cref="CommandProperty"/>的访问器。
        /// </summary>
        public static ICommand GetCommand(AvaloniaObject element)
        {
            return element.GetValue(CommandProperty);
        }

        /// <summary>
        /// 附加属性<see cref="CommandParameterProperty"/>的访问器。
        /// </summary>
        public static void SetCommandParameter(AvaloniaObject element, object parameter)
        {
            element.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// 附加属性<see cref="CommandParameterProperty"/>的访问器。
        /// </summary>
        public static object GetCommandParameter(AvaloniaObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }
    }
}
