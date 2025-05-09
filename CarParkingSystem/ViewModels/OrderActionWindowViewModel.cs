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
    public partial class OrderActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private OrderDao _orderDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddOrder;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private Order _selectedOrder;

        [Required(StringResourceKey.MerchantRequired)]
        [ObservableProperty]
        private System.String _merchant;
        [Required(StringResourceKey.ProductTypeRequired)]
        [ObservableProperty]
        private System.Int32 _productType;
        [Required(StringResourceKey.ProductIdRequired)]
        [ObservableProperty]
        private System.String _productId;
        [Required(StringResourceKey.BuyerRequired)]
        [ObservableProperty]
        private System.String _buyer;
        [Required(StringResourceKey.PayOrderRequired)]
        [ObservableProperty]
        private System.Int32 _payOrder;
        [Required(StringResourceKey.PayNameRequired)]
        [ObservableProperty]
        private System.Int32 _payName;
        [Required(StringResourceKey.PayMoneyRequired)]
        [ObservableProperty]
        private System.Int32 _payMoney;
        [ObservableProperty]
        private System.String _createDate;
        [Required(StringResourceKey.PayStatusRequired)]
        [ObservableProperty]
        private System.Int32 _payStatus;
        [Required(StringResourceKey.PayTypeRequired)]
        [ObservableProperty]
        private System.Int32 _payType;
        [Required(StringResourceKey.ClientTypeRequired)]
        [ObservableProperty]
        private System.Int32 _clientType;
        [Required(StringResourceKey.ClientIdRequired)]
        [ObservableProperty]
        private System.String _clientId;
        [Required(StringResourceKey.PayUrlRequired)]
        [ObservableProperty]
        private System.String _payUrl;
        [ObservableProperty]
        private System.String _expireDate;
        [Required(StringResourceKey.IsProfitSharingRequired)]
        [ObservableProperty]
        private System.Int32 _isProfitSharing;
        [Required(StringResourceKey.TransactionIdRequired)]
        [ObservableProperty]
        private System.String _transactionId;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [Required(StringResourceKey.PhoneRequired)]
        [ObservableProperty]
        private System.String _phone;
        [Required(StringResourceKey.BuyNumberRequired)]
        [ObservableProperty]
        private System.Int32 _buyNumber;
        [ObservableProperty]
        private System.String _startTime;
        [ObservableProperty]
        private System.String _endTime;
        [Required(StringResourceKey.InvoiceRequired)]
        [ObservableProperty]
        private System.String _invoice;

        public OrderActionWindowViewModel(AppDbContext appDbContext, OrderDao orderDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _orderDao = orderDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _merchantValidationMessage;
        [ObservableProperty] private string _productTypeValidationMessage;
        [ObservableProperty] private string _productIdValidationMessage;
        [ObservableProperty] private string _buyerValidationMessage;
        [ObservableProperty] private string _payOrderValidationMessage;
        [ObservableProperty] private string _payNameValidationMessage;
        [ObservableProperty] private string _payMoneyValidationMessage;
        [ObservableProperty] private string _createDateValidationMessage;
        [ObservableProperty] private string _payStatusValidationMessage;
        [ObservableProperty] private string _payTypeValidationMessage;
        [ObservableProperty] private string _clientTypeValidationMessage;
        [ObservableProperty] private string _clientIdValidationMessage;
        [ObservableProperty] private string _payUrlValidationMessage;
        [ObservableProperty] private string _expireDateValidationMessage;
        [ObservableProperty] private string _isProfitSharingValidationMessage;
        [ObservableProperty] private string _transactionIdValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _phoneValidationMessage;
        [ObservableProperty] private string _buyNumberValidationMessage;
        [ObservableProperty] private string _startTimeValidationMessage;
        [ObservableProperty] private string _endTimeValidationMessage;
        [ObservableProperty] private string _invoiceValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(Merchant));
            UpdateValidationMessage(nameof(ProductType));
            UpdateValidationMessage(nameof(ProductId));
            UpdateValidationMessage(nameof(Buyer));
            UpdateValidationMessage(nameof(PayOrder));
            UpdateValidationMessage(nameof(PayName));
            UpdateValidationMessage(nameof(PayMoney));
            UpdateValidationMessage(nameof(CreateDate));
            UpdateValidationMessage(nameof(PayStatus));
            UpdateValidationMessage(nameof(PayType));
            UpdateValidationMessage(nameof(ClientType));
            UpdateValidationMessage(nameof(ClientId));
            UpdateValidationMessage(nameof(PayUrl));
            UpdateValidationMessage(nameof(ExpireDate));
            UpdateValidationMessage(nameof(IsProfitSharing));
            UpdateValidationMessage(nameof(TransactionId));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(Phone));
            UpdateValidationMessage(nameof(BuyNumber));
            UpdateValidationMessage(nameof(StartTime));
            UpdateValidationMessage(nameof(EndTime));
            UpdateValidationMessage(nameof(Invoice));
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
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(ProductType));
                    UpdateValidationMessage(nameof(ProductId));
                    UpdateValidationMessage(nameof(Buyer));
                    UpdateValidationMessage(nameof(PayOrder));
                    UpdateValidationMessage(nameof(PayName));
                    UpdateValidationMessage(nameof(PayMoney));
                    UpdateValidationMessage(nameof(PayStatus));
                    UpdateValidationMessage(nameof(PayType));
                    UpdateValidationMessage(nameof(ClientType));
                    UpdateValidationMessage(nameof(ClientId));
                    UpdateValidationMessage(nameof(PayUrl));
                    UpdateValidationMessage(nameof(IsProfitSharing));
                    UpdateValidationMessage(nameof(TransactionId));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(Phone));
                    UpdateValidationMessage(nameof(BuyNumber));
                    UpdateValidationMessage(nameof(Invoice));
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
                .WithTitle(I18nManager.GetString("UpdateOrderPrompt"))
                .WithContent(I18nManager.GetString("OrderExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempOrder = _orderDao.GetById(SelectedOrder.Id);
                if (tempOrder != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempOrder.Merchant = Merchant;
                    tempOrder.ProductType = ProductType;
                    tempOrder.ProductId = ProductId;
                    tempOrder.Buyer = Buyer;
                    tempOrder.PayOrder = PayOrder;
                    tempOrder.PayName = PayName;
                    tempOrder.PayMoney = PayMoney;
                    tempOrder.CreateDate = tempDt;
                    tempOrder.PayStatus = PayStatus;
                    tempOrder.PayType = PayType;
                    tempOrder.ClientType = ClientType;
                    tempOrder.ClientId = ClientId;
                    tempOrder.PayUrl = PayUrl;
                    tempOrder.ExpireDate = tempDt;
                    tempOrder.IsProfitSharing = IsProfitSharing;
                    tempOrder.TransactionId = TransactionId;
                    tempOrder.Remark = Remark;
                    tempOrder.Phone = Phone;
                    tempOrder.BuyNumber = BuyNumber;
                    tempOrder.StartTime = tempDt;
                    tempOrder.EndTime = tempDt;
                    tempOrder.Invoice = Invoice;
                    int result = _orderDao.Update(tempOrder);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(Order),
                                     Title = I18nManager.GetString("UpdateOrderPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close OrderActionWindow", TokenManage.ORDER_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(ProductType));
                    UpdateValidationMessage(nameof(ProductId));
                    UpdateValidationMessage(nameof(Buyer));
                    UpdateValidationMessage(nameof(PayOrder));
                    UpdateValidationMessage(nameof(PayName));
                    UpdateValidationMessage(nameof(PayMoney));
                    UpdateValidationMessage(nameof(PayStatus));
                    UpdateValidationMessage(nameof(PayType));
                    UpdateValidationMessage(nameof(ClientType));
                    UpdateValidationMessage(nameof(ClientId));
                    UpdateValidationMessage(nameof(PayUrl));
                    UpdateValidationMessage(nameof(IsProfitSharing));
                    UpdateValidationMessage(nameof(TransactionId));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(Phone));
                    UpdateValidationMessage(nameof(BuyNumber));
                    UpdateValidationMessage(nameof(Invoice));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempOrder = _orderDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_orderDao.Add(new Order{
                    Merchant = Merchant,
                    ProductType = ProductType,
                    ProductId = ProductId,
                    Buyer = Buyer,
                    PayOrder = PayOrder,
                    PayName = PayName,
                    PayMoney = PayMoney,
                    CreateDate = tempDt,
                    PayStatus = PayStatus,
                    PayType = PayType,
                    ClientType = ClientType,
                    ClientId = ClientId,
                    PayUrl = PayUrl,
                    ExpireDate = tempDt,
                    IsProfitSharing = IsProfitSharing,
                    TransactionId = TransactionId,
                    Remark = Remark,
                    Phone = Phone,
                    BuyNumber = BuyNumber,
                    StartTime = tempDt,
                    EndTime = tempDt,
                    Invoice = Invoice,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(Order),
                            Title = I18nManager.GetString("CreateOrderPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close ORDERActionWindow", TokenManage.ORDER_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.ORDER_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

