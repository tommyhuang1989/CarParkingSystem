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
  public partial class HandOverViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<HandOver> _handOvers;
        [ObservableProperty] private HandOver _selectedHandOver;
        [ObservableProperty] private string _updateInfo;
        private HandOverDao _handOverDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<HandOver> _predicate { get; set; }
        public Expression<Func<HandOver, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchAbCar;
        [ObservableProperty] private System.Int32 _searchArrearsCar;
        [ObservableProperty] private System.Decimal _searchCashFee;
        [ObservableProperty] private System.String _searchEndDate;
        [ObservableProperty] private System.Decimal _searchEtcfee;
        [ObservableProperty] private System.Int32 _searchInCar;
        [ObservableProperty] private System.Int32 _searchIsFinished;
        [ObservableProperty] private System.Int32 _searchOutCar;
        [ObservableProperty] private System.Decimal _searchPhoneFee;
        [ObservableProperty] private System.String _searchStartDate;
        [ObservableProperty] private System.Decimal _searchTotalFee;
        [ObservableProperty] private System.Int32 _searchUserId;
        [ObservableProperty] private System.Decimal _searchValueCardFee;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public HandOverViewModel(HandOverDao handOverDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.HandOverRecord, MaterialIconKind.Abacus, pid: 2, id: 16, index: 16)
        {
            _handOverDao = handOverDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedHandOverMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN, ReciveRefreshSummarySelectedHandOver);
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
        private void ReciveRefreshSummarySelectedHandOver(object recipient, SelectedHandOverMessage message)
        {
          var selectedCount = HandOvers?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == HandOvers.Count())
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
            if (message.CurrentModelType != typeof(HandOver)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<HandOver> predicate = null, Expression<Func<HandOver, object>> expression = null, bool isDesc = false)
        {
          HandOvers = new ObservableCollection<HandOver>(_handOverDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _handOverDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<HandOver> predicate = null, Expression<Func<HandOver, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _handOverDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              HandOvers = new ObservableCollection<HandOver>(result);
                AllCount = _handOverDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, HandOvers.ToList(), "HandOvers.xlsx", I18nManager.GetString("HandOverInfo"));
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
                foreach (var item in HandOvers)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in HandOvers)
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
                    case "AbCar":
                        _expression = u => u.AbCar; break;
                    case "ArrearsCar":
                        _expression = u => u.ArrearsCar; break;
                    case "CashFee":
                        _expression = u => u.CashFee; break;
                    case "EndDate":
                        _expression = u => u.EndDate; break;
                    case "Etcfee":
                        _expression = u => u.Etcfee; break;
                    case "InCar":
                        _expression = u => u.InCar; break;
                    case "IsFinished":
                        _expression = u => u.IsFinished; break;
                    case "OutCar":
                        _expression = u => u.OutCar; break;
                    case "PhoneFee":
                        _expression = u => u.PhoneFee; break;
                    case "StartDate":
                        _expression = u => u.StartDate; break;
                    case "TotalFee":
                        _expression = u => u.TotalFee; break;
                    case "UserId":
                        _expression = u => u.UserId; break;
                    case "ValueCardFee":
                        _expression = u => u.ValueCardFee; break;
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
        private Task SearchHandOver()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<HandOver>(true);
                if (!String.IsNullOrEmpty(SearchEndDate)) 
                {
                  _predicate = _predicate.And(p => p.EndDate.Contains(SearchEndDate));
                }
                if (!String.IsNullOrEmpty(SearchStartDate)) 
                {
                  _predicate = _predicate.And(p => p.StartDate.Contains(SearchStartDate));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchEndDate = string.Empty;
            SearchStartDate = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<HandOverActionWindowViewModel>();
            actionVM.EndDate = null;
            actionVM.StartDate = null;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddHandOver = true;
            actionVM.Title = I18nManager.GetString("CreateNewHandOver"); 
            var actionWindow = App.Views.CreateView<HandOverActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateHandOver(HandOver handOver)
        {
            var actionVM = App.ServiceProvider.GetService<HandOverActionWindowViewModel>();
            actionVM.SelectedHandOver = handOver;
            actionVM.EndDate = handOver.EndDate;
            actionVM.StartDate = handOver.StartDate;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddHandOver = false;
            actionVM.Title = I18nManager.GetString("UpdateHandOverInfo");
            var actionWindow = App.Views.CreateView<HandOverActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteHandOver()
        {
            var selectedCount = HandOvers.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteHandOverPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteHandOver"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteHandOver"))
                .WithContent(I18nManager.GetString("SureWantToDeleteHandOver"))
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
            var selectedIds = HandOvers.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _handOverDao.DeleteRangeAsync<HandOver>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteHandOverPrompt");
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
                                    CurrentModelType = typeof(HandOver),
                                    Title = I18nManager.GetString("DeleteHandOverPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
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

