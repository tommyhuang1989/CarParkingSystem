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
    public partial class ParkWayActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkWayDao _parkWayDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkWay;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkWay _selectedParkWay;

        [Required(StringResourceKey.AmountRequired)]
        [ObservableProperty]
        private System.Decimal _amount;
        [Required(StringResourceKey.AreaIdRequired)]
        [ObservableProperty]
        private System.Int32 _areaId;
        [Required(StringResourceKey.CarInIdRequired)]
        [ObservableProperty]
        private System.Int32 _carInId;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.CarNoColorRequired)]
        [ObservableProperty]
        private System.Int32 _carNoColor;
        [Required(StringResourceKey.CarNoTypeRequired)]
        [ObservableProperty]
        private System.Int32 _carNoType;
        [Required(StringResourceKey.CarStatusRequired)]
        [ObservableProperty]
        private System.Int32 _carStatus;
        [Required(StringResourceKey.CarTypeIdRequired)]
        [ObservableProperty]
        private System.Int32 _carTypeId;
        [Required(StringResourceKey.ChangedCarNoRequired)]
        [ObservableProperty]
        private System.Int32 _changedCarNo;
        [Required(StringResourceKey.DiscountRequired)]
        [ObservableProperty]
        private System.Decimal _discount;
        [Required(StringResourceKey.DisplayRequired)]
        [ObservableProperty]
        private System.String _display;
        [Required(StringResourceKey.InImageRequired)]
        [ObservableProperty]
        private System.String _inImage;
        [ObservableProperty]
        private System.String _inTime;
        [Required(StringResourceKey.IsAllowEnterRequired)]
        [ObservableProperty]
        private System.Int32 _isAllowEnter;
        [Required(StringResourceKey.IsCsConfirmRequired)]
        [ObservableProperty]
        private System.Int32 _isCsConfirm;
        [Required(StringResourceKey.IsNeedAysnRequired)]
        [ObservableProperty]
        private System.Int32 _isNeedAysn;
        [Required(StringResourceKey.IsPaidRequired)]
        [ObservableProperty]
        private System.Int32 _isPaid;
        [Required(StringResourceKey.LastCarNoRequired)]
        [ObservableProperty]
        private System.String _lastCarNo;
        [ObservableProperty]
        private System.String _lastCarTime;
        [Required(StringResourceKey.OrderIdRequired)]
        [ObservableProperty]
        private System.String _orderId;
        [Required(StringResourceKey.PaidRequired)]
        [ObservableProperty]
        private System.Decimal _paid;
        [Required(StringResourceKey.PlateIdRequired)]
        [ObservableProperty]
        private System.String _plateId;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [Required(StringResourceKey.SpecialCarRequired)]
        [ObservableProperty]
        private System.Int32 _specialCar;
        [Required(StringResourceKey.TriggerFlagRequired)]
        [ObservableProperty]
        private System.Int32 _triggerFlag;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;
        [Required(StringResourceKey.VideoCallRequired)]
        [ObservableProperty]
        private System.Int32 _videoCall;
        [Required(StringResourceKey.VideoCallQrcodeRequired)]
        [ObservableProperty]
        private System.Int32 _videoCallQrcode;
        [ObservableProperty]
        private System.String _videoCallTime;
        [Required(StringResourceKey.VoiceRequired)]
        [ObservableProperty]
        private System.String _voice;
        [Required(StringResourceKey.WaitPayRequired)]
        [ObservableProperty]
        private System.Decimal _waitPay;
        [Required(StringResourceKey.WaittingCarNoRequired)]
        [ObservableProperty]
        private System.String _waittingCarNo;
        [Required(StringResourceKey.WaittingCarNoColorRequired)]
        [ObservableProperty]
        private System.Int32 _waittingCarNoColor;
        [Required(StringResourceKey.WaittingCarNoTypeRequired)]
        [ObservableProperty]
        private System.Int32 _waittingCarNoType;
        [Required(StringResourceKey.WaittingImgRequired)]
        [ObservableProperty]
        private System.String _waittingImg;
        [Required(StringResourceKey.WaittingPlateIdRequired)]
        [ObservableProperty]
        private System.String _waittingPlateId;
        [ObservableProperty]
        private System.String _waittingTime;
        [Required(StringResourceKey.WayCarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _wayCarType;
        [Required(StringResourceKey.WayConnectRequired)]
        [ObservableProperty]
        private System.Int32 _wayConnect;
        [Required(StringResourceKey.WayNameRequired)]
        [ObservableProperty]
        private System.String _wayName;
        [Required(StringResourceKey.WayNoRequired)]
        [ObservableProperty]
        private System.String _wayNo;
        [Required(StringResourceKey.WayStatusRequired)]
        [ObservableProperty]
        private System.Int32 _wayStatus;
        [Required(StringResourceKey.WayTypeRequired)]
        [ObservableProperty]
        private System.Int32 _wayType;

        public ParkWayActionWindowViewModel(AppDbContext appDbContext, ParkWayDao parkWayDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkWayDao = parkWayDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _amountValidationMessage;
        [ObservableProperty] private string _areaIdValidationMessage;
        [ObservableProperty] private string _carInIdValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _carNoColorValidationMessage;
        [ObservableProperty] private string _carNoTypeValidationMessage;
        [ObservableProperty] private string _carStatusValidationMessage;
        [ObservableProperty] private string _carTypeIdValidationMessage;
        [ObservableProperty] private string _changedCarNoValidationMessage;
        [ObservableProperty] private string _discountValidationMessage;
        [ObservableProperty] private string _displayValidationMessage;
        [ObservableProperty] private string _inImageValidationMessage;
        [ObservableProperty] private string _inTimeValidationMessage;
        [ObservableProperty] private string _isAllowEnterValidationMessage;
        [ObservableProperty] private string _isCsConfirmValidationMessage;
        [ObservableProperty] private string _isNeedAysnValidationMessage;
        [ObservableProperty] private string _isPaidValidationMessage;
        [ObservableProperty] private string _lastCarNoValidationMessage;
        [ObservableProperty] private string _lastCarTimeValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _paidValidationMessage;
        [ObservableProperty] private string _plateIdValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _specialCarValidationMessage;
        [ObservableProperty] private string _triggerFlagValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _videoCallValidationMessage;
        [ObservableProperty] private string _videoCallQrcodeValidationMessage;
        [ObservableProperty] private string _videoCallTimeValidationMessage;
        [ObservableProperty] private string _voiceValidationMessage;
        [ObservableProperty] private string _waitPayValidationMessage;
        [ObservableProperty] private string _waittingCarNoValidationMessage;
        [ObservableProperty] private string _waittingCarNoColorValidationMessage;
        [ObservableProperty] private string _waittingCarNoTypeValidationMessage;
        [ObservableProperty] private string _waittingImgValidationMessage;
        [ObservableProperty] private string _waittingPlateIdValidationMessage;
        [ObservableProperty] private string _waittingTimeValidationMessage;
        [ObservableProperty] private string _wayCarTypeValidationMessage;
        [ObservableProperty] private string _wayConnectValidationMessage;
        [ObservableProperty] private string _wayNameValidationMessage;
        [ObservableProperty] private string _wayNoValidationMessage;
        [ObservableProperty] private string _wayStatusValidationMessage;
        [ObservableProperty] private string _wayTypeValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(Amount));
            UpdateValidationMessage(nameof(AreaId));
            UpdateValidationMessage(nameof(CarInId));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(CarNoColor));
            UpdateValidationMessage(nameof(CarNoType));
            UpdateValidationMessage(nameof(CarStatus));
            UpdateValidationMessage(nameof(CarTypeId));
            UpdateValidationMessage(nameof(ChangedCarNo));
            UpdateValidationMessage(nameof(Discount));
            UpdateValidationMessage(nameof(Display));
            UpdateValidationMessage(nameof(InImage));
            UpdateValidationMessage(nameof(InTime));
            UpdateValidationMessage(nameof(IsAllowEnter));
            UpdateValidationMessage(nameof(IsCsConfirm));
            UpdateValidationMessage(nameof(IsNeedAysn));
            UpdateValidationMessage(nameof(IsPaid));
            UpdateValidationMessage(nameof(LastCarNo));
            UpdateValidationMessage(nameof(LastCarTime));
            UpdateValidationMessage(nameof(OrderId));
            UpdateValidationMessage(nameof(Paid));
            UpdateValidationMessage(nameof(PlateId));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(SpecialCar));
            UpdateValidationMessage(nameof(TriggerFlag));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
            UpdateValidationMessage(nameof(VideoCall));
            UpdateValidationMessage(nameof(VideoCallQrcode));
            UpdateValidationMessage(nameof(VideoCallTime));
            UpdateValidationMessage(nameof(Voice));
            UpdateValidationMessage(nameof(WaitPay));
            UpdateValidationMessage(nameof(WaittingCarNo));
            UpdateValidationMessage(nameof(WaittingCarNoColor));
            UpdateValidationMessage(nameof(WaittingCarNoType));
            UpdateValidationMessage(nameof(WaittingImg));
            UpdateValidationMessage(nameof(WaittingPlateId));
            UpdateValidationMessage(nameof(WaittingTime));
            UpdateValidationMessage(nameof(WayCarType));
            UpdateValidationMessage(nameof(WayConnect));
            UpdateValidationMessage(nameof(WayName));
            UpdateValidationMessage(nameof(WayNo));
            UpdateValidationMessage(nameof(WayStatus));
            UpdateValidationMessage(nameof(WayType));
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
                    UpdateValidationMessage(nameof(Amount));
                    UpdateValidationMessage(nameof(AreaId));
                    UpdateValidationMessage(nameof(CarInId));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarNoColor));
                    UpdateValidationMessage(nameof(CarNoType));
                    UpdateValidationMessage(nameof(CarStatus));
                    UpdateValidationMessage(nameof(CarTypeId));
                    UpdateValidationMessage(nameof(ChangedCarNo));
                    UpdateValidationMessage(nameof(Discount));
                    UpdateValidationMessage(nameof(Display));
                    UpdateValidationMessage(nameof(InImage));
                    UpdateValidationMessage(nameof(IsAllowEnter));
                    UpdateValidationMessage(nameof(IsCsConfirm));
                    UpdateValidationMessage(nameof(IsNeedAysn));
                    UpdateValidationMessage(nameof(IsPaid));
                    UpdateValidationMessage(nameof(LastCarNo));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(Paid));
                    UpdateValidationMessage(nameof(PlateId));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(SpecialCar));
                    UpdateValidationMessage(nameof(TriggerFlag));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(VideoCall));
                    UpdateValidationMessage(nameof(VideoCallQrcode));
                    UpdateValidationMessage(nameof(Voice));
                    UpdateValidationMessage(nameof(WaitPay));
                    UpdateValidationMessage(nameof(WaittingCarNo));
                    UpdateValidationMessage(nameof(WaittingCarNoColor));
                    UpdateValidationMessage(nameof(WaittingCarNoType));
                    UpdateValidationMessage(nameof(WaittingImg));
                    UpdateValidationMessage(nameof(WaittingPlateId));
                    UpdateValidationMessage(nameof(WayCarType));
                    UpdateValidationMessage(nameof(WayConnect));
                    UpdateValidationMessage(nameof(WayName));
                    UpdateValidationMessage(nameof(WayNo));
                    UpdateValidationMessage(nameof(WayStatus));
                    UpdateValidationMessage(nameof(WayType));
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
                .WithTitle(I18nManager.GetString("UpdateParkWayPrompt"))
                .WithContent(I18nManager.GetString("ParkWayExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkWay = _parkWayDao.GetById(SelectedParkWay.Id);
                if (tempParkWay != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkWay.Amount = Amount;
                    tempParkWay.AreaId = AreaId;
                    tempParkWay.CarInId = CarInId;
                    tempParkWay.CarNo = CarNo;
                    tempParkWay.CarNoColor = CarNoColor;
                    tempParkWay.CarNoType = CarNoType;
                    tempParkWay.CarStatus = CarStatus;
                    tempParkWay.CarTypeId = CarTypeId;
                    tempParkWay.ChangedCarNo = ChangedCarNo;
                    tempParkWay.Discount = Discount;
                    tempParkWay.Display = Display;
                    tempParkWay.InImage = InImage;
                    tempParkWay.InTime = tempDt;
                    tempParkWay.IsAllowEnter = IsAllowEnter;
                    tempParkWay.IsCsConfirm = IsCsConfirm;
                    tempParkWay.IsNeedAysn = IsNeedAysn;
                    tempParkWay.IsPaid = IsPaid;
                    tempParkWay.LastCarNo = LastCarNo;
                    tempParkWay.LastCarTime = tempDt;
                    tempParkWay.OrderId = OrderId;
                    tempParkWay.Paid = Paid;
                    tempParkWay.PlateId = PlateId;
                    tempParkWay.RecStatus = RecStatus;
                    tempParkWay.Remark = Remark;
                    tempParkWay.SpecialCar = SpecialCar;
                    tempParkWay.TriggerFlag = TriggerFlag;
                    tempParkWay.UpdateDt = UpdateDt;
                    tempParkWay.UpdateUser = UpdateUser;
                    tempParkWay.VideoCall = VideoCall;
                    tempParkWay.VideoCallQrcode = VideoCallQrcode;
                    tempParkWay.VideoCallTime = tempDt;
                    tempParkWay.Voice = Voice;
                    tempParkWay.WaitPay = WaitPay;
                    tempParkWay.WaittingCarNo = WaittingCarNo;
                    tempParkWay.WaittingCarNoColor = WaittingCarNoColor;
                    tempParkWay.WaittingCarNoType = WaittingCarNoType;
                    tempParkWay.WaittingImg = WaittingImg;
                    tempParkWay.WaittingPlateId = WaittingPlateId;
                    tempParkWay.WaittingTime = tempDt;
                    tempParkWay.WayCarType = WayCarType;
                    tempParkWay.WayConnect = WayConnect;
                    tempParkWay.WayName = WayName;
                    tempParkWay.WayNo = WayNo;
                    tempParkWay.WayStatus = WayStatus;
                    tempParkWay.WayType = WayType;
                    int result = _parkWayDao.Update(tempParkWay);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkWay),
                                     Title = I18nManager.GetString("UpdateParkWayPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkWayActionWindow", TokenManage.PARKWAY_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(Amount));
                    UpdateValidationMessage(nameof(AreaId));
                    UpdateValidationMessage(nameof(CarInId));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarNoColor));
                    UpdateValidationMessage(nameof(CarNoType));
                    UpdateValidationMessage(nameof(CarStatus));
                    UpdateValidationMessage(nameof(CarTypeId));
                    UpdateValidationMessage(nameof(ChangedCarNo));
                    UpdateValidationMessage(nameof(Discount));
                    UpdateValidationMessage(nameof(Display));
                    UpdateValidationMessage(nameof(InImage));
                    UpdateValidationMessage(nameof(IsAllowEnter));
                    UpdateValidationMessage(nameof(IsCsConfirm));
                    UpdateValidationMessage(nameof(IsNeedAysn));
                    UpdateValidationMessage(nameof(IsPaid));
                    UpdateValidationMessage(nameof(LastCarNo));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(Paid));
                    UpdateValidationMessage(nameof(PlateId));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(SpecialCar));
                    UpdateValidationMessage(nameof(TriggerFlag));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(VideoCall));
                    UpdateValidationMessage(nameof(VideoCallQrcode));
                    UpdateValidationMessage(nameof(Voice));
                    UpdateValidationMessage(nameof(WaitPay));
                    UpdateValidationMessage(nameof(WaittingCarNo));
                    UpdateValidationMessage(nameof(WaittingCarNoColor));
                    UpdateValidationMessage(nameof(WaittingCarNoType));
                    UpdateValidationMessage(nameof(WaittingImg));
                    UpdateValidationMessage(nameof(WaittingPlateId));
                    UpdateValidationMessage(nameof(WayCarType));
                    UpdateValidationMessage(nameof(WayConnect));
                    UpdateValidationMessage(nameof(WayName));
                    UpdateValidationMessage(nameof(WayNo));
                    UpdateValidationMessage(nameof(WayStatus));
                    UpdateValidationMessage(nameof(WayType));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkWay = _parkWayDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkWayDao.Add(new ParkWay{
                    Amount = Amount,
                    AreaId = AreaId,
                    CarInId = CarInId,
                    CarNo = CarNo,
                    CarNoColor = CarNoColor,
                    CarNoType = CarNoType,
                    CarStatus = CarStatus,
                    CarTypeId = CarTypeId,
                    ChangedCarNo = ChangedCarNo,
                    Discount = Discount,
                    Display = Display,
                    InImage = InImage,
                    InTime = tempDt,
                    IsAllowEnter = IsAllowEnter,
                    IsCsConfirm = IsCsConfirm,
                    IsNeedAysn = IsNeedAysn,
                    IsPaid = IsPaid,
                    LastCarNo = LastCarNo,
                    LastCarTime = tempDt,
                    OrderId = OrderId,
                    Paid = Paid,
                    PlateId = PlateId,
                    RecStatus = RecStatus,
                    Remark = Remark,
                    SpecialCar = SpecialCar,
                    TriggerFlag = TriggerFlag,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                    VideoCall = VideoCall,
                    VideoCallQrcode = VideoCallQrcode,
                    VideoCallTime = tempDt,
                    Voice = Voice,
                    WaitPay = WaitPay,
                    WaittingCarNo = WaittingCarNo,
                    WaittingCarNoColor = WaittingCarNoColor,
                    WaittingCarNoType = WaittingCarNoType,
                    WaittingImg = WaittingImg,
                    WaittingPlateId = WaittingPlateId,
                    WaittingTime = tempDt,
                    WayCarType = WayCarType,
                    WayConnect = WayConnect,
                    WayName = WayName,
                    WayNo = WayNo,
                    WayStatus = WayStatus,
                    WayType = WayType,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkWay),
                            Title = I18nManager.GetString("CreateParkWayPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKWAYActionWindow", TokenManage.PARKWAY_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKWAY_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

