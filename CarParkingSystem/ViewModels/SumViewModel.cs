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
  public partial class SumViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<Sum> _sums;
        [ObservableProperty] private Sum _selectedSum;
        [ObservableProperty] private string _updateInfo;
        private SumDao _sumDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<Sum> _predicate { get; set; }
        public Expression<Func<Sum, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchCarOutCount;
        [ObservableProperty] private System.Decimal _searchCash;
        [ObservableProperty] private System.Decimal _searchChuzhi;
        [ObservableProperty] private System.Decimal _searchChuzhika;
        [ObservableProperty] private System.Decimal _searchDiscount;
        [ObservableProperty] private System.String _searchDt;
        [ObservableProperty] private System.Decimal _searchNeedPay;
        [ObservableProperty] private System.Decimal _searchPaid;
        [ObservableProperty] private System.Decimal _searchPp;
        [ObservableProperty] private System.Decimal _searchYueka;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public SumViewModel(SumDao sumDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.Summary, MaterialIconKind.Abacus, pid: 6, id: 32, index: 32)
        {
            _sumDao = sumDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedSumMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN, ReciveRefreshSummarySelectedSum);
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
        private void ReciveRefreshSummarySelectedSum(object recipient, SelectedSumMessage message)
        {
          var selectedCount = Sums?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == Sums.Count())
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
            if (message.CurrentModelType != typeof(Sum)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<Sum> predicate = null, Expression<Func<Sum, object>> expression = null, bool isDesc = false)
        {
          Sums = new ObservableCollection<Sum>(_sumDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _sumDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<Sum> predicate = null, Expression<Func<Sum, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _sumDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              Sums = new ObservableCollection<Sum>(result);
                AllCount = _sumDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, Sums.ToList(), "Sums.xlsx", I18nManager.GetString("SumInfo"));
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
                foreach (var item in Sums)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in Sums)
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
                    case "CarOutCount":
                        _expression = u => u.CarOutCount; break;
                    case "Cash":
                        _expression = u => u.Cash; break;
                    case "Chuzhi":
                        _expression = u => u.Chuzhi; break;
                    case "Chuzhika":
                        _expression = u => u.Chuzhika; break;
                    case "Discount":
                        _expression = u => u.Discount; break;
                    case "Dt":
                        _expression = u => u.Dt; break;
                    case "NeedPay":
                        _expression = u => u.NeedPay; break;
                    case "Paid":
                        _expression = u => u.Paid; break;
                    case "Pp":
                        _expression = u => u.Pp; break;
                    case "Yueka":
                        _expression = u => u.Yueka; break;
                    case "Id":
                        _expression = u => u.Id; break;
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
        private Task SearchSum()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<Sum>(true);
                if (!String.IsNullOrEmpty(SearchDt)) 
                {
                  _predicate = _predicate.And(p => p.Dt.Contains(SearchDt));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchDt = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<SumActionWindowViewModel>();
            actionVM.Dt = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddSum = true;
            actionVM.Title = I18nManager.GetString("CreateNewSum"); 
            var actionWindow = App.Views.CreateView<SumActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateSum(Sum sum)
        {
            var actionVM = App.ServiceProvider.GetService<SumActionWindowViewModel>();
            actionVM.SelectedSum = sum;
            actionVM.Dt = sum.Dt;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddSum = false;
            actionVM.Title = I18nManager.GetString("UpdateSumInfo");
            var actionWindow = App.Views.CreateView<SumActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteSum()
        {
            var selectedCount = Sums.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteSumPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteSum"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteSum"))
                .WithContent(I18nManager.GetString("SureWantToDeleteSum"))
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
            var selectedIds = Sums.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _sumDao.DeleteRangeAsync<Sum>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteSumPrompt");
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
                                    CurrentModelType = typeof(Sum),
                                    Title = I18nManager.GetString("DeleteSumPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
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

