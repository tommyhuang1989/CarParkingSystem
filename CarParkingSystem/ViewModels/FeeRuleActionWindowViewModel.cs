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
    public partial class FeeRuleActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private FeeRuleDao _feeRuleDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddFeeRule;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private FeeRule _selectedFeeRule;

        [Required(StringResourceKey.FeeRuleDataRequired)]
        [ObservableProperty]
        private System.String _feeRuleData;
        [Required(StringResourceKey.FeeRuleNameRequired)]
        [ObservableProperty]
        private System.String _feeRuleName;
        [Required(StringResourceKey.FeeRuleTypeRequired)]
        [ObservableProperty]
        private System.Int32 _feeRuleType;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;

        public FeeRuleActionWindowViewModel(AppDbContext appDbContext, FeeRuleDao feeRuleDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _feeRuleDao = feeRuleDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _feeRuleDataValidationMessage;
        [ObservableProperty] private string _feeRuleNameValidationMessage;
        [ObservableProperty] private string _feeRuleTypeValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(FeeRuleData));
            UpdateValidationMessage(nameof(FeeRuleName));
            UpdateValidationMessage(nameof(FeeRuleType));
            UpdateValidationMessage(nameof(RecStatus));
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
                    UpdateValidationMessage(nameof(FeeRuleData));
                    UpdateValidationMessage(nameof(FeeRuleName));
                    UpdateValidationMessage(nameof(FeeRuleType));
                    UpdateValidationMessage(nameof(RecStatus));
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
                .WithTitle(I18nManager.GetString("UpdateFeeRulePrompt"))
                .WithContent(I18nManager.GetString("FeeRuleExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempFeeRule = _feeRuleDao.GetById(SelectedFeeRule.Id);
                if (tempFeeRule != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempFeeRule.FeeRuleData = FeeRuleData;
                    tempFeeRule.FeeRuleName = FeeRuleName;
                    tempFeeRule.FeeRuleType = FeeRuleType;
                    tempFeeRule.RecStatus = RecStatus;
                    tempFeeRule.UpdateDt = tempDt;
                    tempFeeRule.UpdateUser = UpdateUser;
                    int result = _feeRuleDao.Update(tempFeeRule);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(FeeRule),
                                     Title = I18nManager.GetString("UpdateFeeRulePrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close FeeRuleActionWindow", TokenManage.FEERULE_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(FeeRuleData));
                    UpdateValidationMessage(nameof(FeeRuleName));
                    UpdateValidationMessage(nameof(FeeRuleType));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(UpdateUser));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempFeeRule = _feeRuleDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_feeRuleDao.Add(new FeeRule{
                    FeeRuleData = FeeRuleData,
                    FeeRuleName = FeeRuleName,
                    FeeRuleType = FeeRuleType,
                    RecStatus = RecStatus,
                    UpdateDt = tempDt,
                    UpdateUser = UpdateUser,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(FeeRule),
                            Title = I18nManager.GetString("CreateFeeRulePrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close FEERULEActionWindow", TokenManage.FEERULE_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.FEERULE_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

