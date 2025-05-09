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
    public partial class ParkInfoActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkInfoDao _parkInfoDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkInfo;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkInfo _selectedParkInfo;

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

        public ParkInfoActionWindowViewModel(AppDbContext appDbContext, ParkInfoDao parkInfoDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkInfoDao = parkInfoDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
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

                var hasSameRecord = false;
                if ((bool)hasSameRecord)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        ToastManager.CreateToast()
                .WithTitle(I18nManager.GetString("UpdateParkInfoPrompt"))
                .WithContent(I18nManager.GetString("ParkInfoExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkInfo = _parkInfoDao.GetById(SelectedParkInfo.Id);
                if (tempParkInfo != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkInfo.PayType = PayType;
                    tempParkInfo.UpdateUser = UpdateUser;
                    tempParkInfo.UpdateDate = tempDt;
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
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkInfo),
                                     Title = I18nManager.GetString("UpdateParkInfoPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkInfoActionWindow", TokenManage.PARKINFO_ACTION_WINDOW_CLOSE_TOKEN);
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
                var result =_parkInfoDao.Add(new ParkInfo{
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
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkInfo),
                            Title = I18nManager.GetString("CreateParkInfoPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKINFOActionWindow", TokenManage.PARKINFO_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKINFO_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

