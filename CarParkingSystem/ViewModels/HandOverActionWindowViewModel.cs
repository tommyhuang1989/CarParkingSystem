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
    public partial class HandOverActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private HandOverDao _handOverDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddHandOver;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private HandOver _selectedHandOver;

        [Required(StringResourceKey.AbCarRequired)]
        [ObservableProperty]
        private System.Int32 _abCar;
        [Required(StringResourceKey.ArrearsCarRequired)]
        [ObservableProperty]
        private System.Int32 _arrearsCar;
        [Required(StringResourceKey.CashFeeRequired)]
        [ObservableProperty]
        private System.Decimal _cashFee;
        [ObservableProperty]
        private System.String _endDate;
        [Required(StringResourceKey.EtcfeeRequired)]
        [ObservableProperty]
        private System.Decimal _etcfee;
        [Required(StringResourceKey.InCarRequired)]
        [ObservableProperty]
        private System.Int32 _inCar;
        [Required(StringResourceKey.IsFinishedRequired)]
        [ObservableProperty]
        private System.Int32 _isFinished;
        [Required(StringResourceKey.OutCarRequired)]
        [ObservableProperty]
        private System.Int32 _outCar;
        [Required(StringResourceKey.PhoneFeeRequired)]
        [ObservableProperty]
        private System.Decimal _phoneFee;
        [ObservableProperty]
        private System.String _startDate;
        [Required(StringResourceKey.TotalFeeRequired)]
        [ObservableProperty]
        private System.Decimal _totalFee;
        [Required(StringResourceKey.UserIdRequired)]
        [ObservableProperty]
        private System.Int32 _userId;
        [Required(StringResourceKey.ValueCardFeeRequired)]
        [ObservableProperty]
        private System.Decimal _valueCardFee;

        public HandOverActionWindowViewModel(AppDbContext appDbContext, HandOverDao handOverDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _handOverDao = handOverDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _abCarValidationMessage;
        [ObservableProperty] private string _arrearsCarValidationMessage;
        [ObservableProperty] private string _cashFeeValidationMessage;
        [ObservableProperty] private string _endDateValidationMessage;
        [ObservableProperty] private string _etcfeeValidationMessage;
        [ObservableProperty] private string _inCarValidationMessage;
        [ObservableProperty] private string _isFinishedValidationMessage;
        [ObservableProperty] private string _outCarValidationMessage;
        [ObservableProperty] private string _phoneFeeValidationMessage;
        [ObservableProperty] private string _startDateValidationMessage;
        [ObservableProperty] private string _totalFeeValidationMessage;
        [ObservableProperty] private string _userIdValidationMessage;
        [ObservableProperty] private string _valueCardFeeValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AbCar));
            UpdateValidationMessage(nameof(ArrearsCar));
            UpdateValidationMessage(nameof(CashFee));
            UpdateValidationMessage(nameof(EndDate));
            UpdateValidationMessage(nameof(Etcfee));
            UpdateValidationMessage(nameof(InCar));
            UpdateValidationMessage(nameof(IsFinished));
            UpdateValidationMessage(nameof(OutCar));
            UpdateValidationMessage(nameof(PhoneFee));
            UpdateValidationMessage(nameof(StartDate));
            UpdateValidationMessage(nameof(TotalFee));
            UpdateValidationMessage(nameof(UserId));
            UpdateValidationMessage(nameof(ValueCardFee));
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
                    UpdateValidationMessage(nameof(AbCar));
                    UpdateValidationMessage(nameof(ArrearsCar));
                    UpdateValidationMessage(nameof(CashFee));
                    UpdateValidationMessage(nameof(Etcfee));
                    UpdateValidationMessage(nameof(InCar));
                    UpdateValidationMessage(nameof(IsFinished));
                    UpdateValidationMessage(nameof(OutCar));
                    UpdateValidationMessage(nameof(PhoneFee));
                    UpdateValidationMessage(nameof(TotalFee));
                    UpdateValidationMessage(nameof(UserId));
                    UpdateValidationMessage(nameof(ValueCardFee));
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
                .WithTitle(I18nManager.GetString("UpdateHandOverPrompt"))
                .WithContent(I18nManager.GetString("HandOverExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempHandOver = _handOverDao.GetById(SelectedHandOver.Id);
                if (tempHandOver != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempHandOver.AbCar = AbCar;
                    tempHandOver.ArrearsCar = ArrearsCar;
                    tempHandOver.CashFee = CashFee;
                    tempHandOver.EndDate = tempDt;
                    tempHandOver.Etcfee = Etcfee;
                    tempHandOver.InCar = InCar;
                    tempHandOver.IsFinished = IsFinished;
                    tempHandOver.OutCar = OutCar;
                    tempHandOver.PhoneFee = PhoneFee;
                    tempHandOver.StartDate = tempDt;
                    tempHandOver.TotalFee = TotalFee;
                    tempHandOver.UserId = UserId;
                    tempHandOver.ValueCardFee = ValueCardFee;
                    int result = _handOverDao.Update(tempHandOver);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(HandOver),
                                     Title = I18nManager.GetString("UpdateHandOverPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close HandOverActionWindow", TokenManage.HANDOVER_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(AbCar));
                    UpdateValidationMessage(nameof(ArrearsCar));
                    UpdateValidationMessage(nameof(CashFee));
                    UpdateValidationMessage(nameof(Etcfee));
                    UpdateValidationMessage(nameof(InCar));
                    UpdateValidationMessage(nameof(IsFinished));
                    UpdateValidationMessage(nameof(OutCar));
                    UpdateValidationMessage(nameof(PhoneFee));
                    UpdateValidationMessage(nameof(TotalFee));
                    UpdateValidationMessage(nameof(UserId));
                    UpdateValidationMessage(nameof(ValueCardFee));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempHandOver = _handOverDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_handOverDao.Add(new HandOver{
                    AbCar = AbCar,
                    ArrearsCar = ArrearsCar,
                    CashFee = CashFee,
                    EndDate = tempDt,
                    Etcfee = Etcfee,
                    InCar = InCar,
                    IsFinished = IsFinished,
                    OutCar = OutCar,
                    PhoneFee = PhoneFee,
                    StartDate = tempDt,
                    TotalFee = TotalFee,
                    UserId = UserId,
                    ValueCardFee = ValueCardFee,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(HandOver),
                            Title = I18nManager.GetString("CreateHandOverPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close HANDOVERActionWindow", TokenManage.HANDOVER_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.HANDOVER_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

