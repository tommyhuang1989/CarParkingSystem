using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Services;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.ColorTheme;
using SukiUI.Content;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
namespace CarParkingSystem.ViewModels
{
    public partial class CardSwipeDeviceViewModel : ParkDeviceTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkDevice> _parkDevices;
        [ObservableProperty] private ParkDevice _selectedParkDevice;
        [ObservableProperty] private string _updateInfo;
        private ParkDeviceDao _parkDeviceDao;
        private AppDbContext _appDbContext;
        public ISukiToastManager _toastManager { get; }
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkDevice> _predicate { get; set; }
        public Expression<Func<ParkDevice, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool? _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAllowCloseGate;
        [ObservableProperty] private System.String _searchCamera0;
        [ObservableProperty] private System.Int32 _searchCamera0Type;
        [ObservableProperty] private System.String _searchCamera1;
        [ObservableProperty] private System.Int32 _searchCamera1Type;
        [ObservableProperty] private System.Int32 _searchCamera485;
        [ObservableProperty] private System.Int32 _searchCameraDoubleFilter;
        [ObservableProperty] private System.Int32 _searchCameraIo;
        [ObservableProperty] private System.String _searchCameraKey;
        [ObservableProperty] private System.Int32 _searchCameraRecomeFilter;
        [ObservableProperty] private System.String _searchCardCameraIp;
        [ObservableProperty] private System.String _searchCardCameraSn;
        [ObservableProperty] private System.Int32 _searchCardCameraType;
        [ObservableProperty] private System.Int32 _searchCardPort;
        [ObservableProperty] private System.String _searchCardIp;
        [ObservableProperty] private System.String _searchCardSn;
        [ObservableProperty] private System.Int32 _searchCardType;
        [ObservableProperty] private System.Int32 _searchDevStatus;
        [ObservableProperty] private System.Int32 _searchHasCard;
        [ObservableProperty] private System.Int32 _searchHasCarmera;
        [ObservableProperty] private System.String _searchLedDisplay;
        [ObservableProperty] private System.String _searchLedIp;
        [ObservableProperty] private System.Int32 _searchLedType;
        [ObservableProperty] private System.Int32 _searchWayId;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchQrCode;
        [ObservableProperty] private System.String _searchSetDisplay;
        [ObservableProperty] private System.String _searchSetVoice;
        [ObservableProperty] private System.Int32 _searchTwoGate;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;


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

