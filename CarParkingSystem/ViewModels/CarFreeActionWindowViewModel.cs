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
    public partial class CarFreeActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private CarFreeDao _carFreeDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddCarFree;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private CarFree _selectedCarFree;

        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [ObservableProperty]
        private System.String _endTime;
        [Required(StringResourceKey.FreeDescRequired)]
        [ObservableProperty]
        private System.String _freeDesc;
        [ObservableProperty]
        private System.String _fromTime;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;
        [Required(StringResourceKey.UserAddrRequired)]
        [ObservableProperty]
        private System.String _userAddr;
        [Required(StringResourceKey.UsernameRequired)]
        [ObservableProperty]
        private System.String _username;
        [Required(StringResourceKey.UserPhoneRequired)]
        [ObservableProperty]
        private System.String _userPhone;
        [Required(StringResourceKey.UserTypeRequired)]
        [ObservableProperty]
        private System.Int32 _userType;
        [Required(StringResourceKey.WxOpenIdRequired)]
        [ObservableProperty]
        private System.String _wxOpenId;

        public CarFreeActionWindowViewModel(AppDbContext appDbContext, CarFreeDao carFreeDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _carFreeDao = carFreeDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _endTimeValidationMessage;
        [ObservableProperty] private string _freeDescValidationMessage;
        [ObservableProperty] private string _fromTimeValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _userAddrValidationMessage;
        [ObservableProperty] private string _usernameValidationMessage;
        [ObservableProperty] private string _userPhoneValidationMessage;
        [ObservableProperty] private string _userTypeValidationMessage;
        [ObservableProperty] private string _wxOpenIdValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(EndTime));
            UpdateValidationMessage(nameof(FreeDesc));
            UpdateValidationMessage(nameof(FromTime));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
            UpdateValidationMessage(nameof(UserAddr));
            UpdateValidationMessage(nameof(Username));
            UpdateValidationMessage(nameof(UserPhone));
            UpdateValidationMessage(nameof(UserType));
            UpdateValidationMessage(nameof(WxOpenId));
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
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(FreeDesc));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(UserAddr));
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(UserPhone));
                    UpdateValidationMessage(nameof(UserType));
                    UpdateValidationMessage(nameof(WxOpenId));
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
                .WithTitle(I18nManager.GetString("UpdateCarFreePrompt"))
                .WithContent(I18nManager.GetString("CarFreeExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempCarFree = _carFreeDao.GetById(SelectedCarFree.Id);
                if (tempCarFree != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempCarFree.CarNo = CarNo;
                    tempCarFree.EndTime = tempDt;
                    tempCarFree.FreeDesc = FreeDesc;
                    tempCarFree.FromTime = tempDt;
                    tempCarFree.RecStatus = RecStatus;
                    tempCarFree.UpdateDt = UpdateDt;
                    tempCarFree.UpdateUser = UpdateUser;
                    tempCarFree.UserAddr = UserAddr;
                    tempCarFree.Username = Username;
                    tempCarFree.UserPhone = UserPhone;
                    tempCarFree.UserType = UserType;
                    tempCarFree.WxOpenId = WxOpenId;
                    int result = _carFreeDao.Update(tempCarFree);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(CarFree),
                                     Title = I18nManager.GetString("UpdateCarFreePrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close CarFreeActionWindow", TokenManage.CARFREE_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(FreeDesc));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(UserAddr));
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(UserPhone));
                    UpdateValidationMessage(nameof(UserType));
                    UpdateValidationMessage(nameof(WxOpenId));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempCarFree = _carFreeDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_carFreeDao.Add(new CarFree{
                    CarNo = CarNo,
                    EndTime = tempDt,
                    FreeDesc = FreeDesc,
                    FromTime = tempDt,
                    RecStatus = RecStatus,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                    UserAddr = UserAddr,
                    Username = Username,
                    UserPhone = UserPhone,
                    UserType = UserType,
                    WxOpenId = WxOpenId,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(CarFree),
                            Title = I18nManager.GetString("CreateCarFreePrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close CARFREEActionWindow", TokenManage.CARFREE_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.CARFREE_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

