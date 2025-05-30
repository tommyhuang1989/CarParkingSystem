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
    public partial class ParkWayCardActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkWayCardDao _parkWayCardDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkWayCard;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkWayCard _selectedParkWayCard;

        [Required(StringResourceKey.CardIdRequired)]
        [ObservableProperty]
        private System.Int32 _cardId;
        [Required(StringResourceKey.IsConfirmRequired)]
        [ObservableProperty]
        private System.Int32 _isConfirm;
        [Required(StringResourceKey.RecStatusRequired)]
        [ObservableProperty]
        private System.Int32 _recStatus;
        [ObservableProperty]
        private System.String _updateDt;
        [Required(StringResourceKey.UpdateUserRequired)]
        [ObservableProperty]
        private System.Int32 _updateUser;
        [Required(StringResourceKey.WayIdRequired)]
        [ObservableProperty]
        private System.Int32 _wayId;

        public ParkWayCardActionWindowViewModel(AppDbContext appDbContext, ParkWayCardDao parkWayCardDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkWayCardDao = parkWayCardDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _cardIdValidationMessage;
        [ObservableProperty] private string _isConfirmValidationMessage;
        [ObservableProperty] private string _recStatusValidationMessage;
        [ObservableProperty] private string _updateDtValidationMessage;
        [ObservableProperty] private string _updateUserValidationMessage;
        [ObservableProperty] private string _wayIdValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(CardId));
            UpdateValidationMessage(nameof(IsConfirm));
            UpdateValidationMessage(nameof(RecStatus));
            UpdateValidationMessage(nameof(UpdateDt));
            UpdateValidationMessage(nameof(UpdateUser));
            UpdateValidationMessage(nameof(WayId));
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
                    UpdateValidationMessage(nameof(CardId));
                    UpdateValidationMessage(nameof(IsConfirm));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(WayId));
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
                .WithTitle(I18nManager.GetString("UpdateParkWayCardPrompt"))
                .WithContent(I18nManager.GetString("ParkWayCardExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkWayCard = _parkWayCardDao.GetById(SelectedParkWayCard.Id);
                if (tempParkWayCard != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkWayCard.CardId = CardId;
                    tempParkWayCard.IsConfirm = IsConfirm;
                    tempParkWayCard.RecStatus = RecStatus;
                    tempParkWayCard.UpdateDt = UpdateDt;
                    tempParkWayCard.UpdateUser = UpdateUser;
                    tempParkWayCard.WayId = WayId;
                    int result = _parkWayCardDao.Update(tempParkWayCard);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkWayCard),
                                     Title = I18nManager.GetString("UpdateParkWayCardPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkWayCardActionWindow", TokenManage.PARKWAYCARD_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(CardId));
                    UpdateValidationMessage(nameof(IsConfirm));
                    UpdateValidationMessage(nameof(RecStatus));
                    UpdateValidationMessage(nameof(UpdateUser));
                    UpdateValidationMessage(nameof(WayId));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkWayCard = _parkWayCardDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkWayCardDao.Add(new ParkWayCard{
                    CardId = CardId,
                    IsConfirm = IsConfirm,
                    RecStatus = RecStatus,
                    UpdateDt = UpdateDt,
                    UpdateUser = UpdateUser,
                    WayId = WayId,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkWayCard),
                            Title = I18nManager.GetString("CreateParkWayCardPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKWAYCARDActionWindow", TokenManage.PARKWAYCARD_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKWAYCARD_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

