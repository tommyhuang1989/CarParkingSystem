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
  public partial class CarVisitorViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<CarVisitor> _carVisitors;
        [ObservableProperty] private CarVisitor _selectedCarVisitor;
        [ObservableProperty] private string _updateInfo;
        private CarVisitorDao _carVisitorDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<CarVisitor> _predicate { get; set; }
        public Expression<Func<CarVisitor, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.String _searchCardNo;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.Int32 _searchClient;
        [ObservableProperty] private System.String _searchClientId;
        [ObservableProperty] private System.String _searchCreateDate;
        [ObservableProperty] private System.String _searchEndDate;
        [ObservableProperty] private System.String _searchOrderId;
        [ObservableProperty] private System.String _searchPhone;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.String _searchStartDate;
        [ObservableProperty] private System.String _searchTrueName;
        [ObservableProperty] private System.String _searchVisitorHouse;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public CarVisitorViewModel(CarVisitorDao carVisitorDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.CarVisitor, MaterialIconKind.Abacus, pid: 5, id: 31, index: 31)
        {
            _carVisitorDao = carVisitorDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedCarVisitorMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN, ReciveRefreshSummarySelectedCarVisitor);
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
        private void ReciveRefreshSummarySelectedCarVisitor(object recipient, SelectedCarVisitorMessage message)
        {
          var selectedCount = CarVisitors?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == CarVisitors.Count())
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
            if (message.CurrentModelType != typeof(CarVisitor)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<CarVisitor> predicate = null, Expression<Func<CarVisitor, object>> expression = null, bool isDesc = false)
        {
          CarVisitors = new ObservableCollection<CarVisitor>(_carVisitorDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _carVisitorDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<CarVisitor> predicate = null, Expression<Func<CarVisitor, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _carVisitorDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              CarVisitors = new ObservableCollection<CarVisitor>(result);
                AllCount = _carVisitorDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, CarVisitors.ToList(), "CarVisitors.xlsx", I18nManager.GetString("CarVisitorInfo"));
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
                foreach (var item in CarVisitors)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in CarVisitors)
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
                    case "CardNo":
                        _expression = u => u.CardNo; break;
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "Client":
                        _expression = u => u.Client; break;
                    case "ClientId":
                        _expression = u => u.ClientId; break;
                    case "CreateDate":
                        _expression = u => u.CreateDate; break;
                    case "EndDate":
                        _expression = u => u.EndDate; break;
                    case "OrderId":
                        _expression = u => u.OrderId; break;
                    case "Phone":
                        _expression = u => u.Phone; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "StartDate":
                        _expression = u => u.StartDate; break;
                    case "TrueName":
                        _expression = u => u.TrueName; break;
                    case "VisitorHouse":
                        _expression = u => u.VisitorHouse; break;
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
        private Task SearchCarVisitor()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<CarVisitor>(true);
                if (!String.IsNullOrEmpty(SearchCardNo)) 
                {
                  _predicate = _predicate.And(p => p.CardNo.Contains(SearchCardNo));
                }
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchClientId)) 
                {
                  _predicate = _predicate.And(p => p.ClientId.Contains(SearchClientId));
                }
                if (!String.IsNullOrEmpty(SearchCreateDate)) 
                {
                  _predicate = _predicate.And(p => p.CreateDate.Contains(SearchCreateDate));
                }
                if (!String.IsNullOrEmpty(SearchEndDate)) 
                {
                  _predicate = _predicate.And(p => p.EndDate.Contains(SearchEndDate));
                }
                if (!String.IsNullOrEmpty(SearchOrderId)) 
                {
                  _predicate = _predicate.And(p => p.OrderId.Contains(SearchOrderId));
                }
                if (!String.IsNullOrEmpty(SearchPhone)) 
                {
                  _predicate = _predicate.And(p => p.Phone.Contains(SearchPhone));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchStartDate)) 
                {
                  _predicate = _predicate.And(p => p.StartDate.Contains(SearchStartDate));
                }
                if (!String.IsNullOrEmpty(SearchTrueName)) 
                {
                  _predicate = _predicate.And(p => p.TrueName.Contains(SearchTrueName));
                }
                if (!String.IsNullOrEmpty(SearchVisitorHouse)) 
                {
                  _predicate = _predicate.And(p => p.VisitorHouse.Contains(SearchVisitorHouse));
                }

                if (!String.IsNullOrEmpty(SearchStartDateTime))
                {
                    var endDate = DateTime.Parse(SearchStartDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                    _predicate = _predicate.And(p => p.CreateDate.CompareTo(endDate) > 0);
                }
                if (!String.IsNullOrEmpty(SearchEndDateTime))
                {
                    var endDate = DateTime.Parse(SearchEndDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                    _predicate = _predicate.And(p => p.CreateDate.CompareTo(endDate) < 0);
                }

                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCardNo = string.Empty;
            SearchCarNo = string.Empty;
            SearchClientId = string.Empty;
            SearchCreateDate = string.Empty;
            SearchEndDate = string.Empty;
            SearchOrderId = string.Empty;
            SearchPhone = string.Empty;
            SearchRemark = string.Empty;
            SearchStartDate = string.Empty;
            SearchTrueName = string.Empty;
            SearchVisitorHouse = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<CarVisitorActionWindowViewModel>();
            actionVM.CardNo = string.Empty;
            actionVM.CarNo = string.Empty;
            actionVM.ClientId = string.Empty;
            actionVM.CreateDate = null;
            actionVM.EndDate = null;
            actionVM.OrderId = string.Empty;
            actionVM.Phone = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.StartDate = null;
            actionVM.TrueName = string.Empty;
            actionVM.VisitorHouse = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarVisitor = true;
            actionVM.Title = I18nManager.GetString("CreateNewCarVisitor"); 
            var actionWindow = App.Views.CreateView<CarVisitorActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateCarVisitor(CarVisitor carVisitor)
        {
            var actionVM = App.ServiceProvider.GetService<CarVisitorActionWindowViewModel>();
            actionVM.SelectedCarVisitor = carVisitor;
            actionVM.CardNo = carVisitor.CardNo;
            actionVM.CarNo = carVisitor.CarNo;
            actionVM.ClientId = carVisitor.ClientId;
            actionVM.CreateDate = carVisitor.CreateDate;
            actionVM.EndDate = carVisitor.EndDate;
            actionVM.OrderId = carVisitor.OrderId;
            actionVM.Phone = carVisitor.Phone;
            actionVM.Remark = carVisitor.Remark;
            actionVM.StartDate = carVisitor.StartDate;
            actionVM.TrueName = carVisitor.TrueName;
            actionVM.VisitorHouse = carVisitor.VisitorHouse;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarVisitor = false;
            actionVM.Title = I18nManager.GetString("UpdateCarVisitorInfo");
            var actionWindow = App.Views.CreateView<CarVisitorActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteCarVisitor()
        {
            var selectedCount = CarVisitors.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarVisitorPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteCarVisitor"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarVisitor"))
                .WithContent(I18nManager.GetString("SureWantToDeleteCarVisitor"))
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
            var selectedIds = CarVisitors.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _carVisitorDao.DeleteRangeAsync<CarVisitor>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteCarVisitorPrompt");
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
                                    CurrentModelType = typeof(CarVisitor),
                                    Title = I18nManager.GetString("DeleteCarVisitorPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
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

