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
    public partial class ParkingOutRecordActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkingOutRecordDao _parkingOutRecordDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkingOutRecord;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkingOutRecord _selectedParkingOutRecord;

        [Required(StringResourceKey.AmountMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _amountMoney;
        [Required(StringResourceKey.CardNoRequired)]
        [ObservableProperty]
        private System.Int32 _cardNo;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.ChargeTypeRequired)]
        [ObservableProperty]
        private System.Int32 _chargeType;
        [Required(StringResourceKey.DiscountMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _discountMoney;
        [Required(StringResourceKey.InCarColorRequired)]
        [ObservableProperty]
        private System.Int32 _inCarColor;
        [Required(StringResourceKey.InCarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _inCarType;
        [Required(StringResourceKey.InCpChangedRequired)]
        [ObservableProperty]
        private System.Int32 _inCpChanged;
        [Required(StringResourceKey.InImgRequired)]
        [ObservableProperty]
        private System.String _inImg;
        [Required(StringResourceKey.InOperatorIdRequired)]
        [ObservableProperty]
        private System.Int32 _inOperatorId;
        [Required(StringResourceKey.InRemarkRequired)]
        [ObservableProperty]
        private System.String _inRemark;
        [ObservableProperty]
        private System.String _inTime;
        [Required(StringResourceKey.InTypeRequired)]
        [ObservableProperty]
        private System.Int32 _inType;
        [Required(StringResourceKey.InWayIdRequired)]
        [ObservableProperty]
        private System.Int32 _inWayId;
        [Required(StringResourceKey.OpenTypeRequired)]
        [ObservableProperty]
        private System.Int32 _openType;
        [Required(StringResourceKey.OrderIdRequired)]
        [ObservableProperty]
        private System.String _orderId;
        [Required(StringResourceKey.OutCarColorRequired)]
        [ObservableProperty]
        private System.Int32 _outCarColor;
        [Required(StringResourceKey.OutCarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _outCarType;
        [Required(StringResourceKey.OutCpChangedRequired)]
        [ObservableProperty]
        private System.Int32 _outCpChanged;
        [Required(StringResourceKey.OutImgRequired)]
        [ObservableProperty]
        private System.String _outImg;
        [Required(StringResourceKey.OutOperatorIdRequired)]
        [ObservableProperty]
        private System.Int32 _outOperatorId;
        [ObservableProperty]
        private System.String _outTime;
        [Required(StringResourceKey.OutTypeRequired)]
        [ObservableProperty]
        private System.Int32 _outType;
        [Required(StringResourceKey.OutWayIdRequired)]
        [ObservableProperty]
        private System.Int32 _outWayId;
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

        public ParkingOutRecordActionWindowViewModel(AppDbContext appDbContext, ParkingOutRecordDao parkingOutRecordDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkingOutRecordDao = parkingOutRecordDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _amountMoneyValidationMessage;
        [ObservableProperty] private string _cardNoValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _chargeTypeValidationMessage;
        [ObservableProperty] private string _discountMoneyValidationMessage;
        [ObservableProperty] private string _inCarColorValidationMessage;
        [ObservableProperty] private string _inCarTypeValidationMessage;
        [ObservableProperty] private string _inCpChangedValidationMessage;
        [ObservableProperty] private string _inImgValidationMessage;
        [ObservableProperty] private string _inOperatorIdValidationMessage;
        [ObservableProperty] private string _inRemarkValidationMessage;
        [ObservableProperty] private string _inTimeValidationMessage;
        [ObservableProperty] private string _inTypeValidationMessage;
        [ObservableProperty] private string _inWayIdValidationMessage;
        [ObservableProperty] private string _openTypeValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _outCarColorValidationMessage;
        [ObservableProperty] private string _outCarTypeValidationMessage;
        [ObservableProperty] private string _outCpChangedValidationMessage;
        [ObservableProperty] private string _outImgValidationMessage;
        [ObservableProperty] private string _outOperatorIdValidationMessage;
        [ObservableProperty] private string _outTimeValidationMessage;
        [ObservableProperty] private string _outTypeValidationMessage;
        [ObservableProperty] private string _outWayIdValidationMessage;
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
            UpdateValidationMessage(nameof(CardNo));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(ChargeType));
            UpdateValidationMessage(nameof(DiscountMoney));
            UpdateValidationMessage(nameof(InCarColor));
            UpdateValidationMessage(nameof(InCarType));
            UpdateValidationMessage(nameof(InCpChanged));
            UpdateValidationMessage(nameof(InImg));
            UpdateValidationMessage(nameof(InOperatorId));
            UpdateValidationMessage(nameof(InRemark));
            UpdateValidationMessage(nameof(InTime));
            UpdateValidationMessage(nameof(InType));
            UpdateValidationMessage(nameof(InWayId));
            UpdateValidationMessage(nameof(OpenType));
            UpdateValidationMessage(nameof(OrderId));
            UpdateValidationMessage(nameof(OutCarColor));
            UpdateValidationMessage(nameof(OutCarType));
            UpdateValidationMessage(nameof(OutCpChanged));
            UpdateValidationMessage(nameof(OutImg));
            UpdateValidationMessage(nameof(OutOperatorId));
            UpdateValidationMessage(nameof(OutTime));
            UpdateValidationMessage(nameof(OutType));
            UpdateValidationMessage(nameof(OutWayId));
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
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(ChargeType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(InCarColor));
                    UpdateValidationMessage(nameof(InCarType));
                    UpdateValidationMessage(nameof(InCpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InRemark));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OpenType));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OutCarColor));
                    UpdateValidationMessage(nameof(OutCarType));
                    UpdateValidationMessage(nameof(OutCpChanged));
                    UpdateValidationMessage(nameof(OutImg));
                    UpdateValidationMessage(nameof(OutOperatorId));
                    UpdateValidationMessage(nameof(OutType));
                    UpdateValidationMessage(nameof(OutWayId));
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
                .WithTitle(I18nManager.GetString("UpdateParkingOutRecordPrompt"))
                .WithContent(I18nManager.GetString("ParkingOutRecordExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkingOutRecord = _parkingOutRecordDao.GetById(SelectedParkingOutRecord.Id);
                if (tempParkingOutRecord != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkingOutRecord.AmountMoney = AmountMoney;
                    tempParkingOutRecord.CardNo = CardNo;
                    tempParkingOutRecord.CarNo = CarNo;
                    tempParkingOutRecord.ChargeType = ChargeType;
                    tempParkingOutRecord.DiscountMoney = DiscountMoney;
                    tempParkingOutRecord.InCarColor = InCarColor;
                    tempParkingOutRecord.InCarType = InCarType;
                    tempParkingOutRecord.InCpChanged = InCpChanged;
                    tempParkingOutRecord.InImg = InImg;
                    tempParkingOutRecord.InOperatorId = InOperatorId;
                    tempParkingOutRecord.InRemark = InRemark;
                    tempParkingOutRecord.InTime = tempDt;
                    tempParkingOutRecord.InType = InType;
                    tempParkingOutRecord.InWayId = InWayId;
                    tempParkingOutRecord.OpenType = OpenType;
                    tempParkingOutRecord.OrderId = OrderId;
                    tempParkingOutRecord.OutCarColor = OutCarColor;
                    tempParkingOutRecord.OutCarType = OutCarType;
                    tempParkingOutRecord.OutCpChanged = OutCpChanged;
                    tempParkingOutRecord.OutImg = OutImg;
                    tempParkingOutRecord.OutOperatorId = OutOperatorId;
                    tempParkingOutRecord.OutTime = tempDt;
                    tempParkingOutRecord.OutType = OutType;
                    tempParkingOutRecord.OutWayId = OutWayId;
                    tempParkingOutRecord.PaidMoney = PaidMoney;
                    tempParkingOutRecord.PlateId = PlateId;
                    tempParkingOutRecord.RecStatus = RecStatus;
                    tempParkingOutRecord.Remark = Remark;
                    tempParkingOutRecord.UpdateDt = UpdateDt;
                    tempParkingOutRecord.UpdateUser = UpdateUser;
                    int result = _parkingOutRecordDao.Update(tempParkingOutRecord);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkingOutRecord),
                                     Title = I18nManager.GetString("UpdateParkingOutRecordPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkingOutRecordActionWindow", TokenManage.PARKINGOUTRECORD_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(ChargeType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(InCarColor));
                    UpdateValidationMessage(nameof(InCarType));
                    UpdateValidationMessage(nameof(InCpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InRemark));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OpenType));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OutCarColor));
                    UpdateValidationMessage(nameof(OutCarType));
                    UpdateValidationMessage(nameof(OutCpChanged));
                    UpdateValidationMessage(nameof(OutImg));
                    UpdateValidationMessage(nameof(OutOperatorId));
                    UpdateValidationMessage(nameof(OutType));
                    UpdateValidationMessage(nameof(OutWayId));
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
                var tempParkingOutRecord = _parkingOutRecordDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkingOutRecordDao.Add(new ParkingOutRecord{
                    AmountMoney = AmountMoney,
                    CardNo = CardNo,
                    CarNo = CarNo,
                    ChargeType = ChargeType,
                    DiscountMoney = DiscountMoney,
                    InCarColor = InCarColor,
                    InCarType = InCarType,
                    InCpChanged = InCpChanged,
                    InImg = InImg,
                    InOperatorId = InOperatorId,
                    InRemark = InRemark,
                    InTime = tempDt,
                    InType = InType,
                    InWayId = InWayId,
                    OpenType = OpenType,
                    OrderId = OrderId,
                    OutCarColor = OutCarColor,
                    OutCarType = OutCarType,
                    OutCpChanged = OutCpChanged,
                    OutImg = OutImg,
                    OutOperatorId = OutOperatorId,
                    OutTime = tempDt,
                    OutType = OutType,
                    OutWayId = OutWayId,
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
                                     CurrentModelType = typeof(ParkingOutRecord),
                            Title = I18nManager.GetString("CreateParkingOutRecordPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKINGOUTRECORDActionWindow", TokenManage.PARKINGOUTRECORD_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKINGOUTRECORD_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

