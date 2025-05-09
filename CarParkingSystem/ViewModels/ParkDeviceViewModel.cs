using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Services;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LinqKit;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SukiUI.ColorTheme;
using SukiUI.Content;
using CarParkingSystem.I18n;
namespace CarParkingSystem.ViewModels
{
  public partial class ParkDeviceViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkDevice> _parkDevices;
        [ObservableProperty] private ParkDevice _selectedParkDevice;
        [ObservableProperty] private string _updateInfo;
        private ParkDeviceDao _parkDeviceDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkDevice> _predicate { get; set; }
        public Expression<Func<ParkDevice, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
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
      public ParkDeviceViewModel(ParkDeviceDao parkDeviceDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.DeviceManagement, MaterialIconKind.Abacus, pid: 1, id: 11, index: 11)
        {
            _parkDeviceDao = parkDeviceDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
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
            else {
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

            if (IsSelectedAll == false) {
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
            return Task.Run(async ()=>
            {
                PageSize = pageSize;

                CurrentPageIndex = 1;//更改页面数量应该从第一页显示

                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);

                WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
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

