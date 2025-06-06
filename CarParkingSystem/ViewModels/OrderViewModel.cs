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
  public partial class OrderViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<Order> _orders;
        [ObservableProperty] private Order _selectedOrder;
        [ObservableProperty] private string _updateInfo;
        private OrderDao _orderDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<Order> _predicate { get; set; }
        public Expression<Func<Order, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.String _searchMerchant;
        [ObservableProperty] private System.Int32 _searchProductType;
        [ObservableProperty] private System.String _searchProductId;
        [ObservableProperty] private System.String _searchBuyer;
        [ObservableProperty] private System.Int32 _searchPayOrder;
        [ObservableProperty] private System.Int32 _searchPayName;
        [ObservableProperty] private System.Int32 _searchPayMoney;
        [ObservableProperty] private System.String _searchCreateDate;
        [ObservableProperty] private System.Int32 _searchPayStatus;
        [ObservableProperty] private System.Int32 _searchPayType;
        [ObservableProperty] private System.Int32 _searchClientType;
        [ObservableProperty] private System.String _searchClientId;
        [ObservableProperty] private System.String _searchPayUrl;
        [ObservableProperty] private System.String _searchExpireDate;
        [ObservableProperty] private System.Int32 _searchIsProfitSharing;
        [ObservableProperty] private System.String _searchTransactionId;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.String _searchPhone;
        [ObservableProperty] private System.Int32 _searchBuyNumber;
        [ObservableProperty] private System.String _searchStartTime;
        [ObservableProperty] private System.String _searchEndTime;
        [ObservableProperty] private System.String _searchInvoice;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public OrderViewModel(OrderDao orderDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.OrderRecord, MaterialIconKind.Abacus, pid: 6, id: 33, index: 33)
        {
          _orderDao = orderDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedOrderMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN, ReciveRefreshSummarySelectedOrder);
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
        private void ReciveRefreshSummarySelectedOrder(object recipient, SelectedOrderMessage message)
        {
          var selectedCount = Orders?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == Orders.Count())
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
            if (message.CurrentModelType != typeof(Order)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<Order> predicate = null, Expression<Func<Order, object>> expression = null, bool isDesc = false)
        {
          Orders = new ObservableCollection<Order>(_orderDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _orderDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<Order> predicate = null, Expression<Func<Order, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _orderDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              Orders = new ObservableCollection<Order>(result);
                AllCount = _orderDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, Orders.ToList(), "Orders.xlsx", I18nManager.GetString("OrderInfo"));
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
                foreach (var item in Orders)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in Orders)
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
                    case "Merchant":
                        _expression = u => u.Merchant; break;
                    case "ProductType":
                        _expression = u => u.ProductType; break;
                    case "ProductId":
                        _expression = u => u.ProductId; break;
                    case "Buyer":
                        _expression = u => u.Buyer; break;
                    case "PayOrder":
                        _expression = u => u.PayOrder; break;
                    case "PayName":
                        _expression = u => u.PayName; break;
                    case "PayMoney":
                        _expression = u => u.PayMoney; break;
                    case "CreateDate":
                        _expression = u => u.CreateDate; break;
                    case "PayStatus":
                        _expression = u => u.PayStatus; break;
                    case "PayType":
                        _expression = u => u.PayType; break;
                    case "ClientType":
                        _expression = u => u.ClientType; break;
                    case "ClientId":
                        _expression = u => u.ClientId; break;
                    case "PayUrl":
                        _expression = u => u.PayUrl; break;
                    case "ExpireDate":
                        _expression = u => u.ExpireDate; break;
                    case "IsProfitSharing":
                        _expression = u => u.IsProfitSharing; break;
                    case "TransactionId":
                        _expression = u => u.TransactionId; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "Phone":
                        _expression = u => u.Phone; break;
                    case "BuyNumber":
                        _expression = u => u.BuyNumber; break;
                    case "StartTime":
                        _expression = u => u.StartTime; break;
                    case "EndTime":
                        _expression = u => u.EndTime; break;
                    case "Invoice":
                        _expression = u => u.Invoice; break;
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
        private Task SearchOrder()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<Order>(true);
                if (!String.IsNullOrEmpty(SearchMerchant)) 
                {
                  _predicate = _predicate.And(p => p.Merchant.Contains(SearchMerchant));
                }
                if (!String.IsNullOrEmpty(SearchProductId)) 
                {
                  _predicate = _predicate.And(p => p.ProductId.Contains(SearchProductId));
                }
                if (!String.IsNullOrEmpty(SearchBuyer)) 
                {
                  _predicate = _predicate.And(p => p.Buyer.Contains(SearchBuyer));
                }
                if (!String.IsNullOrEmpty(SearchCreateDate)) 
                {
                  _predicate = _predicate.And(p => p.CreateDate.Contains(SearchCreateDate));
                }
                if (!String.IsNullOrEmpty(SearchClientId)) 
                {
                  _predicate = _predicate.And(p => p.ClientId.Contains(SearchClientId));
                }
                if (!String.IsNullOrEmpty(SearchPayUrl)) 
                {
                  _predicate = _predicate.And(p => p.PayUrl.Contains(SearchPayUrl));
                }
                if (!String.IsNullOrEmpty(SearchExpireDate)) 
                {
                  _predicate = _predicate.And(p => p.ExpireDate.Contains(SearchExpireDate));
                }
                if (!String.IsNullOrEmpty(SearchTransactionId)) 
                {
                  _predicate = _predicate.And(p => p.TransactionId.Contains(SearchTransactionId));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchPhone)) 
                {
                  _predicate = _predicate.And(p => p.Phone.Contains(SearchPhone));
                }
                if (!String.IsNullOrEmpty(SearchStartTime)) 
                {
                  _predicate = _predicate.And(p => p.StartTime.Contains(SearchStartTime));
                }
                if (!String.IsNullOrEmpty(SearchEndTime)) 
                {
                  _predicate = _predicate.And(p => p.EndTime.Contains(SearchEndTime));
                }
                if (!String.IsNullOrEmpty(SearchInvoice)) 
                {
                  _predicate = _predicate.And(p => p.Invoice.Contains(SearchInvoice));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchMerchant = string.Empty;
            SearchProductId = string.Empty;
            SearchBuyer = string.Empty;
            SearchCreateDate = string.Empty;
            SearchClientId = string.Empty;
            SearchPayUrl = string.Empty;
            SearchExpireDate = string.Empty;
            SearchTransactionId = string.Empty;
            SearchRemark = string.Empty;
            SearchPhone = string.Empty;
            SearchStartTime = string.Empty;
            SearchEndTime = string.Empty;
            SearchInvoice = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<OrderActionWindowViewModel>();
            actionVM.Merchant = string.Empty;
            actionVM.ProductId = string.Empty;
            actionVM.Buyer = string.Empty;
            actionVM.CreateDate = null;
            actionVM.ClientId = string.Empty;
            actionVM.PayUrl = string.Empty;
            actionVM.ExpireDate = null;
            actionVM.TransactionId = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.Phone = string.Empty;
            actionVM.StartTime = string.Empty;
            actionVM.EndTime = string.Empty;
            actionVM.Invoice = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddOrder = true;
            actionVM.Title = I18nManager.GetString("CreateNewOrder"); 
            var actionWindow = App.Views.CreateView<OrderActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateOrder(Order order)
        {
            var actionVM = App.ServiceProvider.GetService<OrderActionWindowViewModel>();
            actionVM.SelectedOrder = order;
            actionVM.Merchant = order.Merchant;
            actionVM.ProductId = order.ProductId;
            actionVM.Buyer = order.Buyer;
            actionVM.CreateDate = order.CreateDate;
            actionVM.ClientId = order.ClientId;
            actionVM.PayUrl = order.PayUrl;
            actionVM.ExpireDate = order.ExpireDate;
            actionVM.TransactionId = order.TransactionId;
            actionVM.Remark = order.Remark;
            actionVM.Phone = order.Phone;
            actionVM.StartTime = order.StartTime;
            actionVM.EndTime = order.EndTime;
            actionVM.Invoice = order.Invoice;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddOrder = false;
            actionVM.Title = I18nManager.GetString("UpdateOrderInfo");
            var actionWindow = App.Views.CreateView<OrderActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteOrder()
        {
            var selectedCount = Orders.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteOrderPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteOrder"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteOrder"))
                .WithContent(I18nManager.GetString("SureWantToDeleteOrder"))
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
            var selectedIds = Orders.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _orderDao.DeleteRangeAsync<Order>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteOrderPrompt");
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
                                    CurrentModelType = typeof(Order),
                                    Title = I18nManager.GetString("DeleteOrderPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
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

