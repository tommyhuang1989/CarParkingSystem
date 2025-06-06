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

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为新增、修改信息界面提供数据的 ViewModel
    /// </summary>
    public partial class ParkWayStopTimeActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkWayStopTimeDao _parkWayStopTimeDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkWayStopTime;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkWayStopTime _selectedParkWayStopTime;

        [Required(StringResourceKey.CardIdRequired)]
        [ObservableProperty]
        private System.Int32 _cardId;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [Required(StringResourceKey.StopEndHourRequired)]
        [ObservableProperty]
        private System.Int32 _stopEndHour;
        [Required(StringResourceKey.StopEndMinuteRequired)]
        [ObservableProperty]
        private System.Int32 _stopEndMinute;
        [Required(StringResourceKey.StopStartHourRequired)]
        [ObservableProperty]
        private System.Int32 _stopStartHour;
        [Required(StringResourceKey.StopStartMinuteRequired)]
        [ObservableProperty]
        private System.Int32 _stopStartMinute;
        [Required(StringResourceKey.WayIdRequired)]
        [ObservableProperty]
        private System.Int32 _wayId;
        [Required(StringResourceKey.WeeksRequired)]
        [ObservableProperty]
        private System.String _weeks;

        public ParkWayStopTimeActionWindowViewModel(AppDbContext appDbContext, ParkWayStopTimeDao parkWayStopTimeDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkWayStopTimeDao = parkWayStopTimeDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _cardIdValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _stopEndHourValidationMessage;
        [ObservableProperty] private string _stopEndMinuteValidationMessage;
        [ObservableProperty] private string _stopStartHourValidationMessage;
        [ObservableProperty] private string _stopStartMinuteValidationMessage;
        [ObservableProperty] private string _wayIdValidationMessage;
        [ObservableProperty] private string _weeksValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CardId));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(StopEndHour));
            UpdateValidationMessage(nameof(StopEndMinute));
            UpdateValidationMessage(nameof(StopStartHour));
            UpdateValidationMessage(nameof(StopStartMinute));
            UpdateValidationMessage(nameof(WayId));
            UpdateValidationMessage(nameof(Weeks));
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
                    UpdateValidationMessage(nameof(CardId));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(StopEndHour));
                    UpdateValidationMessage(nameof(StopEndMinute));
                    UpdateValidationMessage(nameof(StopStartHour));
                    UpdateValidationMessage(nameof(StopStartMinute));
                    UpdateValidationMessage(nameof(WayId));
                    UpdateValidationMessage(nameof(Weeks));
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
                .WithTitle(I18nManager.GetString("UpdateParkWayStopTimePrompt"))
                .WithContent(I18nManager.GetString("ParkWayStopTimeExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkWayStopTime = _parkWayStopTimeDao.GetById(SelectedParkWayStopTime.Id);
                if (tempParkWayStopTime != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkWayStopTime.CardId = CardId;
                    tempParkWayStopTime.Remark = Remark;
                    tempParkWayStopTime.StopEndHour = StopEndHour;
                    tempParkWayStopTime.StopEndMinute = StopEndMinute;
                    tempParkWayStopTime.StopStartHour = StopStartHour;
                    tempParkWayStopTime.StopStartMinute = StopStartMinute;
                    tempParkWayStopTime.WayId = WayId;
                    tempParkWayStopTime.Weeks = Weeks;
                    int result = _parkWayStopTimeDao.Update(tempParkWayStopTime);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkWayStopTime),
                                     Title = I18nManager.GetString("UpdateParkWayStopTimePrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkWayStopTimeActionWindow", TokenManage.PARKWAYSTOPTIME_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CardId));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(StopEndHour));
                    UpdateValidationMessage(nameof(StopEndMinute));
                    UpdateValidationMessage(nameof(StopStartHour));
                    UpdateValidationMessage(nameof(StopStartMinute));
                    UpdateValidationMessage(nameof(WayId));
                    UpdateValidationMessage(nameof(Weeks));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkWayStopTime = _parkWayStopTimeDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkWayStopTimeDao.Add(new ParkWayStopTime{
                    CardId = CardId,
                    Remark = Remark,
                    StopEndHour = StopEndHour,
                    StopEndMinute = StopEndMinute,
                    StopStartHour = StopStartHour,
                    StopStartMinute = StopStartMinute,
                    WayId = WayId,
                    Weeks = Weeks,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkWayStopTime),
                            Title = I18nManager.GetString("CreateParkWayStopTimePrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKWAYSTOPTIMEActionWindow", TokenManage.PARKWAYSTOPTIME_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKWAYSTOPTIME_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

