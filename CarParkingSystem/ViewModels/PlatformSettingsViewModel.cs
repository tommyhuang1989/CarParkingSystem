using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public partial class PlatformSettingsViewModel : ParkSettingsTabViewModel
    {
        private AppDbContext _appDbContext;
        private ParkInfoDao _parkInfoDao;
        public ISukiToastManager _toastManager { get; }
        public ISukiDialogManager _dialogManager { get; }

        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）
        [ObservableProperty] private bool _isBusy;

        [Required(StringResourceKey.PayTypeRequired)]
        [ObservableProperty]
        private System.Int32 _payType;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;
        [ObservableProperty]
        private System.String _updateDate;
        [Required(StringResourceKey.PayTimeRequired)]
        [ObservableProperty]
        private System.Int32 _payTime;
        [Required(StringResourceKey.MerchantRequired)]
        [ObservableProperty]
        private System.String _merchant;
        [Required(StringResourceKey.PayUuidRequired)]
        [ObservableProperty]
        private System.String _payUuid;
        [Required(StringResourceKey.ParkUuidRequired)]
        [ObservableProperty]
        private System.String _parkUuid;
        [Required(StringResourceKey.RemainingCarsRequired)]
        [ObservableProperty]
        private System.Int32 _remainingCars;
        [Required(StringResourceKey.TotalCarsRequired)]
        [ObservableProperty]
        private System.Int32 _totalCars;
        public PlatformSettingsViewModel(AppDbContext appDbContext, ParkInfoDao parkInfoDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager) : base(Language.PlatformSettings)
        {
            _appDbContext = appDbContext;
            _parkInfoDao = parkInfoDao;
            _toastManager = toastManager;
            _dialogManager = dialogManager;
        }

        [ObservableProperty] private string _payTypeValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _updateDateValidationMessage;
        [ObservableProperty] private string _payTimeValidationMessage;
        [ObservableProperty] private string _merchantValidationMessage;
        [ObservableProperty] private string _payUuidValidationMessage;
        [ObservableProperty] private string _parkUuidValidationMessage;
        [ObservableProperty] private string _remainingCarsValidationMessage;
        [ObservableProperty] private string _totalCarsValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(PayType));
            UpdateValidationMessage(nameof(UpdateUser));
            UpdateValidationMessage(nameof(UpdateDate));
            UpdateValidationMessage(nameof(PayTime));
            UpdateValidationMessage(nameof(Merchant));
            UpdateValidationMessage(nameof(PayUuid));
            UpdateValidationMessage(nameof(ParkUuid));
            UpdateValidationMessage(nameof(RemainingCars));
            UpdateValidationMessage(nameof(TotalCars));
        }

        [RelayCommand]
        private Task Save()
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
                    UpdateValidationMessage(nameof(PayType));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(PayTime));
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(PayUuid));
                    UpdateValidationMessage(nameof(ParkUuid));
                    UpdateValidationMessage(nameof(RemainingCars));
                    UpdateValidationMessage(nameof(TotalCars));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                //var hasSameRecord = false;
                //if ((bool)hasSameRecord)
                //{
                //    Dispatcher.UIThread.Invoke(() =>
                //    {
                //        ToastManager.CreateToast()
                //.WithTitle(I18nManager.GetString("UpdateParkInfoPrompt"))
                //.WithContent(I18nManager.GetString("ParkInfoExists"))
                //.Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                //    });
                //    return;
                //}
                var tempParkInfo = _parkInfoDao.GetAll().FirstOrDefault();
                if (tempParkInfo != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkInfo.PayType = PayType;
                    tempParkInfo.UpdateUser = UpdateUser;
                    tempParkInfo.UpdateDate = UpdateDate;//tempDt
                    tempParkInfo.PayTime = PayTime;
                    tempParkInfo.Merchant = Merchant;
                    tempParkInfo.PayUuid = PayUuid;
                    tempParkInfo.ParkUuid = ParkUuid;
                    tempParkInfo.RemainingCars = RemainingCars;
                    tempParkInfo.TotalCars = TotalCars;
                    int result = _parkInfoDao.Update(tempParkInfo);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            //WeakReferenceMessenger.Default.Send(
                            //     new ToastMessage
                            //     {
                            //         CurrentModelType = typeof(ParkInfo),
                            //         Title = I18nManager.GetString("UpdateParkInfoPrompt"),
                            //         Content = I18nManager.GetString("UpdateSuccessfully"),
                            //         NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            //         NeedRefreshData = true
                            //     }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            //WeakReferenceMessenger.Default.Send("Close ParkInfoActionWindow", TokenManage.PARKINFO_ACTION_WINDOW_CLOSE_TOKEN);
                            _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                        });
                    }
                    else
                    {
                        //var message = I18nManager.GetString("UpdateFailed");
                        //UpdateInfo = message;
                        _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                    }
                    await Task.Delay(2000);

                    IsBusy = false;
                }
                else
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var result = _parkInfoDao.Add(new ParkInfo
                    {
                        PayType = PayType,
                        UpdateUser = UpdateUser,
                        UpdateDate = UpdateDate,//tempDt
                        PayTime = PayTime,
                        Merchant = Merchant,
                        PayUuid = PayUuid,
                        ParkUuid = ParkUuid,
                        RemainingCars = RemainingCars,
                        TotalCars = TotalCars,
                    });

                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                    });
                    }
                    else
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                        });
                    }
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
                    UpdateValidationMessage(nameof(PayType));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(PayTime));
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(PayUuid));
                    UpdateValidationMessage(nameof(ParkUuid));
                    UpdateValidationMessage(nameof(RemainingCars));
                    UpdateValidationMessage(nameof(TotalCars));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                /*
                //对数据的唯一性进行验证，这里需要测试来修正
                                var tempParkInfo = _parkInfoDao.GetByUsername(Username);
                                if (tempUser != null)
                                {
                                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                                return;
                                }
                */
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result = _parkInfoDao.Add(new ParkInfo
                {
                    PayType = PayType,
                    UpdateUser = UpdateUser,
                    UpdateDate = tempDt,
                    PayTime = PayTime,
                    Merchant = Merchant,
                    PayUuid = PayUuid,
                    ParkUuid = ParkUuid,
                    RemainingCars = RemainingCars,
                    TotalCars = TotalCars,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        //WeakReferenceMessenger.Default.Send(
                        //    new ToastMessage
                        //    {
                        //        CurrentModelType = typeof(ParkInfo),
                        //        Title = I18nManager.GetString("CreateParkInfoPrompt"),
                        //        Content = I18nManager.GetString("CreateSuccessfully"),
                        //        NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                        //        NeedRefreshData = true
                        //    }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        //WeakReferenceMessenger.Default.Send("Close PARKINFOActionWindow", TokenManage.PARKINFO_ACTION_WINDOW_CLOSE_TOKEN);
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
    }
}
