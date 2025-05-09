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
    public partial class DelayCardActionActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private DelayCardActionDao _delayCardActionDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddDelayCardAction;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private DelayCardAction _selectedDelayCardAction;

        [Required(StringResourceKey.CarIdRequired)]
        [ObservableProperty]
        private System.Int32 _carId;
        [ObservableProperty]
        private System.String _opDate;
        [Required(StringResourceKey.OpKindRequired)]
        [ObservableProperty]
        private System.Int32 _opKind;
        [Required(StringResourceKey.OpMoneyRequired)]
        [ObservableProperty]
        private System.Decimal _opMoney;
        [Required(StringResourceKey.OpNoRequired)]
        [ObservableProperty]
        private System.String _opNo;
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
        [ObservableProperty]
        private System.String _validEnd;
        [ObservableProperty]
        private System.String _validFrom;

        public DelayCardActionActionWindowViewModel(AppDbContext appDbContext, DelayCardActionDao delayCardActionDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _delayCardActionDao = delayCardActionDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _carIdValidationMessage;
        [ObservableProperty] private string _opDateValidationMessage;
        [ObservableProperty] private string _opKindValidationMessage;
        [ObservableProperty] private string _opMoneyValidationMessage;
        [ObservableProperty] private string _opNoValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _remarkValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _validEndValidationMessage;
        [ObservableProperty] private string _validFromValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CarId));
            UpdateValidationMessage(nameof(OpDate));
            UpdateValidationMessage(nameof(OpKind));
            UpdateValidationMessage(nameof(OpMoney));
            UpdateValidationMessage(nameof(OpNo));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(Remark));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
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
                    UpdateValidationMessage(nameof(CarId));
                    UpdateValidationMessage(nameof(OpKind));
                    UpdateValidationMessage(nameof(OpMoney));
                    UpdateValidationMessage(nameof(OpNo));
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
                .WithTitle(I18nManager.GetString("UpdateDelayCardActionPrompt"))
                .WithContent(I18nManager.GetString("DelayCardActionExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempDelayCardAction = _delayCardActionDao.GetById(SelectedDelayCardAction.Id);
                if (tempDelayCardAction != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempDelayCardAction.CarId = CarId;
                    tempDelayCardAction.OpDate = tempDt;
                    tempDelayCardAction.OpKind = OpKind;
                    tempDelayCardAction.OpMoney = OpMoney;
                    tempDelayCardAction.OpNo = OpNo;
                    tempDelayCardAction.RecStatus = RecStatus;
                    tempDelayCardAction.Remark = Remark;
                    tempDelayCardAction.UpdateDt = UpdateDt;
                    tempDelayCardAction.UpdateUser = UpdateUser;
                    tempDelayCardAction.ValidEnd = ValidEnd;
                    tempDelayCardAction.ValidFrom = ValidFrom;
                    int result = _delayCardActionDao.Update(tempDelayCardAction);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(DelayCardAction),
                                     Title = I18nManager.GetString("UpdateDelayCardActionPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close DelayCardActionActionWindow", TokenManage.DELAYCARDACTION_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CarId));
                    UpdateValidationMessage(nameof(OpKind));
                    UpdateValidationMessage(nameof(OpMoney));
                    UpdateValidationMessage(nameof(OpNo));
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
                var tempDelayCardAction = _delayCardActionDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_delayCardActionDao.Add(new DelayCardAction{
                    CarId = CarId,
                    OpDate = tempDt,
                    OpKind = OpKind,
                    OpMoney = OpMoney,
                    OpNo = OpNo,
                    RecStatus = RecStatus,
                    Remark = Remark,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                    ValidEnd = ValidEnd,
                    ValidFrom = ValidFrom,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(DelayCardAction),
                            Title = I18nManager.GetString("CreateDelayCardActionPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close DELAYCARDACTIONActionWindow", TokenManage.DELAYCARDACTION_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.DELAYCARDACTION_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