        public CardSwipeDeviceViewModel(ParkDeviceDao parkDeviceDao, AppDbContext appDbContext, ISukiToastManager toastManager, ISukiDialogManager dialogManager) : base(Language.CardSwipeDevice)
        {
            _parkDeviceDao = parkDeviceDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            _toastManager = toastManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
            WeakReferenceMessenger.Default.Register<SelectedParkDeviceMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN, ReciveRefreshSummarySelectedParkDevice);
        }

        /// <summary>
        /// 1.在列表内选择时会触发；
        /// 2.新增；
        /// 3.删除；
        /// 4.改变 PageSize；
        /// 5.条件搜索时；
        /// 6.重置搜索时；
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void ReciveRefreshSummarySelectedParkDevice(object recipient, SelectedParkDeviceMessage message)
        {
            var selectedCount = ParkDevices?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ParkDevices.Count())
            {
                IsSelectedAll = true;
            }
            else
            {
                IsSelectedAll = null;
            }
        }

        private void Recive(object recipient, ToastMessage message)
        {
            //20250402, 不是相同类型发送的消息不处理
            if (message.CurrentModelType != typeof(ParkDevice)) return;

            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<ParkDevice> predicate = null, Expression<Func<ParkDevice, object>> expression = null, bool isDesc = false)
        {
            ParkDevices = new ObservableCollection<ParkDevice>(_parkDeviceDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
            AllCount = _parkDeviceDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ParkDevice> predicate = null, Expression<Func<ParkDevice, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
                var result = await _parkDeviceDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
                ParkDevices = new ObservableCollection<ParkDevice>(result);
                AllCount = _parkDeviceDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ParkDevices.ToList(), "ParkDevices.xlsx", I18nManager.GetString("ParkDeviceInfo"));
                ExcelService.HighlightFileInExplorer(filePath);
            });
        }

        [RelayCommand]
        private void SelectedAll()
        {
            if (IsSelectedAll == null)
            {
                //实现可以显示三种状态，但是点击只有两种情况的功能
                IsSelectedAll = false;
                //return;
            }

            if (IsSelectedAll == false)
            {
                foreach (var item in ParkDevices)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ParkDevices)
                {
                    item.IsSelected = true;
                }
            }

        }

        [RelayCommand]
        private Task Sorting(object obj)
        {
            List<object> list = (List<object>)obj;
            var fieldName = list[0] as string;
            var isDesc = (bool)list[1];

            return Task.Run(async () =>
            {
                _expression = u => u.Id;
                _isOrderByDesc = isDesc;

                switch (fieldName)
                {
                    case "AllowCloseGate":
                        _expression = u => u.AllowCloseGate; break;
                    case "Camera0":
                        _expression = u => u.Camera0; break;
                    case "Camera0Type":
                        _expression = u => u.Camera0Type; break;
                    case "Camera1":
                        _expression = u => u.Camera1; break;
                    case "Camera1Type":
                        _expression = u => u.Camera1Type; break;
                    case "Camera485":
                        _expression = u => u.Camera485; break;
                    case "CameraDoubleFilter":
                        _expression = u => u.CameraDoubleFilter; break;
                    case "CameraIo":
                        _expression = u => u.CameraIo; break;
                    case "CameraKey":
                        _expression = u => u.CameraKey; break;
                    case "CameraRecomeFilter":
                        _expression = u => u.CameraRecomeFilter; break;
                    case "CardCameraIp":
                        _expression = u => u.CardCameraIp; break;
                    case "CardCameraSn":
                        _expression = u => u.CardCameraSn; break;
                    case "CardCameraType":
                        _expression = u => u.CardCameraType; break;
                    case "CardPort":
                        _expression = u => u.CardPort; break;
                    case "CardIp":
                        _expression = u => u.CardIp; break;
                    case "CardSn":
                        _expression = u => u.CardSn; break;
                    case "CardType":
                        _expression = u => u.CardType; break;
                    case "DevStatus":
                        _expression = u => u.DevStatus; break;
                    case "HasCard":
                        _expression = u => u.HasCard; break;
                    case "HasCarmera":
                        _expression = u => u.HasCarmera; break;
                    case "LedDisplay":
                        _expression = u => u.LedDisplay; break;
                    case "LedIp":
                        _expression = u => u.LedIp; break;
                    case "LedType":
                        _expression = u => u.LedType; break;
                    case "WayId":
                        _expression = u => u.WayId; break;
                    case "Id":
                        _expression = u => u.Id; break;
                    case "QrCode":
                        _expression = u => u.QrCode; break;
                    case "SetDisplay":
                        _expression = u => u.SetDisplay; break;
                    case "SetVoice":
                        _expression = u => u.SetVoice; break;
                    case "TwoGate":
                        _expression = u => u.TwoGate; break;
                        break;
                }

                // 搜索后的结果进行排序时，也需要考虑过滤的条件
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);

                //IsBusy = true;
                //await Task.Delay(3000);
                //IsBusy = false;
            });
        }

        [RelayCommand]
        private Task SearchParkDevice()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ParkDevice>(true);
                if (!String.IsNullOrEmpty(SearchCamera0))
                {
                    _predicate = _predicate.And(p => p.Camera0.Contains(SearchCamera0));
                }
                if (!String.IsNullOrEmpty(SearchCamera1))
                {
                    _predicate = _predicate.And(p => p.Camera1.Contains(SearchCamera1));
                }
                if (!String.IsNullOrEmpty(SearchCameraKey))
                {
                    _predicate = _predicate.And(p => p.CameraKey.Contains(SearchCameraKey));
                }
                if (!String.IsNullOrEmpty(SearchCardCameraIp))
                {
                    _predicate = _predicate.And(p => p.CardCameraIp.Contains(SearchCardCameraIp));
                }
                if (!String.IsNullOrEmpty(SearchCardCameraSn))
                {
                    _predicate = _predicate.And(p => p.CardCameraSn.Contains(SearchCardCameraSn));
                }
                if (!String.IsNullOrEmpty(SearchCardIp))
                {
                    _predicate = _predicate.And(p => p.CardIp.Contains(SearchCardIp));
                }
                if (!String.IsNullOrEmpty(SearchCardSn))
                {
                    _predicate = _predicate.And(p => p.CardSn.Contains(SearchCardSn));
                }
                if (!String.IsNullOrEmpty(SearchLedDisplay))
                {
                    _predicate = _predicate.And(p => p.LedDisplay.Contains(SearchLedDisplay));
                }
                if (!String.IsNullOrEmpty(SearchLedIp))
                {
                    _predicate = _predicate.And(p => p.LedIp.Contains(SearchLedIp));
                }
                if (!String.IsNullOrEmpty(SearchSetDisplay))
                {
                    _predicate = _predicate.And(p => p.SetDisplay.Contains(SearchSetDisplay));
                }
                if (!String.IsNullOrEmpty(SearchSetVoice))
                {
                    _predicate = _predicate.And(p => p.SetVoice.Contains(SearchSetVoice));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCamera0 = string.Empty;
            SearchCamera1 = string.Empty;
            SearchCameraKey = string.Empty;
            SearchCardCameraIp = string.Empty;
            SearchCardCameraSn = string.Empty;
            SearchCardIp = string.Empty;
            SearchCardSn = string.Empty;
            SearchLedDisplay = string.Empty;
            SearchLedIp = string.Empty;
            SearchSetDisplay = string.Empty;
            SearchSetVoice = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private Task SelectionChanged(int pageSize)
        {
            return Task.Run(async () =>
            {
                PageSize = pageSize;

                CurrentPageIndex = 1;//更改页面数量应该从第一页显示

                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);

                WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
            });
        }

        [RelayCommand]
        private void AddOld()
        {
            var actionVM = App.ServiceProvider.GetService<ParkDeviceActionWindowViewModel>();
            actionVM.Camera0 = string.Empty;
            actionVM.Camera1 = string.Empty;
            actionVM.CameraKey = string.Empty;
            actionVM.CardCameraIp = string.Empty;
            actionVM.CardCameraSn = string.Empty;
            actionVM.CardIp = string.Empty;
            actionVM.CardSn = string.Empty;
            actionVM.LedDisplay = string.Empty;
            actionVM.LedIp = string.Empty;
            actionVM.SetDisplay = string.Empty;
            actionVM.SetVoice = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkDevice = true;
            actionVM.Title = I18nManager.GetString("CreateNewParkDevice");
            var actionWindow = App.Views.CreateView<ParkDeviceActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);

            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateParkDevice(ParkDevice parkDevice)
        {
            var actionVM = App.ServiceProvider.GetService<ParkDeviceActionWindowViewModel>();
            actionVM.SelectedParkDevice = parkDevice;
            actionVM.Camera0 = parkDevice.Camera0;
            actionVM.Camera1 = parkDevice.Camera1;
            actionVM.CameraKey = parkDevice.CameraKey;
            actionVM.CardCameraIp = parkDevice.CardCameraIp;
            actionVM.CardCameraSn = parkDevice.CardCameraSn;
            actionVM.CardIp = parkDevice.CardIp;
            actionVM.CardSn = parkDevice.CardSn;
            actionVM.LedDisplay = parkDevice.LedDisplay;
            actionVM.LedIp = parkDevice.LedIp;
            actionVM.SetDisplay = parkDevice.SetDisplay;
            actionVM.SetVoice = parkDevice.SetVoice;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkDevice = false;
            actionVM.Title = I18nManager.GetString("UpdateParkDeviceInfo");
            var actionWindow = App.Views.CreateView<ParkDeviceActionWindowViewModel>(App.ServiceProvider) as Window;
            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }


        [RelayCommand]
        private void DeleteParkDevice()
        {
            var selectedCount = ParkDevices.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkDevicePrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteParkDevice"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkDevice"))
                .WithContent(I18nManager.GetString("SureWantToDeleteParkDevice"))
                .WithActionButton(I18nManager.GetString("Sure"), dialog => SureDeleteAsync(dialog), false)
                .WithActionButton(I18nManager.GetString("Cancel"), _ => CancelDelete(), true)
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .TryShow();
            }
        }

        private void CancelDelete()
        {
            return;
        }

        private async Task SureDeleteAsync(ISukiDialog dialog)
        {
            var selectedIds = ParkDevices.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _parkDeviceDao.DeleteRangeAsync<ParkDevice>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteParkDevicePrompt");
                    dialog.Content = I18nManager.GetString("DeleteSuccessfully");
                    if (dialog.ActionButtons.Count > 1)
                    {
                        (dialog.ActionButtons[0] as Button).IsVisible = false;//隐藏第一个按钮

                        var button = dialog.ActionButtons[1] as Button;//将取消按钮赋值为“确定”
                        button.Content = I18nManager.GetString("Submit");
                    }

                    WeakReferenceMessenger.Default.Send(
                                new ToastMessage
                                {
                                    CurrentModelType = typeof(ParkDevice),
                                    Title = I18nManager.GetString("DeleteParkDevicePrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
                });
            }
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
                        _toastManager.CreateToast()
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
                var result = _parkDeviceDao.Add(new ParkDevice
                {
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
                            new ToastMessage
                            {
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

        #region 分页

        [RelayCommand]
        private Task FirstPage()
        {
            return Task.Run(async () => {
                CurrentPageIndex = 1;
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
            });
        }

        [RelayCommand]
        private Task PrevPage()
        {
            return Task.Run(async () => {
                if (CurrentPageIndex > 1)
                {
                    CurrentPageIndex = CurrentPageIndex - 1;
                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                }
            });
        }

        [RelayCommand]
        private Task NextPage()
        {
            return Task.Run(async () => {
                if (CurrentPageIndex < PageCount)
                {
                    CurrentPageIndex = CurrentPageIndex + 1;
                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                }
            });
        }

        [RelayCommand]
        private Task LastPage()
        {
            return Task.Run(async () => {
                CurrentPageIndex = PageCount;
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
            });
        }

        [RelayCommand]
        private Task GoToPage(object obj)
        {
            return Task.Run(async () => {
                int pageIndexToGo = 0;
                try
                {
                    pageIndexToGo = System.Convert.ToInt32(obj);
                }
                catch (Exception ex)
                {
                }

                // 如果跳转到的页码就是当前页，就不跳转
                if (pageIndexToGo > 0
                    && pageIndexToGo <= PageCount
                    && pageIndexToGo != CurrentPageIndex)
                {
                    CurrentPageIndex = pageIndexToGo;
                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                }
            });
        }
        #endregion

    }
}