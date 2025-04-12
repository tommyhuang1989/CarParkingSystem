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
    public partial class CarVisitorActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private CarVisitorDao _carVisitorDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddCarVisitor;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private CarVisitor _selectedCarVisitor;

        [Required(StringResourceKey.CardNoRequired)]
        [ObservableProperty]
        private System.String _cardNo;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.ClientRequired)]
        [ObservableProperty]
        private System.Int32 _client;
        [Required(StringResourceKey.ClientIdRequired)]
        [ObservableProperty]
        private System.String _clientId;
        [ObservableProperty]
        private System.String _createDate;
        [ObservableProperty]
        private System.String _endDate;
        [Required(StringResourceKey.OrderIdRequired)]
        [ObservableProperty]
        private System.String _orderId;
        [Required(StringResourceKey.PhoneRequired)]
        [ObservableProperty]
        private System.String _phone;
        [Required(StringResourceKey.RemarkRequired)]
        [ObservableProperty]
        private System.String _remark;
        [ObservableProperty]
        private System.String _startDate;
        [Required(StringResourceKey.TrueNameRequired)]
        [ObservableProperty]
        private System.String _trueName;
        [Required(StringResourceKey.VisitorHouseRequired)]
        [ObservableProperty]
        private System.String _visitorHouse;

        public CarVisitorActionWindowViewModel(AppDbContext appDbContext, CarVisitorDao carVisitorDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _carVisitorDao = carVisitorDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _cardNoValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _clientValidationMessage;
        [ObservableProperty] private string _clientIdValidationMessage;
        [ObservableProperty] private string _createDateValidationMessage;
        [ObservableProperty] private string _endDateValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _phoneValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _startDateValidationMessage;
        [ObservableProperty] private string _trueNameValidationMessage;
        [ObservableProperty] private string _visitorHouseValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CardNo));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(Client));
            UpdateValidationMessage(nameof(ClientId));
            UpdateValidationMessage(nameof(CreateDate));
            UpdateValidationMessage(nameof(EndDate));
            UpdateValidationMessage(nameof(OrderId));
            UpdateValidationMessage(nameof(Phone));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(StartDate));
            UpdateValidationMessage(nameof(TrueName));
            UpdateValidationMessage(nameof(VisitorHouse));
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
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(Client));
                    UpdateValidationMessage(nameof(ClientId));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(Phone));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(TrueName));
                    UpdateValidationMessage(nameof(VisitorHouse));
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
                .WithTitle(I18nManager.GetString("UpdateCarVisitorPrompt"))
                .WithContent(I18nManager.GetString("CarVisitorExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempCarVisitor = _carVisitorDao.GetById(SelectedCarVisitor.Id);
                if (tempCarVisitor != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempCarVisitor.CardNo = CardNo;
                    tempCarVisitor.CarNo = CarNo;
                    tempCarVisitor.Client = Client;
                    tempCarVisitor.ClientId = ClientId;
                    tempCarVisitor.CreateDate = tempDt;
                    tempCarVisitor.EndDate = tempDt;
                    tempCarVisitor.OrderId = OrderId;
                    tempCarVisitor.Phone = Phone;
                    tempCarVisitor.Remark = Remark;
                    tempCarVisitor.StartDate = tempDt;
                    tempCarVisitor.TrueName = TrueName;
                    tempCarVisitor.VisitorHouse = VisitorHouse;
                    int result = _carVisitorDao.Update(tempCarVisitor);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(CarVisitor),
                                     Title = I18nManager.GetString("UpdateCarVisitorPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close CarVisitorActionWindow", TokenManage.CARVISITOR_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(Client));
                    UpdateValidationMessage(nameof(ClientId));
                    UpdateValidationMessage(nameof(OrderId));
                    UpdateValidationMessage(nameof(Phone));
                    UpdateValidationMessage(nameof(Remark));
                    UpdateValidationMessage(nameof(TrueName));
                    UpdateValidationMessage(nameof(VisitorHouse));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempCarVisitor = _carVisitorDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_carVisitorDao.Add(new CarVisitor{
                    CardNo = CardNo,
                    CarNo = CarNo,
                    Client = Client,
                    ClientId = ClientId,
                    CreateDate = tempDt,
                    EndDate = tempDt,
                    OrderId = OrderId,
                    Phone = Phone,
                    Remark = Remark,
                    StartDate = tempDt,
                    TrueName = TrueName,
                    VisitorHouse = VisitorHouse,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(CarVisitor),
                            Title = I18nManager.GetString("CreateCarVisitorPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close CARVISITORActionWindow", TokenManage.CARVISITOR_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.CARVISITOR_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

