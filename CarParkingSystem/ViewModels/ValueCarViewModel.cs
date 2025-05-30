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
  public partial class ValueCarViewModel: ValueCarTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ValueCar> _valueCars;
        [ObservableProperty] private ValueCar _selectedValueCar;
        [ObservableProperty] private string _updateInfo;
        private ValueCarDao _valueCarDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ValueCar> _predicate { get; set; }
        public Expression<Func<ValueCar, object>> _expression { get; set; }
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
      public ValueCarViewModel(ValueCarDao valueCarDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.RechargeCar)
        {
            _valueCarDao = valueCarDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedValueCarMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN, ReciveRefreshSummarySelectedValueCar);
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
        private void ReciveRefreshSummarySelectedValueCar(object recipient, SelectedValueCarMessage message)
        {
          var selectedCount = ValueCars?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ValueCars.Count())
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
            if (message.CurrentModelType != typeof(ValueCar)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<ValueCar> predicate = null, Expression<Func<ValueCar, object>> expression = null, bool isDesc = false)
        {
          ValueCars = new ObservableCollection<ValueCar>(_valueCarDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _valueCarDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ValueCar> predicate = null, Expression<Func<ValueCar, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _valueCarDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              ValueCars = new ObservableCollection<ValueCar>(result);
                AllCount = _valueCarDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ValueCars.ToList(), "ValueCars.xlsx", I18nManager.GetString("ValueCarInfo"));
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
                foreach (var item in ValueCars)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ValueCars)
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
        private Task SearchValueCar()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ValueCar>(true);
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
                WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
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
            WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<ValueCarActionWindowViewModel>();
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
            actionVM.IsAddValueCar = true;
            actionVM.Title = I18nManager.GetString("CreateNewValueCar"); 
            var actionWindow = App.Views.CreateView<ValueCarActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateValueCar(ValueCar valueCar)
        {
            var actionVM = App.ServiceProvider.GetService<ValueCarActionWindowViewModel>();
            actionVM.SelectedValueCar = valueCar;
            actionVM.CarCode = valueCar.CarCode;
            actionVM.CarNo = valueCar.CarNo;
            actionVM.ParkSpace = valueCar.ParkSpace;
            actionVM.Remark = valueCar.Remark;
            actionVM.SpaceName = valueCar.SpaceName;
            actionVM.UpdateDt = valueCar.UpdateDt;
            actionVM.Username = valueCar.Username;
            actionVM.UserRemark = valueCar.UserRemark;
            actionVM.UserTel = valueCar.UserTel;
            actionVM.ValidEnd = valueCar.ValidEnd;
            actionVM.ValidFrom = valueCar.ValidFrom;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddValueCar = false;
            actionVM.Title = I18nManager.GetString("UpdateValueCarInfo");
            var actionWindow = App.Views.CreateView<ValueCarActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteValueCar()
        {
            var selectedCount = ValueCars.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteValueCarPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteValueCar"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteValueCar"))
                .WithContent(I18nManager.GetString("SureWantToDeleteValueCar"))
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
            var selectedIds = ValueCars.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _valueCarDao.DeleteRangeAsync<ValueCar>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteValueCarPrompt");
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
                                    CurrentModelType = typeof(ValueCar),
                                    Title = I18nManager.GetString("DeleteValueCarPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
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

