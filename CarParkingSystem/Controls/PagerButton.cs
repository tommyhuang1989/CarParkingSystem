using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Controls
{
    /// <summary>
    /// 分页工具上的按钮
    /// </summary>
    public class PagerButton : Button
    {
        //protected override Type StyleKeyOverride => typeof(Button);//需要返回是按钮
        //protected override Type StyleKeyOverride => base.StyleKeyOverride;

        public PagerButton()
        {
            //IsActiveProperty.Changed.AddClassHandler<PagerButton>(OnIsActiveChanged);
        }

        private void OnIsActiveChanged(PagerButton button, AvaloniaPropertyChangedEventArgs args)
        {
            button.Content += "Tommy test";
        }

        /// <summary>
        /// IsActive StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<PagerButton, bool>(nameof(IsActive), false);

        /// <summary>
        /// Gets or sets the IsActive property. This StyledProperty 
        /// indicates ....
        /// </summary>
        public bool IsActive
        {
            get => this.GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }


    }
}
