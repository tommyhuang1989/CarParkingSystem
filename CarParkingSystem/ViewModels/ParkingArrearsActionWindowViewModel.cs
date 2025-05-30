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
    public partial class ParkingArrearsActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkingArrearsDao _parkingArrearsDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkingArrears;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkingArrears _selectedParkingArrears;

        [Required(StringResourceKey.AmountMoneyRequired)]
        [ObservableProperty]
        private System.String _amountMoney;
        [Required(StringResourceKey.CarColorRequired)]
        [ObservableProperty]
        private System.Int32 _carColor;
        [Required(StringResourceKey.CardNoRequired)]
        [ObservableProperty]
        private System.Int32 _cardNo;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.CarOutIdRequired)]
        [ObservableProperty]
        private System.Int32 _carOutId;
        [Required(StringResourceKey.CarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _carType;
        [Required(StringResourceKey.DiscountMoneyRequired)]
        [ObservableProperty]
        private System.String _discountMoney;
        [Required(StringResourceKey.FeeRequired)]
        [ObservableProperty]
        private System.Decimal _fee;
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
        [Required(StringResourceKey.OrderIdRequired)]
        [ObservableProperty]
        private System.String _orderId;
        [Required(StringResourceKey.OutImgRequired)]
        [ObservableProperty]
        private System.String _outImg;
        [Required(StringResourceKey.OutOperatorIdRequired)]
        [ObservableProperty]
        private System.Int32 _outOperatorId;
        [Required(StringResourceKey.OutRemarkRequired)]
        [ObservableProperty]
        private System.String _outRemark;
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
        private System.String _paidMoney;

        public ParkingArrearsActionWindowViewModel(AppDbContext appDbContext, ParkingArrearsDao parkingArrearsDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkingArrearsDao = parkingArrearsDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _amountMoneyValidationMessage;
        [ObservableProperty] private string _carColorValidationMessage;
        [ObservableProperty] private string _cardNoValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _carOutIdValidationMessage;
        [ObservableProperty] private string _carTypeValidationMessage;
        [ObservableProperty] private string _discountMoneyValidationMessage;
        [ObservableProperty] private string _feeValidationMessage;
        [ObservableProperty] private string _inImgValidationMessage;
        [ObservableProperty] private string _inOperatorIdValidationMessage;
        [ObservableProperty] private string _inRemarkValidationMessage;
        [ObservableProperty] private string _inTimeValidationMessage;
        [ObservableProperty] private string _inTypeValidationMessage;
        [ObservableProperty] private string _inWayIdValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _outImgValidationMessage;
        [ObservableProperty] private string _outOperatorIdValidationMessage;
        [ObservableProperty] private string _outRemarkValidationMessage;
        [ObservableProperty] private string _outTimeValidationMessage;
        [ObservableProperty] private string _outTypeValidationMessage;
        [ObservableProperty] private string _outWayIdValidationMessage;
        [ObservableProperty] private string _paidMoneyValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AmountMoney));
            UpdateValidationMessage(nameof(CarColor));
            UpdateValidationMessage(nameof(CardNo));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(CarOutId));
            UpdateValidationMessage(nameof(CarType));
            UpdateValidationMessage(nameof(DiscountMoney));
            UpdateValidationMessage(nameof(Fee));
            UpdateValidationMessage(nameof(InImg));
            UpdateValidationMessage(nameof(InOperatorId));
            UpdateValidationMessage(nameof(InRemark));
            UpdateValidationMessage(nameof(InTime));
            UpdateValidationMessage(nameof(InType));
            UpdateValidationMessage(nameof(InWayId));
            UpdateValidationMessage(nameof(OrderId));
            UpdateValidationMessage(nameof(OutImg));
            UpdateValidationMessage(nameof(OutOperatorId));
            UpdateValidationMessage(nameof(OutRemark));
            UpdateValidationMessage(nameof(OutTime));
            UpdateValidationMessage(nameof(OutType));
            UpdateValidationMessage(nameof(OutWayId));
            UpdateValidationMessage(nameof(PaidMoney));
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
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarOutId));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(Fee));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InRemark));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OutImg));
                    UpdateValidationMessage(nameof(OutOperatorId));
                    UpdateValidationMessage(nameof(OutRemark));
                    UpdateValidationMessage(nameof(OutType));
                    UpdateValidationMessage(nameof(OutWayId));
                    UpdateValidationMessage(nameof(PaidMoney));
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
                .WithTitle(I18nManager.GetString("UpdateParkingArrearsPrompt"))
                .WithContent(I18nManager.GetString("ParkingArrearsExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkingArrears = _parkingArrearsDao.GetById(SelectedParkingArrears.Id);
                if (tempParkingArrears != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkingArrears.AmountMoney = AmountMoney;
                    tempParkingArrears.CarColor = CarColor;
                    tempParkingArrears.CardNo = CardNo;
                    tempParkingArrears.CarNo = CarNo;
                    tempParkingArrears.CarOutId = CarOutId;
                    tempParkingArrears.CarType = CarType;
                    tempParkingArrears.DiscountMoney = DiscountMoney;
                    tempParkingArrears.Fee = Fee;
                    tempParkingArrears.InImg = InImg;
                    tempParkingArrears.InOperatorId = InOperatorId;
                    tempParkingArrears.InRemark = InRemark;
                    tempParkingArrears.InTime = tempDt;
                    tempParkingArrears.InType = InType;
                    tempParkingArrears.InWayId = InWayId;
                    tempParkingArrears.OrderId = OrderId;
                    tempParkingArrears.OutImg = OutImg;
                    tempParkingArrears.OutOperatorId = OutOperatorId;
                    tempParkingArrears.OutRemark = OutRemark;
                    tempParkingArrears.OutTime = tempDt;
                    tempParkingArrears.OutType = OutType;
                    tempParkingArrears.OutWayId = OutWayId;
                    tempParkingArrears.PaidMoney = PaidMoney;
                    int result = _parkingArrearsDao.Update(tempParkingArrears);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkingArrears),
                                     Title = I18nManager.GetString("UpdateParkingArrearsPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkingArrearsActionWindow", TokenManage.PARKINGARREARS_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarOutId));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(DiscountMoney));
                    UpdateValidationMessage(nameof(Fee));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InOperatorId));
                    UpdateValidationMessage(nameof(InRemark));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(OutImg));
                    UpdateValidationMessage(nameof(OutOperatorId));
                    UpdateValidationMessage(nameof(OutRemark));
                    UpdateValidationMessage(nameof(OutType));
                    UpdateValidationMessage(nameof(OutWayId));
                    UpdateValidationMessage(nameof(PaidMoney));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkingArrears = _parkingArrearsDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkingArrearsDao.Add(new ParkingArrears{
                    AmountMoney = AmountMoney,
                    CarColor = CarColor,
                    CardNo = CardNo,
                    CarNo = CarNo,
                    CarOutId = CarOutId,
                    CarType = CarType,
                    DiscountMoney = DiscountMoney,
                    Fee = Fee,
                    InImg = InImg,
                    InOperatorId = InOperatorId,
                    InRemark = InRemark,
                    InTime = tempDt,
                    InType = InType,
                    InWayId = InWayId,
                    OrderId = OrderId,
                    OutImg = OutImg,
                    OutOperatorId = OutOperatorId,
                    OutRemark = OutRemark,
                    OutTime = tempDt,
                    OutType = OutType,
                    OutWayId = OutWayId,
                    PaidMoney = PaidMoney,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkingArrears),
                            Title = I18nManager.GetString("CreateParkingArrearsPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKINGARREARSActionWindow", TokenManage.PARKINGARREARS_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKINGARREARS_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

