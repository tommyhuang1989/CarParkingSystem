using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CarParkingSystem.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;
using System.Threading.Tasks;
using RegularExpressionAttribute = CarParkingSystem.I18n.RegularExpressionAttribute;
using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;
using Avalonia.Collections;
using CarParkingSystem.Controls;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为新增、修改信息界面提供数据的 ViewModel
    /// </summary>
    public partial class CalendarActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private CalendarDao _calendarDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        public AvaloniaList<ComboItem> _comboBoxItems { get; } = [];

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddCalendar;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private Calendar _selectedCalendar;

        [Required(StringResourceKey.CalendarNameRequired)]
        [ObservableProperty]
        private System.String _calendarName;
        [ObservableProperty]
        private System.String _endDate;
        [ObservableProperty]
        [Required(StringResourceKey.ParkingTimeRequired)]
        private System.String _parkingDate;
        [ObservableProperty]
        private System.String _startDate;

        //[ObservableProperty]
        //private System.Int32 _isHoliday;
        private System.Int32 _isHoliday;

        //[Required(StringResourceKey.IsHolidayRequired)]
        public System.Int32 IsHoliday
        {
            get { return SelectedHolidayItem.SelectedValue; }
            //set { _isHoliday = value; }
        }


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHoliday))]
        private ComboItem _selectedHolidayItem;

        public CalendarActionWindowViewModel(AppDbContext appDbContext, CalendarDao calendarDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _calendarDao = calendarDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;

            _comboBoxItems = new AvaloniaList<ComboItem> {
               new ComboItem {DisplayName = I18nManager.GetString(Language.WorkingDays), SelectedValue = 0 },
               new ComboItem {DisplayName = I18nManager.GetString(Language.Holidays), SelectedValue = 1 },
            };
        }


        [ObservableProperty] private string _calendarNameValidationMessage;
        [ObservableProperty] private string _endDateValidationMessage;
        [ObservableProperty] private string _isHolidayValidationMessage;
        [ObservableProperty] private string _parkingDateValidationMessage;
        [ObservableProperty] private string _startDateValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CalendarName));
            //UpdateValidationMessage(nameof(EndDate));
            //UpdateValidationMessage(nameof(IsHoliday));
            UpdateValidationMessage(nameof(ParkingDate));
            //UpdateValidationMessage(nameof(StartDate));
            }

        public void SetSelectedHolidayItem(int value)
        {
            SelectedHolidayItem = _comboBoxItems?.FirstOrDefault(x => x.SelectedValue == value);
        }
        #region 命令
        /// <summary>
        /// 更新时，先判断：
        /// 1.该用户是否存在
        /// 2.修改后的用户信息会不会跟已经存在的信息冲突
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private Task Update()
        {
            if (IsBusy)
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                IsBusy = true;

                ValidateAllProperties();
                //前端验证
                if (HasErrors)
                {
                    UpdateValidationMessage(nameof(CalendarName));
                    //UpdateValidationMessage(nameof(StartDate));
                    //UpdateValidationMessage(nameof(EndDate));
                    UpdateValidationMessage(nameof(ParkingDate));
                    //UpdateValidationMessage(nameof(IsHoliday));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                var hasSameRecord = false;
                if ((bool)hasSameRecord)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        ToastManager.CreateToast()
                .WithTitle(I18nManager.GetString("UpdateCalendarPrompt"))
                .WithContent(I18nManager.GetString("CalendarExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempCalendar = _calendarDao.GetById(SelectedCalendar.Id);
                if (tempCalendar != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempCalendar.CalendarName = CalendarName;
                    tempCalendar.EndDate = EndDate;
                    tempCalendar.IsHoliday = IsHoliday;
                    tempCalendar.ParkingDate = ParkingDate;
                    tempCalendar.StartDate = StartDate;
                    int result = _calendarDao.Update(tempCalendar);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(Calendar),
                                     Title = I18nManager.GetString("UpdateCalendarPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close CalendarActionWindow", TokenManage.CALENDAR_ACTION_WINDOW_CLOSE_TOKEN);
                        });
                    }
                    else
                    {
                        var message = I18nManager.GetString("UpdateFailed");
                        UpdateInfo = message;
                    }
                    await Task.Delay(2000);

                    IsBusy = false;
                }
            });
        }

        [RelayCommand]
        private Task Add()
        {
            if (IsBusy)
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                IsBusy = true;

                ValidateAllProperties();
                //前端验证
                if (HasErrors)
                {
                    UpdateValidationMessage(nameof(CalendarName));
                    //UpdateValidationMessage(nameof(StartDate));
                    //UpdateValidationMessage(nameof(EndDate));
                    UpdateValidationMessage(nameof(ParkingDate));
                    //UpdateValidationMessage(nameof(IsHoliday));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempCalendar = _calendarDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_calendarDao.Add(new Calendar{
                    CalendarName = CalendarName,
                    EndDate = EndDate,
                    IsHoliday = IsHoliday,
                    ParkingDate = ParkingDate,
                    StartDate = StartDate,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(Calendar),
                            Title = I18nManager.GetString("CreateCalendarPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close CALENDARActionWindow", TokenManage.CALENDAR_ACTION_WINDOW_CLOSE_TOKEN);
                    });
                }
                else
                {
                    UpdateInfo = I18nManager.GetString("CreateFailed");
                }
                await Task.Delay(2000);

                IsBusy = false;
            });
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.CALENDAR_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

