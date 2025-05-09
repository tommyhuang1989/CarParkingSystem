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
  public partial class LongTermRentalCarViewModel: LongTermRentalCarTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<LongTermRentalCar> _longTermRentalCars;
        [ObservableProperty] private LongTermRentalCar _selectedLongTermRentalCar;
        [ObservableProperty] private string _updateInfo;
        private LongTermRentalCarDao _longTermRentalCarDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<LongTermRentalCar> _predicate { get; set; }
        public Expression<Func<LongTermRentalCar, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Decimal _searchBalance;
        [ObservableProperty] private System.String _searchCarCode;
        [ObservableProperty] private System.Int32 _searchCard;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.Decimal _searchDeposit;
        [ObservableProperty] private System.String _searchParkSpace;
        [ObservableProperty] private System.Int32 _searchParkSpaceType;
        [ObservableProperty] private System.Int32 _searchRecStatus;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.String _searchSpaceName;
        [ObservableProperty] private System.String _searchUpdateDt;
        [ObservableProperty] private System.Int32 _searchUpdateUser;
        [ObservableProperty] private System.String _searchUsername;
        [ObservableProperty] private System.String _searchUserRemark;
        [ObservableProperty] private System.String _searchUserTel;
        [ObservableProperty] private System.String _searchValidEnd;
        [ObservableProperty] private System.String _searchValidFrom;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public LongTermRentalCarViewModel(LongTermRentalCarDao longTermRentalCarDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.LongTermRentalCar)
        {
            _longTermRentalCarDao = longTermRentalCarDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedLongTermRentalCarMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN, ReciveRefreshSummarySelectedLongTermRentalCar);
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
        private void ReciveRefreshSummarySelectedLongTermRentalCar(object recipient, SelectedLongTermRentalCarMessage message)
        {
          var selectedCount = LongTermRentalCars?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == LongTermRentalCars.Count())
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
            if (message.CurrentModelType != typeof(LongTermRentalCar)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<LongTermRentalCar> predicate = null, Expression<Func<LongTermRentalCar, object>> expression = null, bool isDesc = false)
        {
          LongTermRentalCars = new ObservableCollection<LongTermRentalCar>(_longTermRentalCarDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _longTermRentalCarDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<LongTermRentalCar> predicate = null, Expression<Func<LongTermRentalCar, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _longTermRentalCarDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              LongTermRentalCars = new ObservableCollection<LongTermRentalCar>(result);
                AllCount = _longTermRentalCarDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, LongTermRentalCars.ToList(), "LongTermRentalCars.xlsx", I18nManager.GetString("LongTermRentalCarInfo"));
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
                foreach (var item in LongTermRentalCars)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in LongTermRentalCars)
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
                    case "Balance":
                        _expression = u => u.Balance; break;
                    case "CarCode":
                        _expression = u => u.CarCode; break;
                    case "Card":
                        _expression = u => u.Card; break;
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "Deposit":
                        _expression = u => u.Deposit; break;
                    case "ParkSpace":
                        _expression = u => u.ParkSpace; break;
                    case "ParkSpaceType":
                        _expression = u => u.ParkSpaceType; break;
                    case "RecStatus":
                        _expression = u => u.RecStatus; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "SpaceName":
                        _expression = u => u.SpaceName; break;
                    case "UpdateDt":
                        _expression = u => u.UpdateDt; break;
                    case "UpdateUser":
                        _expression = u => u.UpdateUser; break;
                    case "Username":
                        _expression = u => u.Username; break;
                    case "UserRemark":
                        _expression = u => u.UserRemark; break;
                    case "UserTel":
                        _expression = u => u.UserTel; break;
                    case "ValidEnd":
                        _expression = u => u.ValidEnd; break;
                    case "ValidFrom":
                        _expression = u => u.ValidFrom; break;
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
        private Task SearchLongTermRentalCar()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<LongTermRentalCar>(true);
                if (!String.IsNullOrEmpty(SearchCarCode)) 
                {
                  _predicate = _predicate.And(p => p.CarCode.Contains(SearchCarCode));
                }
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchParkSpace)) 
                {
                  _predicate = _predicate.And(p => p.ParkSpace.Contains(SearchParkSpace));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchSpaceName)) 
                {
                  _predicate = _predicate.And(p => p.SpaceName.Contains(SearchSpaceName));
                }
                if (!String.IsNullOrEmpty(SearchUpdateDt)) 
                {
                  _predicate = _predicate.And(p => p.UpdateDt.Contains(SearchUpdateDt));
                }
                if (!String.IsNullOrEmpty(SearchUsername)) 
                {
                  _predicate = _predicate.And(p => p.Username.Contains(SearchUsername));
                }
                if (!String.IsNullOrEmpty(SearchUserRemark)) 
                {
                  _predicate = _predicate.And(p => p.UserRemark.Contains(SearchUserRemark));
                }
                if (!String.IsNullOrEmpty(SearchUserTel)) 
                {
                  _predicate = _predicate.And(p => p.UserTel.Contains(SearchUserTel));
                }
                if (!String.IsNullOrEmpty(SearchValidEnd)) 
                {
                  _predicate = _predicate.And(p => p.ValidEnd.Contains(SearchValidEnd));
                }
                if (!String.IsNullOrEmpty(SearchValidFrom)) 
                {
                  _predicate = _predicate.And(p => p.ValidFrom.Contains(SearchValidFrom));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCarCode = string.Empty;
            SearchCarNo = string.Empty;
            SearchParkSpace = string.Empty;
            SearchRemark = string.Empty;
            SearchSpaceName = string.Empty;
            SearchUpdateDt = string.Empty;
            SearchUsername = string.Empty;
            SearchUserRemark = string.Empty;
            SearchUserTel = string.Empty;
            SearchValidEnd = string.Empty;
            SearchValidFrom = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<LongTermRentalCarActionWindowViewModel>();
            actionVM.CarCode = string.Empty;
            actionVM.CarNo = string.Empty;
            actionVM.ParkSpace = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.SpaceName = string.Empty;
            actionVM.UpdateDt = string.Empty;
            actionVM.Username = string.Empty;
            actionVM.UserRemark = string.Empty;
            actionVM.UserTel = string.Empty;
            actionVM.ValidEnd = string.Empty;
            actionVM.ValidFrom = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddLongTermRentalCar = true;
            actionVM.Title = I18nManager.GetString("CreateNewLongTermRentalCar"); 
            var actionWindow = App.Views.CreateView<LongTermRentalCarActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateLongTermRentalCar(LongTermRentalCar longTermRentalCar)
        {
            var actionVM = App.ServiceProvider.GetService<LongTermRentalCarActionWindowViewModel>();
            actionVM.SelectedLongTermRentalCar = longTermRentalCar;
            actionVM.CarCode = longTermRentalCar.CarCode;
            actionVM.CarNo = longTermRentalCar.CarNo;
            actionVM.ParkSpace = longTermRentalCar.ParkSpace;
            actionVM.Remark = longTermRentalCar.Remark;
            actionVM.SpaceName = longTermRentalCar.SpaceName;
            actionVM.UpdateDt = longTermRentalCar.UpdateDt;
            actionVM.Username = longTermRentalCar.Username;
            actionVM.UserRemark = longTermRentalCar.UserRemark;
            actionVM.UserTel = longTermRentalCar.UserTel;
            actionVM.ValidEnd = longTermRentalCar.ValidEnd;
            actionVM.ValidFrom = longTermRentalCar.ValidFrom;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddLongTermRentalCar = false;
            actionVM.Title = I18nManager.GetString("UpdateLongTermRentalCarInfo");
            var actionWindow = App.Views.CreateView<LongTermRentalCarActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteLongTermRentalCar()
        {
            var selectedCount = LongTermRentalCars.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteLongTermRentalCarPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteLongTermRentalCar"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteLongTermRentalCar"))
                .WithContent(I18nManager.GetString("SureWantToDeleteLongTermRentalCar"))
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
            var selectedIds = LongTermRentalCars.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _longTermRentalCarDao.DeleteRangeAsync<LongTermRentalCar>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteLongTermRentalCarPrompt");
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
                                    CurrentModelType = typeof(LongTermRentalCar),
                                    Title = I18nManager.GetString("DeleteLongTermRentalCarPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedLongTermRentalCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_LONGTERMRENTALCAR_TOKEN);
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

