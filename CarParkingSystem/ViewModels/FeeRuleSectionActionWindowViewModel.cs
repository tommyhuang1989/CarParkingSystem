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
    public partial class FeeRuleSectionActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private FeeRuleSectionDao _feeRuleSectionDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddFeeRuleSection;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private FeeRuleSection _selectedFeeRuleSection;

        [Required(StringResourceKey.FeeRuleFlagRequired)]
        [ObservableProperty]
        private System.Int32 _feeRuleFlag;
        [Required(StringResourceKey.FeeRuleIdRequired)]
        [ObservableProperty]
        private System.Int32 _feeRuleId;
        [Required(StringResourceKey.InWayRequired)]
        [ObservableProperty]
        private System.Int32 _inWay;
        [Required(StringResourceKey.OutWayRequired)]
        [ObservableProperty]
        private System.Int32 _outWay;
        [Required(StringResourceKey.OvertimeFeeRuleRequired)]
        [ObservableProperty]
        private System.Int32 _overtimeFeeRule;
        [Required(StringResourceKey.OvertimeTypeRequired)]
        [ObservableProperty]
        private System.Int32 _overtimeType;
        [Required(StringResourceKey.ParkingFeeRuleRequired)]
        [ObservableProperty]
        private System.Int32 _parkingFeeRule;
        [Required(StringResourceKey.ParkingTimeRequired)]
        [ObservableProperty]
        private System.Int32 _parkingTime;
        [Required(StringResourceKey.SectionNameRequired)]
        [ObservableProperty]
        private System.String _sectionName;

        public FeeRuleSectionActionWindowViewModel(AppDbContext appDbContext, FeeRuleSectionDao feeRuleSectionDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _feeRuleSectionDao = feeRuleSectionDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _feeRuleFlagValidationMessage;
        [ObservableProperty] private string _feeRuleIdValidationMessage;
        [ObservableProperty] private string _inWayValidationMessage;
        [ObservableProperty] private string _outWayValidationMessage;
        [ObservableProperty] private string _overtimeFeeRuleValidationMessage;
        [ObservableProperty] private string _overtimeTypeValidationMessage;
        [ObservableProperty] private string _parkingFeeRuleValidationMessage;
        [ObservableProperty] private string _parkingTimeValidationMessage;
        [ObservableProperty] private string _sectionNameValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(FeeRuleFlag));
            UpdateValidationMessage(nameof(FeeRuleId));
            UpdateValidationMessage(nameof(InWay));
            UpdateValidationMessage(nameof(OutWay));
            UpdateValidationMessage(nameof(OvertimeFeeRule));
            UpdateValidationMessage(nameof(OvertimeType));
            UpdateValidationMessage(nameof(ParkingFeeRule));
            UpdateValidationMessage(nameof(ParkingTime));
            UpdateValidationMessage(nameof(SectionName));
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
                    UpdateValidationMessage(nameof(FeeRuleFlag));
                    UpdateValidationMessage(nameof(FeeRuleId));
                    UpdateValidationMessage(nameof(InWay));
                    UpdateValidationMessage(nameof(OutWay));
                    UpdateValidationMessage(nameof(OvertimeFeeRule));
                    UpdateValidationMessage(nameof(OvertimeType));
                    UpdateValidationMessage(nameof(ParkingFeeRule));
                    UpdateValidationMessage(nameof(ParkingTime));
                    UpdateValidationMessage(nameof(SectionName));
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
                .WithTitle(I18nManager.GetString("UpdateFeeRuleSectionPrompt"))
                .WithContent(I18nManager.GetString("FeeRuleSectionExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempFeeRuleSection = _feeRuleSectionDao.GetById(SelectedFeeRuleSection.Id);
                if (tempFeeRuleSection != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempFeeRuleSection.FeeRuleFlag = FeeRuleFlag;
                    tempFeeRuleSection.FeeRuleId = FeeRuleId;
                    tempFeeRuleSection.InWay = InWay;
                    tempFeeRuleSection.OutWay = OutWay;
                    tempFeeRuleSection.OvertimeFeeRule = OvertimeFeeRule;
                    tempFeeRuleSection.OvertimeType = OvertimeType;
                    tempFeeRuleSection.ParkingFeeRule = ParkingFeeRule;
                    tempFeeRuleSection.ParkingTime = ParkingTime;
                    tempFeeRuleSection.SectionName = SectionName;
                    int result = _feeRuleSectionDao.Update(tempFeeRuleSection);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(FeeRuleSection),
                                     Title = I18nManager.GetString("UpdateFeeRuleSectionPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close FeeRuleSectionActionWindow", TokenManage.FEERULESECTION_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(FeeRuleFlag));
                    UpdateValidationMessage(nameof(FeeRuleId));
                    UpdateValidationMessage(nameof(InWay));
                    UpdateValidationMessage(nameof(OutWay));
                    UpdateValidationMessage(nameof(OvertimeFeeRule));
                    UpdateValidationMessage(nameof(OvertimeType));
                    UpdateValidationMessage(nameof(ParkingFeeRule));
                    UpdateValidationMessage(nameof(ParkingTime));
                    UpdateValidationMessage(nameof(SectionName));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempFeeRuleSection = _feeRuleSectionDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_feeRuleSectionDao.Add(new FeeRuleSection{
                    FeeRuleFlag = FeeRuleFlag,
                    FeeRuleId = FeeRuleId,
                    InWay = InWay,
                    OutWay = OutWay,
                    OvertimeFeeRule = OvertimeFeeRule,
                    OvertimeType = OvertimeType,
                    ParkingFeeRule = ParkingFeeRule,
                    ParkingTime = ParkingTime,
                    SectionName = SectionName,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(FeeRuleSection),
                            Title = I18nManager.GetString("CreateFeeRuleSectionPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close FEERULESECTIONActionWindow", TokenManage.FEERULESECTION_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.FEERULESECTION_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

