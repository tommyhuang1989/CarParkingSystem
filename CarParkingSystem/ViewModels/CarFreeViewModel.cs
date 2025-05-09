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
  public partial class CarFreeViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<CarFree> _carFrees;
        [ObservableProperty] private CarFree _selectedCarFree;
        [ObservableProperty] private string _updateInfo;
        private CarFreeDao _carFreeDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<CarFree> _predicate { get; set; }
        public Expression<Func<CarFree, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.String _searchEndTime;
        [ObservableProperty] private System.String _searchFreeDesc;
        [ObservableProperty] private System.String _searchFromTime;
        [ObservableProperty] private System.Int32 _searchRecStatus;
        [ObservableProperty] private System.String _searchUpdateDt;
        [ObservableProperty] private System.Int32 _searchUpdateUser;
        [ObservableProperty] private System.String _searchUserAddr;
        [ObservableProperty] private System.String _searchUsername;
        [ObservableProperty] private System.String _searchUserPhone;
        [ObservableProperty] private System.Int32 _searchUserType;
        [ObservableProperty] private System.String _searchWxOpenId;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public CarFreeViewModel(CarFreeDao carFreeDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.FreeCar, MaterialIconKind.Abacus, pid: 4, id: 23, index: 23)
        {
            _carFreeDao = carFreeDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedCarFreeMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN, ReciveRefreshSummarySelectedCarFree);
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
        private void ReciveRefreshSummarySelectedCarFree(object recipient, SelectedCarFreeMessage message)
        {
          var selectedCount = CarFrees?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == CarFrees.Count())
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
            if (message.CurrentModelType != typeof(CarFree)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<CarFree> predicate = null, Expression<Func<CarFree, object>> expression = null, bool isDesc = false)
        {
          CarFrees = new ObservableCollection<CarFree>(_carFreeDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _carFreeDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<CarFree> predicate = null, Expression<Func<CarFree, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _carFreeDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              CarFrees = new ObservableCollection<CarFree>(result);
                AllCount = _carFreeDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, CarFrees.ToList(), "CarFrees.xlsx", I18nManager.GetString("CarFreeInfo"));
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
                foreach (var item in CarFrees)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in CarFrees)
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
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "EndTime":
                        _expression = u => u.EndTime; break;
                    case "FreeDesc":
                        _expression = u => u.FreeDesc; break;
                    case "FromTime":
                        _expression = u => u.FromTime; break;
                    case "RecStatus":
                        _expression = u => u.RecStatus; break;
                    case "UpdateDt":
                        _expression = u => u.UpdateDt; break;
                    case "UpdateUser":
                        _expression = u => u.UpdateUser; break;
                    case "UserAddr":
                        _expression = u => u.UserAddr; break;
                    case "Username":
                        _expression = u => u.Username; break;
                    case "UserPhone":
                        _expression = u => u.UserPhone; break;
                    case "UserType":
                        _expression = u => u.UserType; break;
                    case "WxOpenId":
                        _expression = u => u.WxOpenId; break;
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
        private Task SearchCarFree()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<CarFree>(true);
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchEndTime)) 
                {
                  _predicate = _predicate.And(p => p.EndTime.Contains(SearchEndTime));
                }
                if (!String.IsNullOrEmpty(SearchFreeDesc)) 
                {
                  _predicate = _predicate.And(p => p.FreeDesc.Contains(SearchFreeDesc));
                }
                if (!String.IsNullOrEmpty(SearchFromTime)) 
                {
                  _predicate = _predicate.And(p => p.FromTime.Contains(SearchFromTime));
                }
                if (!String.IsNullOrEmpty(SearchUpdateDt)) 
                {
                  _predicate = _predicate.And(p => p.UpdateDt.Contains(SearchUpdateDt));
                }
                if (!String.IsNullOrEmpty(SearchUserAddr)) 
                {
                  _predicate = _predicate.And(p => p.UserAddr.Contains(SearchUserAddr));
                }
                if (!String.IsNullOrEmpty(SearchUsername)) 
                {
                  _predicate = _predicate.And(p => p.Username.Contains(SearchUsername));
                }
                if (!String.IsNullOrEmpty(SearchUserPhone)) 
                {
                  _predicate = _predicate.And(p => p.UserPhone.Contains(SearchUserPhone));
                }
                if (!String.IsNullOrEmpty(SearchWxOpenId)) 
                {
                  _predicate = _predicate.And(p => p.WxOpenId.Contains(SearchWxOpenId));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCarNo = string.Empty;
            SearchEndTime = string.Empty;
            SearchFreeDesc = string.Empty;
            SearchFromTime = string.Empty;
            SearchUpdateDt = string.Empty;
            SearchUserAddr = string.Empty;
            SearchUsername = string.Empty;
            SearchUserPhone = string.Empty;
            SearchWxOpenId = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<CarFreeActionWindowViewModel>();
            actionVM.CarNo = string.Empty;
            actionVM.EndTime = string.Empty;
            actionVM.FreeDesc = string.Empty;
            actionVM.FromTime = string.Empty;
            actionVM.UpdateDt = string.Empty;
            actionVM.UserAddr = string.Empty;
            actionVM.Username = string.Empty;
            actionVM.UserPhone = string.Empty;
            actionVM.WxOpenId = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarFree = true;
            actionVM.Title = I18nManager.GetString("CreateNewCarFree"); 
            var actionWindow = App.Views.CreateView<CarFreeActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateCarFree(CarFree carFree)
        {
            var actionVM = App.ServiceProvider.GetService<CarFreeActionWindowViewModel>();
            actionVM.SelectedCarFree = carFree;
            actionVM.CarNo = carFree.CarNo;
            actionVM.EndTime = carFree.EndTime;
            actionVM.FreeDesc = carFree.FreeDesc;
            actionVM.FromTime = carFree.FromTime;
            actionVM.UpdateDt = carFree.UpdateDt;
            actionVM.UserAddr = carFree.UserAddr;
            actionVM.Username = carFree.Username;
            actionVM.UserPhone = carFree.UserPhone;
            actionVM.WxOpenId = carFree.WxOpenId;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarFree = false;
            actionVM.Title = I18nManager.GetString("UpdateCarFreeInfo");
            var actionWindow = App.Views.CreateView<CarFreeActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteCarFree()
        {
            var selectedCount = CarFrees.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarFreePrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteCarFree"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarFree"))
                .WithContent(I18nManager.GetString("SureWantToDeleteCarFree"))
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
            var selectedIds = CarFrees.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _carFreeDao.DeleteRangeAsync<CarFree>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteCarFreePrompt");
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
                                    CurrentModelType = typeof(CarFree),
                                    Title = I18nManager.GetString("DeleteCarFreePrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
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

