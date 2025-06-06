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
    public partial class ParkAreaActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkAreaDao _parkAreaDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkArea;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkArea _selectedParkArea;

        [Required(StringResourceKey.AreaNameRequired)]
        [ObservableProperty]
        private System.String _areaName;
        [Required(StringResourceKey.ShowAreaLotRequired)]
        [ObservableProperty]
        private System.Int32 _showAreaLot;
        [Required(StringResourceKey.TempCarFullCanInRequired)]
        [ObservableProperty]
        private System.Int32 _tempCarFullCanIn;
        [Required(StringResourceKey.TotalCarsRequired)]
        [ObservableProperty]
        private System.Int32 _totalCars;
        [Required(StringResourceKey.UpateUserRequired)]
        [ObservableProperty]
        private System.Int32 _upateUser;
        [ObservableProperty]
        private System.String _updateDate;
        [Required(StringResourceKey.UsedCarsRequired)]
        [ObservableProperty]
        private System.Int32 _usedCars;

        public ParkAreaActionWindowViewModel(AppDbContext appDbContext, ParkAreaDao parkAreaDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkAreaDao = parkAreaDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _areaNameValidationMessage;
        [ObservableProperty] private string _showAreaLotValidationMessage;
        [ObservableProperty] private string _tempCarFullCanInValidationMessage;
        [ObservableProperty] private string _totalCarsValidationMessage;
        [ObservableProperty] private string _upateUserValidationMessage;
        [ObservableProperty] private string _updateDateValidationMessage;
        [ObservableProperty] private string _usedCarsValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AreaName));
            UpdateValidationMessage(nameof(ShowAreaLot));
            UpdateValidationMessage(nameof(TempCarFullCanIn));
            UpdateValidationMessage(nameof(TotalCars));
            UpdateValidationMessage(nameof(UpateUser));
            UpdateValidationMessage(nameof(UpdateDate));
            UpdateValidationMessage(nameof(UsedCars));
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
                    UpdateValidationMessage(nameof(AreaName));
                    UpdateValidationMessage(nameof(ShowAreaLot));
                    UpdateValidationMessage(nameof(TempCarFullCanIn));
                    UpdateValidationMessage(nameof(TotalCars));
                    UpdateValidationMessage(nameof(UpateUser));
                    UpdateValidationMessage(nameof(UsedCars));
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
                .WithTitle(I18nManager.GetString("UpdateParkAreaPrompt"))
                .WithContent(I18nManager.GetString("ParkAreaExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkArea = _parkAreaDao.GetById(SelectedParkArea.Id);
                if (tempParkArea != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkArea.AreaName = AreaName;
                    tempParkArea.ShowAreaLot = ShowAreaLot;
                    tempParkArea.TempCarFullCanIn = TempCarFullCanIn;
                    tempParkArea.TotalCars = TotalCars;
                    tempParkArea.UpateUser = UpateUser;
                    tempParkArea.UpdateDate = tempDt;
                    tempParkArea.UsedCars = UsedCars;
                    int result = _parkAreaDao.Update(tempParkArea);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkArea),
                                     Title = I18nManager.GetString("UpdateParkAreaPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkAreaActionWindow", TokenManage.PARKAREA_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(AreaName));
                    UpdateValidationMessage(nameof(ShowAreaLot));
                    UpdateValidationMessage(nameof(TempCarFullCanIn));
                    UpdateValidationMessage(nameof(TotalCars));
                    UpdateValidationMessage(nameof(UpateUser));
                    UpdateValidationMessage(nameof(UsedCars));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkArea = _parkAreaDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkAreaDao.Add(new ParkArea{
                    AreaName = AreaName,
                    ShowAreaLot = ShowAreaLot,
                    TempCarFullCanIn = TempCarFullCanIn,
                    TotalCars = TotalCars,
                    UpateUser = UpateUser,
                    UpdateDate = tempDt,
                    UsedCars = UsedCars,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkArea),
                            Title = I18nManager.GetString("CreateParkAreaPrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKAREAActionWindow", TokenManage.PARKAREA_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKAREA_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

