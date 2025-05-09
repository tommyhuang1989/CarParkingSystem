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
    public partial class OrderRefundActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private OrderRefundDao _orderRefundDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddOrderRefund;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private OrderRefund _selectedOrderRefund;

        [Required(StringResourceKey.ProductTypeRequired)]
        [ObservableProperty]
        private System.Int32 _productType;
        [Required(StringResourceKey.ProductIdRequired)]
        [ObservableProperty]
        private System.String _productId;
        [Required(StringResourceKey.BuyerRequired)]
        [ObservableProperty]
        private System.String _buyer;
        [Required(StringResourceKey.OrderMoneyRequired)]
        [ObservableProperty]
        private System.Int32 _orderMoney;
        [Required(StringResourceKey.RefundTypeRequired)]
        [ObservableProperty]
        private System.Int32 _refundType;
        [Required(StringResourceKey.RefundMoneyRequired)]
        [ObservableProperty]
        private System.Int32 _refundMoney;
        [Required(StringResourceKey.ReasonRequired)]
        [ObservableProperty]
        private System.String _reason;
        [Required(StringResourceKey.RefundStatusRequired)]
        [ObservableProperty]
        private System.Int32 _refundStatus;
        [Required(StringResourceKey.RefundRemarkRequired)]
        [ObservableProperty]
        private System.String _refundRemark;
        [Required(StringResourceKey.CreateUserRequired)]
        [ObservableProperty]
        private System.Int32 _createUser;
        [ObservableProperty]
        private System.String _createDate;
        [Required(StringResourceKey.PayOrderRequired)]
        [ObservableProperty]
        private System.Int32 _payOrder;
        [Required(StringResourceKey.RefundOrderIdRequired)]
        [ObservableProperty]
        private System.Int32 _refundOrderId;
        [Required(StringResourceKey.RefundTransactionIdRequired)]
        [ObservableProperty]
        private System.String _refundTransactionId;
        [Required(StringResourceKey.MerchantRequired)]
        [ObservableProperty]
        private System.String _merchant;
        [Required(StringResourceKey.TransactionIdRequired)]
        [ObservableProperty]
        private System.String _transactionId;
        [Required(StringResourceKey.ClientTypeRequired)]
        [ObservableProperty]
        private System.Int32 _clientType;
        [Required(StringResourceKey.ClientIdRequired)]
        [ObservableProperty]
        private System.String _clientId;

        public OrderRefundActionWindowViewModel(AppDbContext appDbContext, OrderRefundDao orderRefundDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _orderRefundDao = orderRefundDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _productTypeValidationMessage;
        [ObservableProperty] private string _productIdValidationMessage;
        [ObservableProperty] private string _buyerValidationMessage;
        [ObservableProperty] private string _orderMoneyValidationMessage;
        [ObservableProperty] private string _refundTypeValidationMessage;
        [ObservableProperty] private string _refundMoneyValidationMessage;
        [ObservableProperty] private string _reasonValidationMessage;
        [ObservableProperty] private string _refundStatusValidationMessage;
        [ObservableProperty] private string _refundRemarkValidationMessage;
        [ObservableProperty] private string _createUserValidationMessage;
        [ObservableProperty] private string _createDateValidationMessage;
        [ObservableProperty] private string _payOrderValidationMessage;
        [ObservableProperty] private string _refundOrderIdValidationMessage;
        [ObservableProperty] private string _refundTransactionIdValidationMessage;
        [ObservableProperty] private string _merchantValidationMessage;
        [ObservableProperty] private string _transactionIdValidationMessage;
        [ObservableProperty] private string _clientTypeValidationMessage;
        [ObservableProperty] private string _clientIdValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(ProductType));
            UpdateValidationMessage(nameof(ProductId));
            UpdateValidationMessage(nameof(Buyer));
            UpdateValidationMessage(nameof(OrderMoney));
            UpdateValidationMessage(nameof(RefundType));
            UpdateValidationMessage(nameof(RefundMoney));
            UpdateValidationMessage(nameof(Reason));
            UpdateValidationMessage(nameof(RefundStatus));
            UpdateValidationMessage(nameof(RefundRemark));
            UpdateValidationMessage(nameof(CreateUser));
            UpdateValidationMessage(nameof(CreateDate));
            UpdateValidationMessage(nameof(PayOrder));
            UpdateValidationMessage(nameof(RefundOrderId));
            UpdateValidationMessage(nameof(RefundTransactionId));
            UpdateValidationMessage(nameof(Merchant));
            UpdateValidationMessage(nameof(TransactionId));
            UpdateValidationMessage(nameof(ClientType));
            UpdateValidationMessage(nameof(ClientId));
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
                    UpdateValidationMessage(nameof(ProductType));
                    UpdateValidationMessage(nameof(ProductId));
                    UpdateValidationMessage(nameof(Buyer));
                    UpdateValidationMessage(nameof(OrderMoney));
                    UpdateValidationMessage(nameof(RefundType));
                    UpdateValidationMessage(nameof(RefundMoney));
                    UpdateValidationMessage(nameof(Reason));
                    UpdateValidationMessage(nameof(RefundStatus));
                    UpdateValidationMessage(nameof(RefundRemark));
                    UpdateValidationMessage(nameof(CreateUser));
                    UpdateValidationMessage(nameof(PayOrder));
                    UpdateValidationMessage(nameof(RefundOrderId));
                    UpdateValidationMessage(nameof(RefundTransactionId));
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(TransactionId));
                    UpdateValidationMessage(nameof(ClientType));
                    UpdateValidationMessage(nameof(ClientId));
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
                .WithTitle(I18nManager.GetString("UpdateOrderRefundPrompt"))
                .WithContent(I18nManager.GetString("OrderRefundExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempOrderRefund = _orderRefundDao.GetById(SelectedOrderRefund.Id);
                if (tempOrderRefund != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempOrderRefund.ProductType = ProductType;
                    tempOrderRefund.ProductId = ProductId;
                    tempOrderRefund.Buyer = Buyer;
                    tempOrderRefund.OrderMoney = OrderMoney;
                    tempOrderRefund.RefundType = RefundType;
                    tempOrderRefund.RefundMoney = RefundMoney;
                    tempOrderRefund.Reason = Reason;
                    tempOrderRefund.RefundStatus = RefundStatus;
                    tempOrderRefund.RefundRemark = RefundRemark;
                    tempOrderRefund.CreateUser = CreateUser;
                    tempOrderRefund.CreateDate = tempDt;
                    tempOrderRefund.PayOrder = PayOrder;
                    tempOrderRefund.RefundOrderId = RefundOrderId;
                    tempOrderRefund.RefundTransactionId = RefundTransactionId;
                    tempOrderRefund.Merchant = Merchant;
                    tempOrderRefund.TransactionId = TransactionId;
                    tempOrderRefund.ClientType = ClientType;
                    tempOrderRefund.ClientId = ClientId;
                    int result = _orderRefundDao.Update(tempOrderRefund);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(OrderRefund),
                                     Title = I18nManager.GetString("UpdateOrderRefundPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close OrderRefundActionWindow", TokenManage.ORDERREFUND_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(ProductType));
                    UpdateValidationMessage(nameof(ProductId));
                    UpdateValidationMessage(nameof(Buyer));
                    UpdateValidationMessage(nameof(OrderMoney));
                    UpdateValidationMessage(nameof(RefundType));
                    UpdateValidationMessage(nameof(RefundMoney));
                    UpdateValidationMessage(nameof(Reason));
                    UpdateValidationMessage(nameof(RefundStatus));
                    UpdateValidationMessage(nameof(RefundRemark));
                    UpdateValidationMessage(nameof(CreateUser));
                    UpdateValidationMessage(nameof(PayOrder));
                    UpdateValidationMessage(nameof(RefundOrderId));
                    UpdateValidationMessage(nameof(RefundTransactionId));
                    UpdateValidationMessage(nameof(Merchant));
                    UpdateValidationMessage(nameof(TransactionId));
                    UpdateValidationMessage(nameof(ClientType));
                    UpdateValidationMessage(nameof(ClientId));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempOrderRefund = _orderRefundDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_orderRefundDao.Add(new OrderRefund{
                    ProductType = ProductType,
                    ProductId = ProductId,
                    Buyer = Buyer,
                    OrderMoney = OrderMoney,
                    RefundType = RefundType,
                    RefundMoney = RefundMoney,
                    Reason = Reason,
                    RefundStatus = RefundStatus,
                    RefundRemark = RefundRemark,
                    CreateUser = CreateUser,
                    CreateDate = tempDt,
                    PayOrder = PayOrder,
                    RefundOrderId = RefundOrderId,
                    RefundTransactionId = RefundTransactionId,
                    Merchant = Merchant,
                    TransactionId = TransactionId,
                    ClientType = ClientType,
                    ClientId = ClientId,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(OrderRefund),
                            Title = I18nManager.GetString("CreateOrderRefundPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close ORDERREFUNDActionWindow", TokenManage.ORDERREFUND_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.ORDERREFUND_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

