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
namespace CarParkingSystem.ViewModels
{
  public partial class RoleViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<Role> _roles;
        [ObservableProperty] private Role _selectedRole;
        [ObservableProperty] private string _updateInfo;
        private RoleDao _roleDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<Role> _predicate { get; set; }
        public Expression<Func<Role, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchRoleId;
        [ObservableProperty] private System.String _searchRoleName;
        [ObservableProperty] private System.String _searchRoleRemark;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public RoleViewModel(RoleDao roleDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base("RoleManagement", MaterialIconKind.Abacus, pid: 7, id: 37, index: 37)
        {
            _roleDao = roleDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedRoleMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN, ReciveRefreshSummarySelectedRole);
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
        private void ReciveRefreshSummarySelectedRole(object recipient, SelectedRoleMessage message)
        {
          var selectedCount = Roles?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == Roles.Count())
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
            if (message.CurrentModelType != typeof(Role)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<Role> predicate = null, Expression<Func<Role, object>> expression = null, bool isDesc = false)
        {
          Roles = new ObservableCollection<Role>(_roleDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _roleDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<Role> predicate = null, Expression<Func<Role, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _roleDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              Roles = new ObservableCollection<Role>(result);
                AllCount = _roleDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, Roles.ToList(), "Roles.xlsx", I18nManager.GetString("RoleInfo"));
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
                foreach (var item in Roles)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in Roles)
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
                    case "RoleId":
                        _expression = u => u.RoleId; break;
                    case "RoleName":
                        _expression = u => u.RoleName; break;
                    case "RoleRemark":
                        _expression = u => u.RoleRemark; break;
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
        private Task SearchRole()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<Role>(true);
                if (!String.IsNullOrEmpty(SearchRoleName)) 
                {
                  _predicate = _predicate.And(p => p.RoleName.Contains(SearchRoleName));
                }
                if (!String.IsNullOrEmpty(SearchRoleRemark)) 
                {
                  _predicate = _predicate.And(p => p.RoleRemark.Contains(SearchRoleRemark));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchRoleName = string.Empty;
            SearchRoleRemark = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<RoleActionWindowViewModel>();
            actionVM.RoleName = string.Empty;
            actionVM.RoleRemark = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddRole = true;
            actionVM.Title = I18nManager.GetString("CreateNewRole"); 
            var actionWindow = App.Views.CreateView<RoleActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateRole(Role role)
        {
            var actionVM = App.ServiceProvider.GetService<RoleActionWindowViewModel>();
            actionVM.SelectedRole = role;
            actionVM.RoleName = role.RoleName;
            actionVM.RoleRemark = role.RoleRemark;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddRole = false;
            actionVM.Title = I18nManager.GetString("UpdateRoleInfo");
            var actionWindow = App.Views.CreateView<RoleActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteRole()
        {
            var selectedCount = Roles.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteRolePrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteRole"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteRole"))
                .WithContent(I18nManager.GetString("SureWantToDeleteRole"))
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
            var selectedIds = Roles.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _roleDao.DeleteRangeAsync<Role>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteRolePrompt");
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
                                    CurrentModelType = typeof(Role),
                                    Title = I18nManager.GetString("DeleteRolePrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
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

