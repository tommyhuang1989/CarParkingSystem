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
    public partial class ParkingAbnormalActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkingAbnormalDao _parkingAbnormalDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkingAbnormal;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkingAbnormal _selectedParkingAbnormal;

        [Required(StringResourceKey.CarColorRequired)]
        [ObservableProperty]
        private System.Int32 _carColor;
        [Required(StringResourceKey.CardNoRequired)]
        [ObservableProperty]
        private System.Int32 _cardNo;
        [Required(StringResourceKey.CarNoRequired)]
        [ObservableProperty]
        private System.String _carNo;
        [Required(StringResourceKey.CarTypeRequired)]
        [ObservableProperty]
        private System.Int32 _carType;
        [Required(StringResourceKey.InCpChangedRequired)]
        [ObservableProperty]
        private System.Int32 _inCpChanged;
        [Required(StringResourceKey.InImgRequired)]
        [ObservableProperty]
        private System.String _inImg;
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

        public ParkingAbnormalActionWindowViewModel(AppDbContext appDbContext, ParkingAbnormalDao parkingAbnormalDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkingAbnormalDao = parkingAbnormalDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _carColorValidationMessage;
        [ObservableProperty] private string _cardNoValidationMessage;
        [ObservableProperty] private string _carNoValidationMessage;
        [ObservableProperty] private string _carTypeValidationMessage;
        [ObservableProperty] private string _inCpChangedValidationMessage;
        [ObservableProperty] private string _inImgValidationMessage;
        [ObservableProperty] private string _inTimeValidationMessage;
        [ObservableProperty] private string _inTypeValidationMessage;
        [ObservableProperty] private string _inWayIdValidationMessage;
        [ObservableProperty] private string _orderIdValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CarColor));
            UpdateValidationMessage(nameof(CardNo));
            UpdateValidationMessage(nameof(CarNo));
            UpdateValidationMessage(nameof(CarType));
            UpdateValidationMessage(nameof(InCpChanged));
            UpdateValidationMessage(nameof(InImg));
            UpdateValidationMessage(nameof(InTime));
            UpdateValidationMessage(nameof(InType));
            UpdateValidationMessage(nameof(InWayId));
            UpdateValidationMessage(nameof(OrderId));
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
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(InCpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OrderId));
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
                .WithTitle(I18nManager.GetString("UpdateParkingAbnormalPrompt"))
                .WithContent(I18nManager.GetString("ParkingAbnormalExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkingAbnormal = _parkingAbnormalDao.GetById(SelectedParkingAbnormal.Id);
                if (tempParkingAbnormal != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkingAbnormal.CarColor = CarColor;
                    tempParkingAbnormal.CardNo = CardNo;
                    tempParkingAbnormal.CarNo = CarNo;
                    tempParkingAbnormal.CarType = CarType;
                    tempParkingAbnormal.InCpChanged = InCpChanged;
                    tempParkingAbnormal.InImg = InImg;
                    tempParkingAbnormal.InTime = tempDt;
                    tempParkingAbnormal.InType = InType;
                    tempParkingAbnormal.InWayId = InWayId;
                    tempParkingAbnormal.OrderId = OrderId;
                    tempParkingAbnormal.RecStatus = RecStatus;
                    tempParkingAbnormal.Remark = Remark;
                    tempParkingAbnormal.UpdateDt = UpdateDt;
                    tempParkingAbnormal.UpdateUser = UpdateUser;
                    int result = _parkingAbnormalDao.Update(tempParkingAbnormal);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkingAbnormal),
                                     Title = I18nManager.GetString("UpdateParkingAbnormalPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkingAbnormalActionWindow", TokenManage.PARKINGABNORMAL_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CarColor));
                    UpdateValidationMessage(nameof(CardNo));
                    UpdateValidationMessage(nameof(CarNo));
                    UpdateValidationMessage(nameof(CarType));
                    UpdateValidationMessage(nameof(InCpChanged));
                    UpdateValidationMessage(nameof(InImg));
                    UpdateValidationMessage(nameof(InType));
                    UpdateValidationMessage(nameof(InWayId));
                    UpdateValidationMessage(nameof(OrderId));
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
                var tempParkingAbnormal = _parkingAbnormalDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkingAbnormalDao.Add(new ParkingAbnormal{
                    CarColor = CarColor,
                    CardNo = CardNo,
                    CarNo = CarNo,
                    CarType = CarType,
                    InCpChanged = InCpChanged,
                    InImg = InImg,
                    InTime = tempDt,
                    InType = InType,
                    InWayId = InWayId,
                    OrderId = OrderId,
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
                                     CurrentModelType = typeof(ParkingAbnormal),
                            Title = I18nManager.GetString("CreateParkingAbnormalPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKINGABNORMALActionWindow", TokenManage.PARKINGABNORMAL_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKINGABNORMAL_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

