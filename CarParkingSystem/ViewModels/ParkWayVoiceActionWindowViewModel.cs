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
    public partial class ParkWayVoiceActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkWayVoiceDao _parkWayVoiceDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkWayVoice;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkWayVoice _selectedParkWayVoice;

        [Required(StringResourceKey.EndHourRequired)]
        [ObservableProperty]
        private System.Int32 _endHour;
        [Required(StringResourceKey.EndMinuteRequired)]
        [ObservableProperty]
        private System.Int32 _endMinute;
        [ObservableProperty]
        private System.String _lastUpdateDate;
        [Required(StringResourceKey.StartHourRequired)]
        [ObservableProperty]
        private System.Int32 _startHour;
        [Required(StringResourceKey.StartMinuteRequired)]
        [ObservableProperty]
        private System.Int32 _startMinute;
        [Required(StringResourceKey.VolumeRequired)]
        [ObservableProperty]
        private System.Int32 _volume;
        [Required(StringResourceKey.WayIdRequired)]
        [ObservableProperty]
        private System.Int32 _wayId;

        public ParkWayVoiceActionWindowViewModel(AppDbContext appDbContext, ParkWayVoiceDao parkWayVoiceDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkWayVoiceDao = parkWayVoiceDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _endHourValidationMessage;
        [ObservableProperty] private string _endMinuteValidationMessage;
        [ObservableProperty] private string _lastUpdateDateValidationMessage;
        [ObservableProperty] private string _startHourValidationMessage;
        [ObservableProperty] private string _startMinuteValidationMessage;
        [ObservableProperty] private string _volumeValidationMessage;
        [ObservableProperty] private string _wayIdValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(EndHour));
            UpdateValidationMessage(nameof(EndMinute));
            UpdateValidationMessage(nameof(LastUpdateDate));
            UpdateValidationMessage(nameof(StartHour));
            UpdateValidationMessage(nameof(StartMinute));
            UpdateValidationMessage(nameof(Volume));
            UpdateValidationMessage(nameof(WayId));
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
                    UpdateValidationMessage(nameof(EndHour));
                    UpdateValidationMessage(nameof(EndMinute));
                    UpdateValidationMessage(nameof(StartHour));
                    UpdateValidationMessage(nameof(StartMinute));
                    UpdateValidationMessage(nameof(Volume));
                    UpdateValidationMessage(nameof(WayId));
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
                .WithTitle(I18nManager.GetString("UpdateParkWayVoicePrompt"))
                .WithContent(I18nManager.GetString("ParkWayVoiceExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkWayVoice = _parkWayVoiceDao.GetById(SelectedParkWayVoice.Id);
                if (tempParkWayVoice != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkWayVoice.EndHour = EndHour;
                    tempParkWayVoice.EndMinute = EndMinute;
                    tempParkWayVoice.LastUpdateDate = tempDt;
                    tempParkWayVoice.StartHour = StartHour;
                    tempParkWayVoice.StartMinute = StartMinute;
                    tempParkWayVoice.Volume = Volume;
                    tempParkWayVoice.WayId = WayId;
                    int result = _parkWayVoiceDao.Update(tempParkWayVoice);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkWayVoice),
                                     Title = I18nManager.GetString("UpdateParkWayVoicePrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkWayVoiceActionWindow", TokenManage.PARKWAYVOICE_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(EndHour));
                    UpdateValidationMessage(nameof(EndMinute));
                    UpdateValidationMessage(nameof(StartHour));
                    UpdateValidationMessage(nameof(StartMinute));
                    UpdateValidationMessage(nameof(Volume));
                    UpdateValidationMessage(nameof(WayId));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkWayVoice = _parkWayVoiceDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkWayVoiceDao.Add(new ParkWayVoice{
                    EndHour = EndHour,
                    EndMinute = EndMinute,
                    LastUpdateDate = tempDt,
                    StartHour = StartHour,
                    StartMinute = StartMinute,
                    Volume = Volume,
                    WayId = WayId,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkWayVoice),
                            Title = I18nManager.GetString("CreateParkWayVoicePrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKWAYVOICEActionWindow", TokenManage.PARKWAYVOICE_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKWAYVOICE_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

