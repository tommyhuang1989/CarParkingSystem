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
using HarfBuzzSharp;
using CarParkingSystem.I18n;
namespace CarParkingSystem.ViewModels
{
  public partial class OrderRefundViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<OrderRefund> _orderRefunds;
        [ObservableProperty] private OrderRefund _selectedOrderRefund;
        [ObservableProperty] private string _updateInfo;
        private OrderRefundDao _orderRefundDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<OrderRefund> _predicate { get; set; }
        public Expression<Func<OrderRefund, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchProductType;
        [ObservableProperty] private System.String _searchProductId;
        [ObservableProperty] private System.String _searchBuyer;
        [ObservableProperty] private System.Int32 _searchOrderMoney;
        [ObservableProperty] private System.Int32 _searchRefundType;
        [ObservableProperty] private System.Int32 _searchRefundMoney;
        [ObservableProperty] private System.String _searchReason;
        [ObservableProperty] private System.Int32 _searchRefundStatus;
        [ObservableProperty] private System.String _searchRefundRemark;
        [ObservableProperty] private System.Int32 _searchCreateUser;
        [ObservableProperty] private System.String _searchCreateDate;
        [ObservableProperty] private System.Int32 _searchPayOrder;
        [ObservableProperty] private System.Int32 _searchRefundOrderId;
        [ObservableProperty] private System.String _searchRefundTransactionId;
        [ObservableProperty] private System.String _searchMerchant;
        [ObservableProperty] private System.String _searchTransactionId;
        [ObservableProperty] private System.Int32 _searchClientType;
        [ObservableProperty] private System.String _searchClientId;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public OrderRefundViewModel(OrderRefundDao orderRefundDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(I18n.Language.OrderRefundRecord, MaterialIconKind.Abacus, pid: 6, id: 34, index: 34)
        {
            _orderRefundDao = orderRefundDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedOrderRefundMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN, ReciveRefreshSummarySelectedOrderRefund);
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
        private void ReciveRefreshSummarySelectedOrderRefund(object recipient, SelectedOrderRefundMessage message)
        {
          var selectedCount = OrderRefunds?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == OrderRefunds.Count())
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
            if (message.CurrentModelType != typeof(OrderRefund)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<OrderRefund> predicate = null, Expression<Func<OrderRefund, object>> expression = null, bool isDesc = false)
        {
          OrderRefunds = new ObservableCollection<OrderRefund>(_orderRefundDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _orderRefundDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<OrderRefund> predicate = null, Expression<Func<OrderRefund, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _orderRefundDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              OrderRefunds = new ObservableCollection<OrderRefund>(result);
                AllCount = _orderRefundDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, OrderRefunds.ToList(), "OrderRefunds.xlsx", I18nManager.GetString("OrderRefundInfo"));
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
                foreach (var item in OrderRefunds)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in OrderRefunds)
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
                    case "Id":
                        _expression = u => u.Id; break;
                    case "ProductType":
                        _expression = u => u.ProductType; break;
                    case "ProductId":
                        _expression = u => u.ProductId; break;
                    case "Buyer":
                        _expression = u => u.Buyer; break;
                    case "OrderMoney":
                        _expression = u => u.OrderMoney; break;
                    case "RefundType":
                        _expression = u => u.RefundType; break;
                    case "RefundMoney":
                        _expression = u => u.RefundMoney; break;
                    case "Reason":
                        _expression = u => u.Reason; break;
                    case "RefundStatus":
                        _expression = u => u.RefundStatus; break;
                    case "RefundRemark":
                        _expression = u => u.RefundRemark; break;
                    case "CreateUser":
                        _expression = u => u.CreateUser; break;
                    case "CreateDate":
                        _expression = u => u.CreateDate; break;
                    case "PayOrder":
                        _expression = u => u.PayOrder; break;
                    case "RefundOrderId":
                        _expression = u => u.RefundOrderId; break;
                    case "RefundTransactionId":
                        _expression = u => u.RefundTransactionId; break;
                    case "Merchant":
                        _expression = u => u.Merchant; break;
                    case "TransactionId":
                        _expression = u => u.TransactionId; break;
                    case "ClientType":
                        _expression = u => u.ClientType; break;
                    case "ClientId":
                        _expression = u => u.ClientId; break;
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
        private Task SearchOrderRefund()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<OrderRefund>(true);
                if (!String.IsNullOrEmpty(SearchProductId)) 
                {
                  _predicate = _predicate.And(p => p.ProductId.Contains(SearchProductId));
                }
                if (!String.IsNullOrEmpty(SearchBuyer)) 
                {
                  _predicate = _predicate.And(p => p.Buyer.Contains(SearchBuyer));
                }
                if (!String.IsNullOrEmpty(SearchReason)) 
                {
                  _predicate = _predicate.And(p => p.Reason.Contains(SearchReason));
                }
                if (!String.IsNullOrEmpty(SearchRefundRemark)) 
                {
                  _predicate = _predicate.And(p => p.RefundRemark.Contains(SearchRefundRemark));
                }
                if (!String.IsNullOrEmpty(SearchCreateDate)) 
                {
                  _predicate = _predicate.And(p => p.CreateDate.Contains(SearchCreateDate));
                }
                if (!String.IsNullOrEmpty(SearchRefundTransactionId)) 
                {
                  _predicate = _predicate.And(p => p.RefundTransactionId.Contains(SearchRefundTransactionId));
                }
                if (!String.IsNullOrEmpty(SearchMerchant)) 
                {
                  _predicate = _predicate.And(p => p.Merchant.Contains(SearchMerchant));
                }
                if (!String.IsNullOrEmpty(SearchTransactionId)) 
                {
                  _predicate = _predicate.And(p => p.TransactionId.Contains(SearchTransactionId));
                }
                if (!String.IsNullOrEmpty(SearchClientId)) 
                {
                  _predicate = _predicate.And(p => p.ClientId.Contains(SearchClientId));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchProductId = string.Empty;
            SearchBuyer = string.Empty;
            SearchReason = string.Empty;
            SearchRefundRemark = string.Empty;
            SearchCreateDate = string.Empty;
            SearchRefundTransactionId = string.Empty;
            SearchMerchant = string.Empty;
            SearchTransactionId = string.Empty;
            SearchClientId = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<OrderRefundActionWindowViewModel>();
            actionVM.ProductId = string.Empty;
            actionVM.Buyer = string.Empty;
            actionVM.Reason = string.Empty;
            actionVM.RefundRemark = string.Empty;
            actionVM.CreateDate = null;
            actionVM.RefundTransactionId = string.Empty;
            actionVM.Merchant = string.Empty;
            actionVM.TransactionId = string.Empty;
            actionVM.ClientId = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddOrderRefund = true;
            actionVM.Title = I18nManager.GetString("CreateNewOrderRefund"); 
            var actionWindow = App.Views.CreateView<OrderRefundActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateOrderRefund(OrderRefund orderRefund)
        {
            var actionVM = App.ServiceProvider.GetService<OrderRefundActionWindowViewModel>();
            actionVM.SelectedOrderRefund = orderRefund;
            actionVM.ProductId = orderRefund.ProductId;
            actionVM.Buyer = orderRefund.Buyer;
            actionVM.Reason = orderRefund.Reason;
            actionVM.RefundRemark = orderRefund.RefundRemark;
            actionVM.CreateDate = orderRefund.CreateDate;
            actionVM.RefundTransactionId = orderRefund.RefundTransactionId;
            actionVM.Merchant = orderRefund.Merchant;
            actionVM.TransactionId = orderRefund.TransactionId;
            actionVM.ClientId = orderRefund.ClientId;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddOrderRefund = false;
            actionVM.Title = I18nManager.GetString("UpdateOrderRefundInfo");
            var actionWindow = App.Views.CreateView<OrderRefundActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteOrderRefund()
        {
            var selectedCount = OrderRefunds.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteOrderRefundPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteOrderRefund"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteOrderRefund"))
                .WithContent(I18nManager.GetString("SureWantToDeleteOrderRefund"))
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
            var selectedIds = OrderRefunds.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _orderRefundDao.DeleteRangeAsync<OrderRefund>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteOrderRefundPrompt");
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
                                    CurrentModelType = typeof(OrderRefund),
                                    Title = I18nManager.GetString("DeleteOrderRefundPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
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

