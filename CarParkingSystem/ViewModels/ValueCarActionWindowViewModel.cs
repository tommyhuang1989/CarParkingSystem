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
    public partial class ValueCarActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ValueCarDao _valueCarDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddValueCar;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ValueCar _selectedValueCar;

        [Required(StringResourceKey.BalanceRequired)]
        [ObservableProperty]
        private System.Decimal _balance;
        [Required(StringResourceKey.CarCodeRequired)]
        [ObservableProperty]
        private System.String _carCode;
        [Required(StringResourceKey.CardRequired)]
        [ObservableProperty]
        private System.Int32 _card;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.DepositRequired)]
        [ObservableProperty]
        private System.Decimal _deposit;
        [Required(StringResourceKey.ParkSpaceRequired)]
        [ObservableProperty]
        private System.String _parkSpace;
        [Required(StringResourceKey.ParkSpaceTypeRequired)]
        [ObservableProperty]
        private System.Int32 _parkSpaceType;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [Required(StringResourceKey.SpaceNameRequired)]
        [ObservableProperty]
        private System.String _spaceName;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;
        [Required(StringResourceKey.UsernameRequired)]
        [ObservableProperty]
        private System.String _username;
        [Required(StringResourceKey.UserRemarkRequired)]
        [ObservableProperty]
        private System.String _userRemark;
        [Required(StringResourceKey.UserTelRequired)]
        [ObservableProperty]
        private System.String _userTel;
        [ObservableProperty]
        private System.String _validEnd;
        [ObservableProperty]
        private System.String _validFrom;

        public ValueCarActionWindowViewModel(AppDbContext appDbContext, ValueCarDao valueCarDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _valueCarDao = valueCarDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _balanceValidationMessage;
        [ObservableProperty] private string _carCodeValidationMessage;
        [ObservableProperty] private string _cardValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _depositValidationMessage;
        [ObservableProperty] private string _parkSpaceValidationMessage;
        [ObservableProperty] private string _parkSpaceTypeValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _spaceNameValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _usernameValidationMessage;
        [ObservableProperty] private string _userRemarkValidationMessage;
        [ObservableProperty] private string _userTelValidationMessage;
        [ObservableProperty] private string _validEndValidationMessage;
        [ObservableProperty] private string _validFromValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(Balance));
            UpdateValidationMessage(nameof(CarCode));
            UpdateValidationMessage(nameof(Card));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(Deposit));
            UpdateValidationMessage(nameof(ParkSpace));
            UpdateValidationMessage(nameof(ParkSpaceType));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(SpaceName));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
            UpdateValidationMessage(nameof(Username));
            UpdateValidationMessage(nameof(UserRemark));
            UpdateValidationMessage(nameof(UserTel));
            UpdateValidationMessage(nameof(ValidEnd));
            UpdateValidationMessage(nameof(ValidFrom));
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
                    UpdateValidationMessage(nameof(Balance));
                    UpdateValidationMessage(nameof(CarCode));
                    UpdateValidationMessage(nameof(Card));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(Deposit));
                    UpdateValidationMessage(nameof(ParkSpace));
                    UpdateValidationMessage(nameof(ParkSpaceType));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(SpaceName));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(UserRemark));
                    UpdateValidationMessage(nameof(UserTel));
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
                .WithTitle(I18nManager.GetString("UpdateValueCarPrompt"))
                .WithContent(I18nManager.GetString("ValueCarExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempValueCar = _valueCarDao.GetById(SelectedValueCar.Id);
                if (tempValueCar != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempValueCar.Balance = Balance;
                    tempValueCar.CarCode = CarCode;
                    tempValueCar.Card = Card;
                    tempValueCar.CarNo = CarNo;
                    tempValueCar.Deposit = Deposit;
                    tempValueCar.ParkSpace = ParkSpace;
                    tempValueCar.ParkSpaceType = ParkSpaceType;
                    tempValueCar.RecStatus = RecStatus;
                    tempValueCar.Remark = Remark;
                    tempValueCar.SpaceName = SpaceName;
                    tempValueCar.UpdateDt = UpdateDt;
                    tempValueCar.UpdateUser = UpdateUser;
                    tempValueCar.Username = Username;
                    tempValueCar.UserRemark = UserRemark;
                    tempValueCar.UserTel = UserTel;
                    tempValueCar.ValidEnd = ValidEnd;
                    tempValueCar.ValidFrom = ValidFrom;
                    int result = _valueCarDao.Update(tempValueCar);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ValueCar),
                                     Title = I18nManager.GetString("UpdateValueCarPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ValueCarActionWindow", TokenManage.VALUECAR_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(Balance));
                    UpdateValidationMessage(nameof(CarCode));
                    UpdateValidationMessage(nameof(Card));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(Deposit));
                    UpdateValidationMessage(nameof(ParkSpace));
                    UpdateValidationMessage(nameof(ParkSpaceType));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(SpaceName));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(UserRemark));
                    UpdateValidationMessage(nameof(UserTel));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempValueCar = _valueCarDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_valueCarDao.Add(new ValueCar{
                    Balance = Balance,
                    CarCode = CarCode,
                    Card = Card,
                    CarNo = CarNo,
                    Deposit = Deposit,
                    ParkSpace = ParkSpace,
                    ParkSpaceType = ParkSpaceType,
                    RecStatus = RecStatus,
                    Remark = Remark,
                    SpaceName = SpaceName,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                    Username = Username,
                    UserRemark = UserRemark,
                    UserTel = UserTel,
                    ValidEnd = ValidEnd,
                    ValidFrom = ValidFrom,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ValueCar),
                            Title = I18nManager.GetString("CreateValueCarPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close VALUECARActionWindow", TokenManage.VALUECAR_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.VALUECAR_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

