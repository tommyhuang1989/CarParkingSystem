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
  public partial class FeeRuleSectionViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<FeeRuleSection> _feeRuleSections;
        [ObservableProperty] private FeeRuleSection _selectedFeeRuleSection;
        [ObservableProperty] private string _updateInfo;
        private FeeRuleSectionDao _feeRuleSectionDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<FeeRuleSection> _predicate { get; set; }
        public Expression<Func<FeeRuleSection, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchFeeRuleFlag;
        [ObservableProperty] private System.Int32 _searchFeeRuleId;
        [ObservableProperty] private System.Int32 _searchInWay;
        [ObservableProperty] private System.Int32 _searchOutWay;
        [ObservableProperty] private System.Int32 _searchOvertimeFeeRule;
        [ObservableProperty] private System.Int32 _searchOvertimeType;
        [ObservableProperty] private System.Int32 _searchParkingFeeRule;
        [ObservableProperty] private System.Int32 _searchParkingTime;
        [ObservableProperty] private System.String _searchSectionName;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public FeeRuleSectionViewModel(FeeRuleSectionDao feeRuleSectionDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.SectionFee, MaterialIconKind.Abacus, pid: 3, id: 18, index: 18)
        {
            _feeRuleSectionDao = feeRuleSectionDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedFeeRuleSectionMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN, ReciveRefreshSummarySelectedFeeRuleSection);
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
        private void ReciveRefreshSummarySelectedFeeRuleSection(object recipient, SelectedFeeRuleSectionMessage message)
        {
          var selectedCount = FeeRuleSections?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == FeeRuleSections.Count())
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
            if (message.CurrentModelType != typeof(FeeRuleSection)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<FeeRuleSection> predicate = null, Expression<Func<FeeRuleSection, object>> expression = null, bool isDesc = false)
        {
          FeeRuleSections = new ObservableCollection<FeeRuleSection>(_feeRuleSectionDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _feeRuleSectionDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<FeeRuleSection> predicate = null, Expression<Func<FeeRuleSection, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _feeRuleSectionDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              FeeRuleSections = new ObservableCollection<FeeRuleSection>(result);
                AllCount = _feeRuleSectionDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, FeeRuleSections.ToList(), "FeeRuleSections.xlsx", I18nManager.GetString("FeeRuleSectionInfo"));
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
                foreach (var item in FeeRuleSections)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in FeeRuleSections)
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
                    case "FeeRuleFlag":
                        _expression = u => u.FeeRuleFlag; break;
                    case "FeeRuleId":
                        _expression = u => u.FeeRuleId; break;
                    case "InWay":
                        _expression = u => u.InWay; break;
                    case "OutWay":
                        _expression = u => u.OutWay; break;
                    case "OvertimeFeeRule":
                        _expression = u => u.OvertimeFeeRule; break;
                    case "OvertimeType":
                        _expression = u => u.OvertimeType; break;
                    case "ParkingFeeRule":
                        _expression = u => u.ParkingFeeRule; break;
                    case "ParkingTime":
                        _expression = u => u.ParkingTime; break;
                    case "SectionName":
                        _expression = u => u.SectionName; break;
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
        private Task SearchFeeRuleSection()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<FeeRuleSection>(true);
                if (!String.IsNullOrEmpty(SearchSectionName)) 
                {
                  _predicate = _predicate.And(p => p.SectionName.Contains(SearchSectionName));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchSectionName = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<FeeRuleSectionActionWindowViewModel>();
            actionVM.SectionName = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddFeeRuleSection = true;
            actionVM.Title = I18nManager.GetString("CreateNewFeeRuleSection"); 
            var actionWindow = App.Views.CreateView<FeeRuleSectionActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateFeeRuleSection(FeeRuleSection feeRuleSection)
        {
            var actionVM = App.ServiceProvider.GetService<FeeRuleSectionActionWindowViewModel>();
            actionVM.SelectedFeeRuleSection = feeRuleSection;
            actionVM.SectionName = feeRuleSection.SectionName;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddFeeRuleSection = false;
            actionVM.Title = I18nManager.GetString("UpdateFeeRuleSectionInfo");
            var actionWindow = App.Views.CreateView<FeeRuleSectionActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteFeeRuleSection()
        {
            var selectedCount = FeeRuleSections.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteFeeRuleSectionPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteFeeRuleSection"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteFeeRuleSection"))
                .WithContent(I18nManager.GetString("SureWantToDeleteFeeRuleSection"))
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
            var selectedIds = FeeRuleSections.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _feeRuleSectionDao.DeleteRangeAsync<FeeRuleSection>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteFeeRuleSectionPrompt");
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
                                    CurrentModelType = typeof(FeeRuleSection),
                                    Title = I18nManager.GetString("DeleteFeeRuleSectionPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
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

