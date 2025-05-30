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
    public partial class LicensePlateRecognitionActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private ParkDeviceDao _parkDeviceDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddParkDevice;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ParkDevice _selectedParkDevice;

        [Required(StringResourceKey.AllowCloseGateRequired)]
        [ObservableProperty]
        private System.Int32 _allowCloseGate;
        [Required(StringResourceKey.Camera0Required)]
        [ObservableProperty]
        private System.String _camera0;
        [Required(StringResourceKey.Camera0TypeRequired)]
        [ObservableProperty]
        private System.Int32 _camera0Type;
        [Required(StringResourceKey.Camera1Required)]
        [ObservableProperty]
        private System.String _camera1;
        [Required(StringResourceKey.Camera1TypeRequired)]
        [ObservableProperty]
        private System.Int32 _camera1Type;
        [Required(StringResourceKey.Camera485Required)]
        [ObservableProperty]
        private System.Int32 _camera485;
        [Required(StringResourceKey.CameraDoubleFilterRequired)]
        [ObservableProperty]
        private System.Int32 _cameraDoubleFilter;
        [Required(StringResourceKey.CameraIoRequired)]
        [ObservableProperty]
        private System.Int32 _cameraIo;
        [Required(StringResourceKey.CameraKeyRequired)]
        [ObservableProperty]
        private System.String _cameraKey;
        [Required(StringResourceKey.CameraRecomeFilterRequired)]
        [ObservableProperty]
        private System.Int32 _cameraRecomeFilter;
        [Required(StringResourceKey.CardCameraIpRequired)]
        [ObservableProperty]
        private System.String _cardCameraIp;
        [Required(StringResourceKey.CardCameraSnRequired)]
        [ObservableProperty]
        private System.String _cardCameraSn;
        [Required(StringResourceKey.CardCameraTypeRequired)]
        [ObservableProperty]
        private System.Int32 _cardCameraType;
        [Required(StringResourceKey.CardPortRequired)]
        [ObservableProperty]
        private System.Int32 _cardPort;
        [Required(StringResourceKey.CardIpRequired)]
        [ObservableProperty]
        private System.String _cardIp;
        [Required(StringResourceKey.CardSnRequired)]
        [ObservableProperty]
        private System.String _cardSn;
        [Required(StringResourceKey.CardTypeRequired)]
        [ObservableProperty]
        private System.Int32 _cardType;
        [Required(StringResourceKey.DevStatusRequired)]
        [ObservableProperty]
        private System.Int32 _devStatus;
        [Required(StringResourceKey.HasCardRequired)]
        [ObservableProperty]
        private System.Int32 _hasCard;
        [Required(StringResourceKey.HasCarmeraRequired)]
        [ObservableProperty]
        private System.Int32 _hasCarmera;
        [Required(StringResourceKey.LedDisplayRequired)]
        [ObservableProperty]
        private System.String _ledDisplay;
        [Required(StringResourceKey.LedIpRequired)]
        [ObservableProperty]
        private System.String _ledIp;
        [Required(StringResourceKey.LedTypeRequired)]
        [ObservableProperty]
        private System.Int32 _ledType;
        [Required(StringResourceKey.WayIdRequired)]
        [ObservableProperty]
        private System.Int32 _wayId;
        [Required(StringResourceKey.QrCodeRequired)]
        [ObservableProperty]
        private System.Int32 _qrCode;
        [Required(StringResourceKey.SetDisplayRequired)]
        [ObservableProperty]
        private System.String _setDisplay;
        [Required(StringResourceKey.SetVoiceRequired)]
        [ObservableProperty]
        private System.String _setVoice;
        [Required(StringResourceKey.TwoGateRequired)]
        [ObservableProperty]
        private System.Int32 _twoGate;

        public LicensePlateRecognitionActionWindowViewModel(AppDbContext appDbContext, ParkDeviceDao parkDeviceDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _parkDeviceDao = parkDeviceDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }


        [ObservableProperty] private string _allowCloseGateValidationMessage;
        [ObservableProperty] private string _camera0ValidationMessage;
        [ObservableProperty] private string _camera0TypeValidationMessage;
        [ObservableProperty] private string _camera1ValidationMessage;
        [ObservableProperty] private string _camera1TypeValidationMessage;
        [ObservableProperty] private string _camera485ValidationMessage;
        [ObservableProperty] private string _cameraDoubleFilterValidationMessage;
        [ObservableProperty] private string _cameraIoValidationMessage;
        [ObservableProperty] private string _cameraKeyValidationMessage;
        [ObservableProperty] private string _cameraRecomeFilterValidationMessage;
        [ObservableProperty] private string _cardCameraIpValidationMessage;
        [ObservableProperty] private string _cardCameraSnValidationMessage;
        [ObservableProperty] private string _cardCameraTypeValidationMessage;
        [ObservableProperty] private string _cardPortValidationMessage;
        [ObservableProperty] private string _cardIpValidationMessage;
        [ObservableProperty] private string _cardSnValidationMessage;
        [ObservableProperty] private string _cardTypeValidationMessage;
        [ObservableProperty] private string _devStatusValidationMessage;
        [ObservableProperty] private string _hasCardValidationMessage;
        [ObservableProperty] private string _hasCarmeraValidationMessage;
        [ObservableProperty] private string _ledDisplayValidationMessage;
        [ObservableProperty] private string _ledIpValidationMessage;
        [ObservableProperty] private string _ledTypeValidationMessage;
        [ObservableProperty] private string _wayIdValidationMessage;
        [ObservableProperty] private string _qrCodeValidationMessage;
        [ObservableProperty] private string _setDisplayValidationMessage;
        [ObservableProperty] private string _setVoiceValidationMessage;
        [ObservableProperty] private string _twoGateValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AllowCloseGate));
            UpdateValidationMessage(nameof(Camera0));
            UpdateValidationMessage(nameof(Camera0Type));
            UpdateValidationMessage(nameof(Camera1));
            UpdateValidationMessage(nameof(Camera1Type));
            UpdateValidationMessage(nameof(Camera485));
            UpdateValidationMessage(nameof(CameraDoubleFilter));
            UpdateValidationMessage(nameof(CameraIo));
            UpdateValidationMessage(nameof(CameraKey));
            UpdateValidationMessage(nameof(CameraRecomeFilter));
            UpdateValidationMessage(nameof(CardCameraIp));
            UpdateValidationMessage(nameof(CardCameraSn));
            UpdateValidationMessage(nameof(CardCameraType));
            UpdateValidationMessage(nameof(CardPort));
            UpdateValidationMessage(nameof(CardIp));
            UpdateValidationMessage(nameof(CardSn));
            UpdateValidationMessage(nameof(CardType));
            UpdateValidationMessage(nameof(DevStatus));
            UpdateValidationMessage(nameof(HasCard));
            UpdateValidationMessage(nameof(HasCarmera));
            UpdateValidationMessage(nameof(LedDisplay));
            UpdateValidationMessage(nameof(LedIp));
            UpdateValidationMessage(nameof(LedType));
            UpdateValidationMessage(nameof(WayId));
            UpdateValidationMessage(nameof(QrCode));
            UpdateValidationMessage(nameof(SetDisplay));
            UpdateValidationMessage(nameof(SetVoice));
            UpdateValidationMessage(nameof(TwoGate));
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
                    UpdateValidationMessage(nameof(AllowCloseGate));
                    UpdateValidationMessage(nameof(Camera0));
                    UpdateValidationMessage(nameof(Camera0Type));
                    UpdateValidationMessage(nameof(Camera1));
                    UpdateValidationMessage(nameof(Camera1Type));
                    UpdateValidationMessage(nameof(Camera485));
                    UpdateValidationMessage(nameof(CameraDoubleFilter));
                    UpdateValidationMessage(nameof(CameraIo));
                    UpdateValidationMessage(nameof(CameraKey));
                    UpdateValidationMessage(nameof(CameraRecomeFilter));
                    UpdateValidationMessage(nameof(CardCameraIp));
                    UpdateValidationMessage(nameof(CardCameraSn));
                    UpdateValidationMessage(nameof(CardCameraType));
                    UpdateValidationMessage(nameof(CardPort));
                    UpdateValidationMessage(nameof(CardIp));
                    UpdateValidationMessage(nameof(CardSn));
                    UpdateValidationMessage(nameof(CardType));
                    UpdateValidationMessage(nameof(DevStatus));
                    UpdateValidationMessage(nameof(HasCard));
                    UpdateValidationMessage(nameof(HasCarmera));
                    UpdateValidationMessage(nameof(LedDisplay));
                    UpdateValidationMessage(nameof(LedIp));
                    UpdateValidationMessage(nameof(LedType));
                    UpdateValidationMessage(nameof(WayId));
                    UpdateValidationMessage(nameof(QrCode));
                    UpdateValidationMessage(nameof(SetDisplay));
                    UpdateValidationMessage(nameof(SetVoice));
                    UpdateValidationMessage(nameof(TwoGate));
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
                .WithTitle(I18nManager.GetString("UpdateParkDevicePrompt"))
                .WithContent(I18nManager.GetString("ParkDeviceExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempParkDevice = _parkDeviceDao.GetById(SelectedParkDevice.Id);
                if (tempParkDevice != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkDevice.AllowCloseGate = AllowCloseGate;
                    tempParkDevice.Camera0 = Camera0;
                    tempParkDevice.Camera0Type = Camera0Type;
                    tempParkDevice.Camera1 = Camera1;
                    tempParkDevice.Camera1Type = Camera1Type;
                    tempParkDevice.Camera485 = Camera485;
                    tempParkDevice.CameraDoubleFilter = CameraDoubleFilter;
                    tempParkDevice.CameraIo = CameraIo;
                    tempParkDevice.CameraKey = CameraKey;
                    tempParkDevice.CameraRecomeFilter = CameraRecomeFilter;
                    tempParkDevice.CardCameraIp = CardCameraIp;
                    tempParkDevice.CardCameraSn = CardCameraSn;
                    tempParkDevice.CardCameraType = CardCameraType;
                    tempParkDevice.CardPort = CardPort;
                    tempParkDevice.CardIp = CardIp;
                    tempParkDevice.CardSn = CardSn;
                    tempParkDevice.CardType = CardType;
                    tempParkDevice.DevStatus = DevStatus;
                    tempParkDevice.HasCard = HasCard;
                    tempParkDevice.HasCarmera = HasCarmera;
                    tempParkDevice.LedDisplay = LedDisplay;
                    tempParkDevice.LedIp = LedIp;
                    tempParkDevice.LedType = LedType;
                    tempParkDevice.WayId = WayId;
                    tempParkDevice.QrCode = QrCode;
                    tempParkDevice.SetDisplay = SetDisplay;
                    tempParkDevice.SetVoice = SetVoice;
                    tempParkDevice.TwoGate = TwoGate;
                    int result = _parkDeviceDao.Update(tempParkDevice);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(ParkDevice),
                                     Title = I18nManager.GetString("UpdateParkDevicePrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close ParkDeviceActionWindow", TokenManage.PARKDEVICE_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(AllowCloseGate));
                    UpdateValidationMessage(nameof(Camera0));
                    UpdateValidationMessage(nameof(Camera0Type));
                    UpdateValidationMessage(nameof(Camera1));
                    UpdateValidationMessage(nameof(Camera1Type));
                    UpdateValidationMessage(nameof(Camera485));
                    UpdateValidationMessage(nameof(CameraDoubleFilter));
                    UpdateValidationMessage(nameof(CameraIo));
                    UpdateValidationMessage(nameof(CameraKey));
                    UpdateValidationMessage(nameof(CameraRecomeFilter));
                    UpdateValidationMessage(nameof(CardCameraIp));
                    UpdateValidationMessage(nameof(CardCameraSn));
                    UpdateValidationMessage(nameof(CardCameraType));
                    UpdateValidationMessage(nameof(CardPort));
                    UpdateValidationMessage(nameof(CardIp));
                    UpdateValidationMessage(nameof(CardSn));
                    UpdateValidationMessage(nameof(CardType));
                    UpdateValidationMessage(nameof(DevStatus));
                    UpdateValidationMessage(nameof(HasCard));
                    UpdateValidationMessage(nameof(HasCarmera));
                    UpdateValidationMessage(nameof(LedDisplay));
                    UpdateValidationMessage(nameof(LedIp));
                    UpdateValidationMessage(nameof(LedType));
                    UpdateValidationMessage(nameof(WayId));
                    UpdateValidationMessage(nameof(QrCode));
                    UpdateValidationMessage(nameof(SetDisplay));
                    UpdateValidationMessage(nameof(SetVoice));
                    UpdateValidationMessage(nameof(TwoGate));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

/*
//对数据的唯一性进行验证，这里需要测试来修正
                var tempParkDevice = _parkDeviceDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                return;
                }
*/
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result =_parkDeviceDao.Add(new ParkDevice{
                    AllowCloseGate = AllowCloseGate,
                    Camera0 = Camera0,
                    Camera0Type = Camera0Type,
                    Camera1 = Camera1,
                    Camera1Type = Camera1Type,
                    Camera485 = Camera485,
                    CameraDoubleFilter = CameraDoubleFilter,
                    CameraIo = CameraIo,
                    CameraKey = CameraKey,
                    CameraRecomeFilter = CameraRecomeFilter,
                    CardCameraIp = CardCameraIp,
                    CardCameraSn = CardCameraSn,
                    CardCameraType = CardCameraType,
                    CardPort = CardPort,
                    CardIp = CardIp,
                    CardSn = CardSn,
                    CardType = CardType,
                    DevStatus = DevStatus,
                    HasCard = HasCard,
                    HasCarmera = HasCarmera,
                    LedDisplay = LedDisplay,
                    LedIp = LedIp,
                    LedType = LedType,
                    WayId = WayId,
                    QrCode = QrCode,
                    SetDisplay = SetDisplay,
                    SetVoice = SetVoice,
                    TwoGate = TwoGate,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage { 
                                     CurrentModelType = typeof(ParkDevice),
                            Title = I18nManager.GetString("CreateParkDevicePrompt"),
                            Content = I18nManager.GetString("CreateSuccessfully"),
                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close PARKDEVICEActionWindow", TokenManage.PARKDEVICE_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKDEVICE_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

