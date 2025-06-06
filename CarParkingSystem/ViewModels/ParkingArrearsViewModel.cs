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
  public partial class ParkingArrearsViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkingArrears> _parkingArrearss;
        [ObservableProperty] private ParkingArrears _selectedParkingArrears;
        [ObservableProperty] private string _updateInfo;
        private ParkingArrearsDao _parkingArrearsDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkingArrears> _predicate { get; set; }
        public Expression<Func<ParkingArrears, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.String _searchAmountMoney;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchCarColor;
        [ObservableProperty] private System.Int32 _searchCardNo;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.Int32 _searchCarOutId;
        [ObservableProperty] private System.Int32 _searchCarType;
        [ObservableProperty] private System.String _searchDiscountMoney;
        [ObservableProperty] private System.Decimal _searchFee;
        [ObservableProperty] private System.String _searchInImg;
        [ObservableProperty] private System.Int32 _searchInOperatorId;
        [ObservableProperty] private System.String _searchInRemark;
        [ObservableProperty] private System.String _searchInTime;
        [ObservableProperty] private System.Int32 _searchInType;
        [ObservableProperty] private System.Int32 _searchInWayId;
        [ObservableProperty] private System.String _searchOrderId;
        [ObservableProperty] private System.String _searchOutImg;
        [ObservableProperty] private System.Int32 _searchOutOperatorId;
        [ObservableProperty] private System.String _searchOutRemark;
        [ObservableProperty] private System.String _searchOutTime;
        [ObservableProperty] private System.Int32 _searchOutType;
        [ObservableProperty] private System.Int32 _searchOutWayId;
        [ObservableProperty] private System.String _searchPaidMoney;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public ParkingArrearsViewModel(ParkingArrearsDao parkingArrearsDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.ParkingArrears, MaterialIconKind.Abacus, pid: 5, id: 29, index: 29)
        {
            _parkingArrearsDao = parkingArrearsDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedParkingArrearsMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN, ReciveRefreshSummarySelectedParkingArrears);
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
        private void ReciveRefreshSummarySelectedParkingArrears(object recipient, SelectedParkingArrearsMessage message)
        {
          var selectedCount = ParkingArrearss?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ParkingArrearss.Count())
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
            if (message.CurrentModelType != typeof(ParkingArrears)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<ParkingArrears> predicate = null, Expression<Func<ParkingArrears, object>> expression = null, bool isDesc = false)
        {
          ParkingArrearss = new ObservableCollection<ParkingArrears>(_parkingArrearsDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _parkingArrearsDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ParkingArrears> predicate = null, Expression<Func<ParkingArrears, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _parkingArrearsDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              ParkingArrearss = new ObservableCollection<ParkingArrears>(result);
                AllCount = _parkingArrearsDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ParkingArrearss.ToList(), "ParkingArrearss.xlsx", I18nManager.GetString("ParkingArrearsInfo"));
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
                foreach (var item in ParkingArrearss)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ParkingArrearss)
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
                    case "AmountMoney":
                        _expression = u => u.AmountMoney; break;
                    case "Id":
                        _expression = u => u.Id; break;
                    case "CarColor":
                        _expression = u => u.CarColor; break;
                    case "CardNo":
                        _expression = u => u.CardNo; break;
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "CarOutId":
                        _expression = u => u.CarOutId; break;
                    case "CarType":
                        _expression = u => u.CarType; break;
                    case "DiscountMoney":
                        _expression = u => u.DiscountMoney; break;
                    case "Fee":
                        _expression = u => u.Fee; break;
                    case "InImg":
                        _expression = u => u.InImg; break;
                    case "InOperatorId":
                        _expression = u => u.InOperatorId; break;
                    case "InRemark":
                        _expression = u => u.InRemark; break;
                    case "InTime":
                        _expression = u => u.InTime; break;
                    case "InType":
                        _expression = u => u.InType; break;
                    case "InWayId":
                        _expression = u => u.InWayId; break;
                    case "OrderId":
                        _expression = u => u.OrderId; break;
                    case "OutImg":
                        _expression = u => u.OutImg; break;
                    case "OutOperatorId":
                        _expression = u => u.OutOperatorId; break;
                    case "OutRemark":
                        _expression = u => u.OutRemark; break;
                    case "OutTime":
                        _expression = u => u.OutTime; break;
                    case "OutType":
                        _expression = u => u.OutType; break;
                    case "OutWayId":
                        _expression = u => u.OutWayId; break;
                    case "PaidMoney":
                        _expression = u => u.PaidMoney; break;
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
        private Task SearchParkingArrears()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ParkingArrears>(true);
                if (!String.IsNullOrEmpty(SearchAmountMoney)) 
                {
                  _predicate = _predicate.And(p => p.AmountMoney.Contains(SearchAmountMoney));
                }
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchDiscountMoney)) 
                {
                  _predicate = _predicate.And(p => p.DiscountMoney.Contains(SearchDiscountMoney));
                }
                if (!String.IsNullOrEmpty(SearchInImg)) 
                {
                  _predicate = _predicate.And(p => p.InImg.Contains(SearchInImg));
                }
                if (!String.IsNullOrEmpty(SearchInRemark)) 
                {
                  _predicate = _predicate.And(p => p.InRemark.Contains(SearchInRemark));
                }
                if (!String.IsNullOrEmpty(SearchInTime)) 
                {
                  _predicate = _predicate.And(p => p.InTime.Contains(SearchInTime));
                }
                if (!String.IsNullOrEmpty(SearchOrderId)) 
                {
                  _predicate = _predicate.And(p => p.OrderId.Contains(SearchOrderId));
                }
                if (!String.IsNullOrEmpty(SearchOutImg)) 
                {
                  _predicate = _predicate.And(p => p.OutImg.Contains(SearchOutImg));
                }
                if (!String.IsNullOrEmpty(SearchOutRemark)) 
                {
                  _predicate = _predicate.And(p => p.OutRemark.Contains(SearchOutRemark));
                }
                if (!String.IsNullOrEmpty(SearchOutTime)) 
                {
                  _predicate = _predicate.And(p => p.OutTime.Contains(SearchOutTime));
                }
                if (!String.IsNullOrEmpty(SearchPaidMoney)) 
                {
                  _predicate = _predicate.And(p => p.PaidMoney.Contains(SearchPaidMoney));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchAmountMoney = string.Empty;
            SearchCarNo = string.Empty;
            SearchDiscountMoney = string.Empty;
            SearchInImg = string.Empty;
            SearchInRemark = string.Empty;
            SearchInTime = string.Empty;
            SearchOrderId = string.Empty;
            SearchOutImg = string.Empty;
            SearchOutRemark = string.Empty;
            SearchOutTime = string.Empty;
            SearchPaidMoney = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<ParkingArrearsActionWindowViewModel>();
            actionVM.AmountMoney = string.Empty;
            actionVM.CarNo = string.Empty;
            actionVM.DiscountMoney = string.Empty;
            actionVM.InImg = string.Empty;
            actionVM.InRemark = string.Empty;
            actionVM.InTime = string.Empty;
            actionVM.OrderId = string.Empty;
            actionVM.OutImg = string.Empty;
            actionVM.OutRemark = string.Empty;
            actionVM.OutTime = string.Empty;
            actionVM.PaidMoney = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkingArrears = true;
            actionVM.Title = I18nManager.GetString("CreateNewParkingArrears"); 
            var actionWindow = App.Views.CreateView<ParkingArrearsActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateParkingArrears(ParkingArrears parkingArrears)
        {
            var actionVM = App.ServiceProvider.GetService<ParkingArrearsActionWindowViewModel>();
            actionVM.SelectedParkingArrears = parkingArrears;
            actionVM.AmountMoney = parkingArrears.AmountMoney;
            actionVM.CarNo = parkingArrears.CarNo;
            actionVM.DiscountMoney = parkingArrears.DiscountMoney;
            actionVM.InImg = parkingArrears.InImg;
            actionVM.InRemark = parkingArrears.InRemark;
            actionVM.InTime = parkingArrears.InTime;
            actionVM.OrderId = parkingArrears.OrderId;
            actionVM.OutImg = parkingArrears.OutImg;
            actionVM.OutRemark = parkingArrears.OutRemark;
            actionVM.OutTime = parkingArrears.OutTime;
            actionVM.PaidMoney = parkingArrears.PaidMoney;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkingArrears = false;
            actionVM.Title = I18nManager.GetString("UpdateParkingArrearsInfo");
            var actionWindow = App.Views.CreateView<ParkingArrearsActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteParkingArrears()
        {
            var selectedCount = ParkingArrearss.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkingArrearsPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteParkingArrears"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkingArrears"))
                .WithContent(I18nManager.GetString("SureWantToDeleteParkingArrears"))
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
            var selectedIds = ParkingArrearss.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _parkingArrearsDao.DeleteRangeAsync<ParkingArrears>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteParkingArrearsPrompt");
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
                                    CurrentModelType = typeof(ParkingArrears),
                                    Title = I18nManager.GetString("DeleteParkingArrearsPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
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

