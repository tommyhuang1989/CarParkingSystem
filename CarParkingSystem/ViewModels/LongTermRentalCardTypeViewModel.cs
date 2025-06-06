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
  public partial class LongTermRentalCardTypeViewModel: LongTermRentalCarTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<LongTermRentalCardType> _longTermRentalCardTypes;
        [ObservableProperty] private LongTermRentalCardType _selectedLongTermRentalCardType;
        [ObservableProperty] private string _updateInfo;
        private LongTermRentalCardTypeDao _longTermRentalCardTypeDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<LongTermRentalCardType> _predicate { get; set; }
        public Expression<Func<LongTermRentalCardType, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Decimal _searchAmount;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.String _searchCardName;
        [ObservableProperty] private System.Int32 _searchCardType;
        [ObservableProperty] private System.Int32 _searchCarSpace;
        [ObservableProperty] private System.String _searchEndDate;
        [ObservableProperty] private System.Int32 _searchExpireCard;
        [ObservableProperty] private System.Int32 _searchFeeRuleType;
        [ObservableProperty] private System.Int32 _searchInCheck;
        [ObservableProperty] private System.String _searchMonthToTempDiscount;
        [ObservableProperty] private System.Int32 _searchRecStatus;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.String _searchStartDate;
        [ObservableProperty] private System.Int32 _searchTotalCar;
        [ObservableProperty] private System.String _searchUpdateDt;
        [ObservableProperty] private System.Int32 _searchUpdateUser;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public LongTermRentalCardTypeViewModel(LongTermRentalCardTypeDao longTermRentalCardTypeDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.LongTermRentalType)
        {
            _longTermRentalCardTypeDao = longTermRentalCardTypeDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedLongTermRentalCardTypeMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN, ReciveRefreshSummarySelectedLongTermRentalCardType);
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
        private void ReciveRefreshSummarySelectedLongTermRentalCardType(object recipient, SelectedLongTermRentalCardTypeMessage message)
        {
          var selectedCount = LongTermRentalCardTypes?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == LongTermRentalCardTypes.Count())
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
            if (message.CurrentModelType != typeof(LongTermRentalCardType)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<LongTermRentalCardType> predicate = null, Expression<Func<LongTermRentalCardType, object>> expression = null, bool isDesc = false)
        {
          LongTermRentalCardTypes = new ObservableCollection<LongTermRentalCardType>(_longTermRentalCardTypeDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _longTermRentalCardTypeDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<LongTermRentalCardType> predicate = null, Expression<Func<LongTermRentalCardType, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _longTermRentalCardTypeDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              LongTermRentalCardTypes = new ObservableCollection<LongTermRentalCardType>(result);
                AllCount = _longTermRentalCardTypeDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, LongTermRentalCardTypes.ToList(), "LongTermRentalCardTypes.xlsx", I18nManager.GetString("LongTermRentalCardTypeInfo"));
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
                foreach (var item in LongTermRentalCardTypes)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in LongTermRentalCardTypes)
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
                    case "Id":
                        _expression = u => u.Id; break;
                    case "CardName":
                        _expression = u => u.CardName; break;
                    case "CardType":
                        _expression = u => u.CardType; break;
                    case "CarSpace":
                        _expression = u => u.CarSpace; break;
                    case "EndDate":
                        _expression = u => u.EndDate; break;
                    case "ExpireCard":
                        _expression = u => u.ExpireCard; break;
                    case "FeeRuleType":
                        _expression = u => u.FeeRuleType; break;
                    case "InCheck":
                        _expression = u => u.InCheck; break;
                    case "MonthToTempDiscount":
                        _expression = u => u.MonthToTempDiscount; break;
                    case "RecStatus":
                        _expression = u => u.RecStatus; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "StartDate":
                        _expression = u => u.StartDate; break;
                    case "TotalCar":
                        _expression = u => u.TotalCar; break;
                    case "UpdateDt":
                        _expression = u => u.UpdateDt; break;
                    case "UpdateUser":
                        _expression = u => u.UpdateUser; break;
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
        private Task SearchLongTermRentalCardType()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<LongTermRentalCardType>(true);
                if (!String.IsNullOrEmpty(SearchCardName)) 
                {
                  _predicate = _predicate.And(p => p.CardName.Contains(SearchCardName));
                }
                if (!String.IsNullOrEmpty(SearchEndDate)) 
                {
                  _predicate = _predicate.And(p => p.EndDate.Contains(SearchEndDate));
                }
                if (!String.IsNullOrEmpty(SearchMonthToTempDiscount)) 
                {
                  _predicate = _predicate.And(p => p.MonthToTempDiscount.Contains(SearchMonthToTempDiscount));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchStartDate)) 
                {
                  _predicate = _predicate.And(p => p.StartDate.Contains(SearchStartDate));
                }
                if (!String.IsNullOrEmpty(SearchUpdateDt)) 
                {
                  _predicate = _predicate.And(p => p.UpdateDt.Contains(SearchUpdateDt));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCardName = string.Empty;
            SearchEndDate = string.Empty;
            SearchMonthToTempDiscount = string.Empty;
            SearchRemark = string.Empty;
            SearchStartDate = string.Empty;
            SearchUpdateDt = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<LongTermRentalCardTypeActionWindowViewModel>();
            actionVM.CardName = string.Empty;
            actionVM.EndDate = null;
            actionVM.MonthToTempDiscount = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.StartDate = null;
            actionVM.UpdateDt = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddLongTermRentalCardType = true;
            actionVM.Title = I18nManager.GetString("CreateNewLongTermRentalCardType"); 
            var actionWindow = App.Views.CreateView<LongTermRentalCardTypeActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateLongTermRentalCardType(LongTermRentalCardType longTermRentalCardType)
        {
            var actionVM = App.ServiceProvider.GetService<LongTermRentalCardTypeActionWindowViewModel>();
            actionVM.SelectedLongTermRentalCardType = longTermRentalCardType;
            actionVM.CardName = longTermRentalCardType.CardName;
            actionVM.EndDate = longTermRentalCardType.EndDate;
            actionVM.MonthToTempDiscount = longTermRentalCardType.MonthToTempDiscount;
            actionVM.Remark = longTermRentalCardType.Remark;
            actionVM.StartDate = longTermRentalCardType.StartDate;
            actionVM.UpdateDt = longTermRentalCardType.UpdateDt;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddLongTermRentalCardType = false;
            actionVM.Title = I18nManager.GetString("UpdateLongTermRentalCardTypeInfo");
            var actionWindow = App.Views.CreateView<LongTermRentalCardTypeActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteLongTermRentalCardType()
        {
            var selectedCount = LongTermRentalCardTypes.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteLongTermRentalCardTypePrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteLongTermRentalCardType"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteLongTermRentalCardType"))
                .WithContent(I18nManager.GetString("SureWantToDeleteLongTermRentalCardType"))
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
            var selectedIds = LongTermRentalCardTypes.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _longTermRentalCardTypeDao.DeleteRangeAsync<LongTermRentalCardType>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteLongTermRentalCardTypePrompt");
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
                                    CurrentModelType = typeof(LongTermRentalCardType),
                                    Title = I18nManager.GetString("DeleteLongTermRentalCardTypePrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCARDTYPE_TOKEN);
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

