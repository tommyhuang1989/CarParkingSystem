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
  public partial class ParkWayViewModel: ParkWayTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkWay> _parkWays;
        [ObservableProperty] private ParkWay _selectedParkWay;
        [ObservableProperty] private string _updateInfo;
        private ParkWayDao _parkWayDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkWay> _predicate { get; set; }
        public Expression<Func<ParkWay, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Decimal _searchAmount;
        [ObservableProperty] private System.Int32 _searchAreaId;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchCarInId;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.Int32 _searchCarNoColor;
        [ObservableProperty] private System.Int32 _searchCarNoType;
        [ObservableProperty] private System.Int32 _searchCarStatus;
        [ObservableProperty] private System.Int32 _searchCarTypeId;
        [ObservableProperty] private System.Int32 _searchChangedCarNo;
        [ObservableProperty] private System.Decimal _searchDiscount;
        [ObservableProperty] private System.String _searchDisplay;
        [ObservableProperty] private System.String _searchInImage;
        [ObservableProperty] private System.String _searchInTime;
        [ObservableProperty] private System.Int32 _searchIsAllowEnter;
        [ObservableProperty] private System.Int32 _searchIsCsConfirm;
        [ObservableProperty] private System.Int32 _searchIsNeedAysn;
        [ObservableProperty] private System.Int32 _searchIsPaid;
        [ObservableProperty] private System.String _searchLastCarNo;
        [ObservableProperty] private System.String _searchLastCarTime;
        [ObservableProperty] private System.String _searchOrderId;
        [ObservableProperty] private System.Decimal _searchPaid;
        [ObservableProperty] private System.String _searchPlateId;
        [ObservableProperty] private System.Int32 _searchRecStatus;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.Int32 _searchSpecialCar;
        [ObservableProperty] private System.Int32 _searchTriggerFlag;
        [ObservableProperty] private System.String _searchUpdateDt;
        [ObservableProperty] private System.Int32 _searchUpdateUser;
        [ObservableProperty] private System.Int32 _searchVideoCall;
        [ObservableProperty] private System.Int32 _searchVideoCallQrcode;
        [ObservableProperty] private System.String _searchVideoCallTime;
        [ObservableProperty] private System.String _searchVoice;
        [ObservableProperty] private System.Decimal _searchWaitPay;
        [ObservableProperty] private System.String _searchWaittingCarNo;
        [ObservableProperty] private System.Int32 _searchWaittingCarNoColor;
        [ObservableProperty] private System.Int32 _searchWaittingCarNoType;
        [ObservableProperty] private System.String _searchWaittingImg;
        [ObservableProperty] private System.String _searchWaittingPlateId;
        [ObservableProperty] private System.String _searchWaittingTime;
        [ObservableProperty] private System.Int32 _searchWayCarType;
        [ObservableProperty] private System.Int32 _searchWayConnect;
        [ObservableProperty] private System.String _searchWayName;
        [ObservableProperty] private System.String _searchWayNo;
        [ObservableProperty] private System.Int32 _searchWayStatus;
        [ObservableProperty] private System.Int32 _searchWayType;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public ParkWayViewModel(ParkWayDao parkWayDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.ParkWayManagement)
        {
            _parkWayDao = parkWayDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedParkWayMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN, ReciveRefreshSummarySelectedParkWay);
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
        private void ReciveRefreshSummarySelectedParkWay(object recipient, SelectedParkWayMessage message)
        {
          var selectedCount = ParkWays?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ParkWays.Count())
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
            if (message.CurrentModelType != typeof(ParkWay)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<ParkWay> predicate = null, Expression<Func<ParkWay, object>> expression = null, bool isDesc = false)
        {
          ParkWays = new ObservableCollection<ParkWay>(_parkWayDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _parkWayDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ParkWay> predicate = null, Expression<Func<ParkWay, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _parkWayDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              ParkWays = new ObservableCollection<ParkWay>(result);
                AllCount = _parkWayDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ParkWays.ToList(), "ParkWays.xlsx", I18nManager.GetString("ParkWayInfo"));
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
                foreach (var item in ParkWays)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ParkWays)
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
                    case "Amount":
                        _expression = u => u.Amount; break;
                    case "AreaId":
                        _expression = u => u.AreaId; break;
                    case "Id":
                        _expression = u => u.Id; break;
                    case "CarInId":
                        _expression = u => u.CarInId; break;
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "CarNoColor":
                        _expression = u => u.CarNoColor; break;
                    case "CarNoType":
                        _expression = u => u.CarNoType; break;
                    case "CarStatus":
                        _expression = u => u.CarStatus; break;
                    case "CarTypeId":
                        _expression = u => u.CarTypeId; break;
                    case "ChangedCarNo":
                        _expression = u => u.ChangedCarNo; break;
                    case "Discount":
                        _expression = u => u.Discount; break;
                    case "Display":
                        _expression = u => u.Display; break;
                    case "InImage":
                        _expression = u => u.InImage; break;
                    case "InTime":
                        _expression = u => u.InTime; break;
                    case "IsAllowEnter":
                        _expression = u => u.IsAllowEnter; break;
                    case "IsCsConfirm":
                        _expression = u => u.IsCsConfirm; break;
                    case "IsNeedAysn":
                        _expression = u => u.IsNeedAysn; break;
                    case "IsPaid":
                        _expression = u => u.IsPaid; break;
                    case "LastCarNo":
                        _expression = u => u.LastCarNo; break;
                    case "LastCarTime":
                        _expression = u => u.LastCarTime; break;
                    case "OrderId":
                        _expression = u => u.OrderId; break;
                    case "Paid":
                        _expression = u => u.Paid; break;
                    case "PlateId":
                        _expression = u => u.PlateId; break;
                    case "RecStatus":
                        _expression = u => u.RecStatus; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "SpecialCar":
                        _expression = u => u.SpecialCar; break;
                    case "TriggerFlag":
                        _expression = u => u.TriggerFlag; break;
                    case "UpdateDt":
                        _expression = u => u.UpdateDt; break;
                    case "UpdateUser":
                        _expression = u => u.UpdateUser; break;
                    case "VideoCall":
                        _expression = u => u.VideoCall; break;
                    case "VideoCallQrcode":
                        _expression = u => u.VideoCallQrcode; break;
                    case "VideoCallTime":
                        _expression = u => u.VideoCallTime; break;
                    case "Voice":
                        _expression = u => u.Voice; break;
                    case "WaitPay":
                        _expression = u => u.WaitPay; break;
                    case "WaittingCarNo":
                        _expression = u => u.WaittingCarNo; break;
                    case "WaittingCarNoColor":
                        _expression = u => u.WaittingCarNoColor; break;
                    case "WaittingCarNoType":
                        _expression = u => u.WaittingCarNoType; break;
                    case "WaittingImg":
                        _expression = u => u.WaittingImg; break;
                    case "WaittingPlateId":
                        _expression = u => u.WaittingPlateId; break;
                    case "WaittingTime":
                        _expression = u => u.WaittingTime; break;
                    case "WayCarType":
                        _expression = u => u.WayCarType; break;
                    case "WayConnect":
                        _expression = u => u.WayConnect; break;
                    case "WayName":
                        _expression = u => u.WayName; break;
                    case "WayNo":
                        _expression = u => u.WayNo; break;
                    case "WayStatus":
                        _expression = u => u.WayStatus; break;
                    case "WayType":
                        _expression = u => u.WayType; break;
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
        private Task SearchParkWay()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ParkWay>(true);
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchDisplay)) 
                {
                  _predicate = _predicate.And(p => p.Display.Contains(SearchDisplay));
                }
                if (!String.IsNullOrEmpty(SearchInImage)) 
                {
                  _predicate = _predicate.And(p => p.InImage.Contains(SearchInImage));
                }
                if (!String.IsNullOrEmpty(SearchInTime)) 
                {
                  _predicate = _predicate.And(p => p.InTime.Contains(SearchInTime));
                }
                if (!String.IsNullOrEmpty(SearchLastCarNo)) 
                {
                  _predicate = _predicate.And(p => p.LastCarNo.Contains(SearchLastCarNo));
                }
                if (!String.IsNullOrEmpty(SearchLastCarTime)) 
                {
                  _predicate = _predicate.And(p => p.LastCarTime.Contains(SearchLastCarTime));
                }
                if (!String.IsNullOrEmpty(SearchOrderId)) 
                {
                  _predicate = _predicate.And(p => p.OrderId.Contains(SearchOrderId));
                }
                if (!String.IsNullOrEmpty(SearchPlateId)) 
                {
                  _predicate = _predicate.And(p => p.PlateId.Contains(SearchPlateId));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchUpdateDt)) 
                {
                  _predicate = _predicate.And(p => p.UpdateDt.Contains(SearchUpdateDt));
                }
                if (!String.IsNullOrEmpty(SearchVideoCallTime)) 
                {
                  _predicate = _predicate.And(p => p.VideoCallTime.Contains(SearchVideoCallTime));
                }
                if (!String.IsNullOrEmpty(SearchVoice)) 
                {
                  _predicate = _predicate.And(p => p.Voice.Contains(SearchVoice));
                }
                if (!String.IsNullOrEmpty(SearchWaittingCarNo)) 
                {
                  _predicate = _predicate.And(p => p.WaittingCarNo.Contains(SearchWaittingCarNo));
                }
                if (!String.IsNullOrEmpty(SearchWaittingImg)) 
                {
                  _predicate = _predicate.And(p => p.WaittingImg.Contains(SearchWaittingImg));
                }
                if (!String.IsNullOrEmpty(SearchWaittingPlateId)) 
                {
                  _predicate = _predicate.And(p => p.WaittingPlateId.Contains(SearchWaittingPlateId));
                }
                if (!String.IsNullOrEmpty(SearchWaittingTime)) 
                {
                  _predicate = _predicate.And(p => p.WaittingTime.Contains(SearchWaittingTime));
                }
                if (!String.IsNullOrEmpty(SearchWayName)) 
                {
                  _predicate = _predicate.And(p => p.WayName.Contains(SearchWayName));
                }
                if (!String.IsNullOrEmpty(SearchWayNo)) 
                {
                  _predicate = _predicate.And(p => p.WayNo.Contains(SearchWayNo));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCarNo = string.Empty;
            SearchDisplay = string.Empty;
            SearchInImage = string.Empty;
            SearchInTime = string.Empty;
            SearchLastCarNo = string.Empty;
            SearchLastCarTime = string.Empty;
            SearchOrderId = string.Empty;
            SearchPlateId = string.Empty;
            SearchRemark = string.Empty;
            SearchUpdateDt = string.Empty;
            SearchVideoCallTime = string.Empty;
            SearchVoice = string.Empty;
            SearchWaittingCarNo = string.Empty;
            SearchWaittingImg = string.Empty;
            SearchWaittingPlateId = string.Empty;
            SearchWaittingTime = string.Empty;
            SearchWayName = string.Empty;
            SearchWayNo = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<ParkWayActionWindowViewModel>();
            actionVM.CarNo = string.Empty;
            actionVM.Display = string.Empty;
            actionVM.InImage = string.Empty;
            actionVM.InTime = string.Empty;
            actionVM.LastCarNo = string.Empty;
            actionVM.LastCarTime = string.Empty;
            actionVM.OrderId = string.Empty;
            actionVM.PlateId = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.UpdateDt = string.Empty;
            actionVM.VideoCallTime = string.Empty;
            actionVM.Voice = string.Empty;
            actionVM.WaittingCarNo = string.Empty;
            actionVM.WaittingImg = string.Empty;
            actionVM.WaittingPlateId = string.Empty;
            actionVM.WaittingTime = string.Empty;
            actionVM.WayName = string.Empty;
            actionVM.WayNo = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkWay = true;
            actionVM.Title = I18nManager.GetString("CreateNewParkWay"); 
            var actionWindow = App.Views.CreateView<ParkWayActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateParkWay(ParkWay parkWay)
        {
            var actionVM = App.ServiceProvider.GetService<ParkWayActionWindowViewModel>();
            actionVM.SelectedParkWay = parkWay;
            actionVM.CarNo = parkWay.CarNo;
            actionVM.Display = parkWay.Display;
            actionVM.InImage = parkWay.InImage;
            actionVM.InTime = parkWay.InTime;
            actionVM.LastCarNo = parkWay.LastCarNo;
            actionVM.LastCarTime = parkWay.LastCarTime;
            actionVM.OrderId = parkWay.OrderId;
            actionVM.PlateId = parkWay.PlateId;
            actionVM.Remark = parkWay.Remark;
            actionVM.UpdateDt = parkWay.UpdateDt;
            actionVM.VideoCallTime = parkWay.VideoCallTime;
            actionVM.Voice = parkWay.Voice;
            actionVM.WaittingCarNo = parkWay.WaittingCarNo;
            actionVM.WaittingImg = parkWay.WaittingImg;
            actionVM.WaittingPlateId = parkWay.WaittingPlateId;
            actionVM.WaittingTime = parkWay.WaittingTime;
            actionVM.WayName = parkWay.WayName;
            actionVM.WayNo = parkWay.WayNo;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkWay = false;
            actionVM.Title = I18nManager.GetString("UpdateParkWayInfo");
            var actionWindow = App.Views.CreateView<ParkWayActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteParkWay()
        {
            var selectedCount = ParkWays.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkWayPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteParkWay"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkWay"))
                .WithContent(I18nManager.GetString("SureWantToDeleteParkWay"))
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
            var selectedIds = ParkWays.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _parkWayDao.DeleteRangeAsync<ParkWay>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteParkWayPrompt");
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
                                    CurrentModelType = typeof(ParkWay),
                                    Title = I18nManager.GetString("DeleteParkWayPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
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

