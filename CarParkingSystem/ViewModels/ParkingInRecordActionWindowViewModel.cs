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
    public partial class ParkingInRecordActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkingInRecordDao _parkingInRecordDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkingInRecord;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkingInRecord _selectedParkingInRecord;

        [Required(StringResourceKey.AmountMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _amountMoney;
        [Required(StringResourceKey.AutoPayRequired)]
        [ObservableProperty]
        private System.Int32 _autoPay;
        [Required(StringResourceKey.AutoPayIdRequired)]
        [ObservableProperty]
        private System.String _autoPayId;
        [ObservableProperty]
        private System.String _calculateOutTime;
        [Required(StringResourceKey.CarColorRequired)]
        [ObservableProperty]
        private System.Int32 _carColor;
        [ObservableProperty]
        private System.String _cardChangeTime;
        [Required(StringResourceKey.CardNoRequired)]
        [ObservableProperty]
        private System.Int32 _cardNo;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.CarStatusRequired)]
        [ObservableProperty]
        private System.Int32 _carStatus;
        [Required(StringResourceKey.CarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _carType;
        [Required(StringResourceKey.DiscountMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _discountMoney;
        [Required(StringResourceKey.IncpChangedRequired)]
        [ObservableProperty]
        private System.Int32 _incpChanged;
        [Required(StringResourceKey.InImgRequired)]
        [ObservableProperty]
        private System.String _inImg;
        [Required(StringResourceKey.InOperatorIdRequired)]
        [ObservableProperty]
        private System.Int32 _inOperatorId;
        [ObservableProperty]
        private System.String _inTime;
        [Required(StringResourceKey.InTypeRequired)]
        [ObservableProperty]
        private System.Int32 _inType;
        [Required(StringResourceKey.InWayIdRequired)]
        [ObservableProperty]
        private System.Int32 _inWayId;
        [Required(StringResourceKey.MonthToTempNumberRequired)]
        [ObservableProperty]
        private System.Int32 _monthToTempNumber;
        [Required(StringResourceKey.OpenTypeRequired)]
        [ObservableProperty]
        private System.Int32 _openType;
        [Required(StringResourceKey.OrderIdRequired)]
        [ObservableProperty]
        private System.String _orderId;
        [Required(StringResourceKey.OriginCardNoRequired)]
        [ObservableProperty]
        private System.Int32 _originCardNo;
        [Required(StringResourceKey.PaidMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _paidMoney;
        [Required(StringResourceKey.PlateIdRequired)]
        [ObservableProperty]
        private System.String _plateId;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;

        public ParkingInRecordActionWindowViewModel(AppDbContext appDbContext, ParkingInRecordDao parkingInRecordDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkingInRecordDao = parkingInRecordDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _amountMoneyValidationMessage;
        [ObservableProperty] private string _autoPayValidationMessage;
        [ObservableProperty] private string _autoPayIdValidationMessage;
        [ObservableProperty] private string _calculateOutTimeValidationMessage;
        [ObservableProperty] private string _carColorValidationMessage;
        [ObservableProperty] private string _cardChangeTimeValidationMessage;
        [ObservableProperty] private string _cardNoValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _carStatusValidationMessage;
        [ObservableProperty] private string _carTypeValidationMessage;
        [ObservableProperty] private string _discountMoneyValidationMessage;
        [ObservableProperty] private string _incpChangedValidationMessage;
        [ObservableProperty] private string _inImgValidationMessage;
        [ObservableProperty] private string _inOperatorIdValidationMessage;
        [ObservableProperty] private string _inTimeValidationMessage;
        [ObservableProperty] private string _inTypeValidationMessage;
        [ObservableProperty] private string _inWayIdValidationMessage;
        [ObservableProperty] private string _monthToTempNumberValidationMessage;
        [ObservableProperty] private string _openTypeValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _originCardNoValidationMessage;
        [ObservableProperty] private string _paidMoneyValidationMessage;
        [ObservableProperty] private string _plateIdValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AmountMoney));
            UpdateValidationMessage(nameof(AutoPay));
            UpdateValidationMessage(nameof(AutoPayId));
            UpdateValidationMessage(nameof(CalculateOutTime));
            UpdateValidationMessage(nameof(CarColor));
            UpdateValidationMessage(nameof(CardChangeTime));
            UpdateValidationMessage(nameof(CardNo));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(CarStatus));
            UpdateValidationMessage(nameof(CarType));
            UpdateValidationMessage(nameof(DiscountMoney));
            UpdateValidationMessage(nameof(IncpChanged));
            UpdateValidationMessage(nameof(InImg));
            UpdateValidationMessage(nameof(InOperatorId));
            UpdateValidationMessage(nameof(InTime));
            UpdateValidationMessage(nameof(InType));
            UpdateValidationMessage(nameof(InWayId));
            UpdateValidationMessage(nameof(MonthToTempNumber));
            UpdateValidationMessage(nameof(OpenType));
            UpdateValidationMessage(nameof(OrderId));
            UpdateValidationMessage(nameof(OriginCardNo));
            UpdateValidationMessage(nameof(PaidMoney));
            UpdateValidationMessage(nameof(PlateId));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
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
                    UpdateValidationMessage(nameof(AmountMoney));
                    UpdateValidationMessage(nameof(AutoPay));
                    UpdateValidationMessage(nameof(AutoPayId));
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarStatus));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(IncpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(MonthToTempNumber));
                    UpdateValidationMessage(nameof(OpenType));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OriginCardNo));
                    UpdateValidationMessage(nameof(PaidMoney));
                    UpdateValidationMessage(nameof(PlateId));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(UpdateUser));
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
                .WithTitle(I18nManager.GetString("UpdateParkingInRecordPrompt"))
                .WithContent(I18nManager.GetString("ParkingInRecordExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkingInRecord = _parkingInRecordDao.GetById(SelectedParkingInRecord.Id);
                if (tempParkingInRecord != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkingInRecord.AmountMoney = AmountMoney;
                    tempParkingInRecord.AutoPay = AutoPay;
                    tempParkingInRecord.AutoPayId = AutoPayId;
                    tempParkingInRecord.CalculateOutTime = tempDt;
                    tempParkingInRecord.CarColor = CarColor;
                    tempParkingInRecord.CardChangeTime = tempDt;
                    tempParkingInRecord.CardNo = CardNo;
                    tempParkingInRecord.CarNo = CarNo;
                    tempParkingInRecord.CarStatus = CarStatus;
                    tempParkingInRecord.CarType = CarType;
                    tempParkingInRecord.DiscountMoney = DiscountMoney;
                    tempParkingInRecord.IncpChanged = IncpChanged;
                    tempParkingInRecord.InImg = InImg;
                    tempParkingInRecord.InOperatorId = InOperatorId;
                    tempParkingInRecord.InTime = tempDt;
                    tempParkingInRecord.InType = InType;
                    tempParkingInRecord.InWayId = InWayId;
                    tempParkingInRecord.MonthToTempNumber = MonthToTempNumber;
                    tempParkingInRecord.OpenType = OpenType;
                    tempParkingInRecord.OrderId = OrderId;
                    tempParkingInRecord.OriginCardNo = OriginCardNo;
                    tempParkingInRecord.PaidMoney = PaidMoney;
                    tempParkingInRecord.PlateId = PlateId;
                    tempParkingInRecord.RecStatus = RecStatus;
                    tempParkingInRecord.Remark = Remark;
                    tempParkingInRecord.UpdateDt = UpdateDt;
                    tempParkingInRecord.UpdateUser = UpdateUser;
                    int result = _parkingInRecordDao.Update(tempParkingInRecord);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkingInRecord),
                                     Title = I18nManager.GetString("UpdateParkingInRecordPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkingInRecordActionWindow", TokenManage.PARKINGINRECORD_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(AmountMoney));
                    UpdateValidationMessage(nameof(AutoPay));
                    UpdateValidationMessage(nameof(AutoPayId));
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarStatus));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(IncpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(MonthToTempNumber));
                    UpdateValidationMessage(nameof(OpenType));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OriginCardNo));
                    UpdateValidationMessage(nameof(PaidMoney));
                    UpdateValidationMessage(nameof(PlateId));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(UpdateUser));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkingInRecord = _parkingInRecordDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkingInRecordDao.Add(new ParkingInRecord{
                    AmountMoney = AmountMoney,
                    AutoPay = AutoPay,
                    AutoPayId = AutoPayId,
                    CalculateOutTime = tempDt,
                    CarColor = CarColor,
                    CardChangeTime = tempDt,
                    CardNo = CardNo,
                    CarNo = CarNo,
                    CarStatus = CarStatus,
                    CarType = CarType,
                    DiscountMoney = DiscountMoney,
                    IncpChanged = IncpChanged,
                    InImg = InImg,
                    InOperatorId = InOperatorId,
                    InTime = tempDt,
                    InType = InType,
                    InWayId = InWayId,
                    MonthToTempNumber = MonthToTempNumber,
                    OpenType = OpenType,
                    OrderId = OrderId,
                    OriginCardNo = OriginCardNo,
                    PaidMoney = PaidMoney,
                    PlateId = PlateId,
                    RecStatus = RecStatus,
                    Remark = Remark,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkingInRecord),
                            Title = I18nManager.GetString("CreateParkingInRecordPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKINGINRECORDActionWindow", TokenManage.PARKINGINRECORD_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKINGINRECORD_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

