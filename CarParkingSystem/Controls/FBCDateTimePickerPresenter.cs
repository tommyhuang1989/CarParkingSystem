using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Controls
{
    /// <summary>
    /// FBC: My references
    /// https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/DateTimePickers/DatePicker.cs
    /// https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Themes.Default/DatePicker.xaml
    /// https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/DateTimePickers/DatePickerPresenter.cs
    /// </summary>
    public class FBCDateTimePickerPresenter : PickerPresenterBase
    {
        public FBCDateTimePickerPresenter() : base()
        {
            Value = DateTime.Now;
        }

        public static readonly DirectProperty<FBCDateTimePickerPresenter, DateTime> DateOnlyProperty =
            AvaloniaProperty.RegisterDirect<FBCDateTimePickerPresenter, DateTime>(nameof(DateOnly),
                x => x.DateOnly, (x, v) => x.DateOnly = v);

        public static readonly DirectProperty<FBCDateTimePickerPresenter, int> HourProperty =
            AvaloniaProperty.RegisterDirect<FBCDateTimePickerPresenter, int>(nameof(Hour),
                x => x.Hour, (x, v) => x.Hour = v);

        public static readonly DirectProperty<FBCDateTimePickerPresenter, int> MinuteProperty =
            AvaloniaProperty.RegisterDirect<FBCDateTimePickerPresenter, int>(nameof(Minute),
                x => x.Minute, (x, v) => x.Minute = v);

        public static readonly DirectProperty<FBCDateTimePickerPresenter, int> SecondProperty =
            AvaloniaProperty.RegisterDirect<FBCDateTimePickerPresenter, int>(nameof(Second),
                x => x.Second, (x, v) => x.Second = v);


        private DateTime dateOnly;
        private int hour;
        private int minute;
        private int second;

        public DateTime DateOnly
        {
            get { return dateOnly; }
            set
            {
                SetAndRaise(DateOnlyProperty, ref dateOnly, value);
            }
        }

        public int Hour
        {
            get { return hour; }
            set
            {
                SetAndRaise(HourProperty, ref hour, value);
            }
        }
        public int Minute
        {
            get { return minute; }
            set
            {
                minute = value;
                SetAndRaise(MinuteProperty, ref minute, value);
            }
        }
        public int Second
        {
            get { return second; }
            set
            {
                second = value;
                SetAndRaise(SecondProperty, ref second, value);
            }
        }
        NumericUpDown nuHour;
        NumericUpDown nuMinute;
        NumericUpDown nuSecond;
        Button BtnOK;
        Button BtnCancel;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            nuHour = e.NameScope.Get<NumericUpDown>("Hour");
            nuMinute = e.NameScope.Get<NumericUpDown>("Minute");
            nuSecond = e.NameScope.Get<NumericUpDown>("Second");
            BtnOK = e.NameScope.Get<Button>("BtnOK");
            BtnCancel = e.NameScope.Get<Button>("BtnCancel");

            if (BtnOK != null)
            {
                BtnOK.Click += OnAcceptButtonClicked;
            }
            if (BtnCancel != null)
            {
                BtnCancel.Click += OnDismissButtonClicked;
            }

        }

        private void OnDismissButtonClicked(object? sender, RoutedEventArgs e)
        {
            OnDismiss();
        }

        private void OnAcceptButtonClicked(object? sender, RoutedEventArgs e)
        {
            //Date = _syncDate;
            OnConfirmed();
        }

        public DateTime Value
        {
            get { return new DateTime(DateOnly.Year, DateOnly.Month, DateOnly.Day, Hour, Minute, Second); }
            set
            {
                DateOnly = value.Date;
                Hour = value.Hour;
                Minute = value.Minute;
                Second = value.Second;
                //OnPropertyChanged();
            }
        }

    }
}
