using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Controls;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
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
using SukiUI.ColorTheme;
using SukiUI.Content;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为用户管理界面提供数据的 ViewModel
    /// </summary>
    public partial class UserViewModel : DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private ObservableCollection<User> _users;
        [ObservableProperty] private User _selectedUser;
        [ObservableProperty] private string _updateInfo;

        private UserDao _userDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<User> _predicate { get; set; }
        public Expression<Func<User, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }

        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private string _searchUsername;
        [ObservableProperty] private string _searchEmail;
        [ObservableProperty] private string _searchPhone;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
        [ObservableProperty] private bool _isShowSearchBar = false;
        //[ObservableProperty] private bool _isActive = false;

        //UserActionWindowViewModel userActionVM, 
        public UserViewModel(UserDao userDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.UserSettings, MaterialIconKind.User, pid: 2, id: 12, index: 12, showEvenOneFoor: true)
        {
            _userDao = userDao;

            RefreshData();

            _appDbContext = appDbContext;
            _dialogManager = dialogManager;

            // add, update, delete 时刷新数据
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
            WeakReferenceMessenger.Default.Register<SelectedUserMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN, ReciveRefreshSummarySelectedUser);
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
        private void ReciveRefreshSummarySelectedUser(object recipient, SelectedUserMessage message)
        {
            var selectedCount = Users?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == Users.Count())
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
            if (message.CurrentModelType != typeof(User)) return;

            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<User> predicate = null, Expression<Func<User, object>> expression = null, bool isDesc = false)
        {
            Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));//改为分页查询

            AllCount = _userDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<User> predicate = null, Expression<Func<User, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
                var result = await _userDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
                Users = new ObservableCollection<User>(result);

                AllCount = _userDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }


        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, Users.ToList(), "Users.xlsx", I18nManager.GetString("UserInfo"));

                ExcelService.HighlightFileInExplorer(filePath);

                //if (filePath == null || !File.Exists(filePath)) { 
                //    WeakReferenceMessenger.Default.Send(
                //                     new ToastMessage
                //                     {
                //                         Title = "导出用户信息提示",
                //                         Content = "导出",
                //                         NotifyType = Avalonia.Controls.Notifications.NotificationType.Error
                //                     }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                //}

                //if (!string.IsNullOrEmpty(filePath))
                //{
                //    var directoryPath = Path.GetDirectoryName(filePath);
                //    var topLevel = TopLevel.GetTopLevel(window);
                //    var launcher = topLevel.Launcher;
                //    await launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(directoryPath));
                //}
            });
        }

        [RelayCommand]
        private void SelectedAll()
        {
            if (IsSelectedAll == null)
            {
                IsSelectedAll = false;
                //return;
            }

            if (IsSelectedAll == false) {
                foreach (var item in Users)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in Users)
                {
                    item.IsSelected = true;
                }
            }
            
        }

        //[RelayCommand]
        //private void Active()
        //{
        //    IsActive = !IsActive;
        //}

        [RelayCommand]
        private Task Sorting(object obj)
        {
            List<object> list = (List<object>)obj;
            var fieldName = list[0] as string;
            var isDesc = (bool)list[1];

            return Task.Run(async () =>
            {
                //Expression<Func<User, object>> expression = u => u.Id;
                _expression = u => u.Id;
                _isOrderByDesc = isDesc;

                switch (fieldName)
                {
                    case "Username":
                        _expression = u => u.Username; break;
                    case "Password":
                        _expression = u => u.Password;break;
                    case "Email":
                        _expression = u => u.Email; break;
                    case "Phone":
                        _expression = u => u.Phone; break;
                    case "CreatedAt":
                        _expression = u => u.CreatedAt; break;
                    case "UpdatedAt":
                        _expression = u => u.UpdatedAt; break;
                    case "Status":
                        _expression = u => u.Status; break;
                    case "LastLoginTime":
                        _expression = u => u.LastLoginTime; break;
                    default:
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
        private Task ShowSearchBar()
        {
            return Task.Run(async () =>
            {
                IsShowSearchBar = !IsShowSearchBar;
            });
        }

        [RelayCommand]
        private Task SearchUser()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<User>(true);
                if (!String.IsNullOrEmpty(SearchUsername)) 
                {
                    _predicate = _predicate.And(p => p.Username.Contains(SearchUsername));
                }
                if (!String.IsNullOrEmpty(SearchEmail))
                {
                    _predicate = _predicate.And(p => p.Email.Contains(SearchEmail));
                }
                if (!String.IsNullOrEmpty(SearchPhone))
                {
                    _predicate = _predicate.And(p => p.Phone.Contains(SearchPhone));
                }
                if (!String.IsNullOrEmpty(SearchStartDateTime))
                {
                    var endDate = DateTime.Parse(SearchStartDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                    _predicate = _predicate.And(p => p.CreatedAt.CompareTo(endDate) > 0);
                }
                if (!String.IsNullOrEmpty(SearchEndDateTime))
                {
                    var endDate = DateTime.Parse(SearchEndDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                    _predicate = _predicate.And(p => p.CreatedAt.CompareTo(endDate) < 0);
                }

                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                //IsBusy = true;
                //await Task.Delay(3000);
                //IsBusy = false;


                WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchUsername = string.Empty;
            SearchEmail = string.Empty;
            SearchPhone = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;

            WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private Task SelectionChanged(int pageSize)
        {
            return Task.Run(async ()=>
            {
                PageSize = pageSize;
                //switch (obj)
                //{
                //    case 0:
                //        PageSize = 15; break;
                //    case 1:
                //        PageSize = 25; break;
                //    case 2:
                //        PageSize = 50; break;
                //    case 3:
                //        PageSize = 100; break;
                //    //case 4:
                //    //    PageSize = 2; break;
                //    //case 5:
                //    //    PageSize = 5; break;
                //    default:
                //        break;
                //}

                CurrentPageIndex = 1;//更改页面数量应该从第一页显示

                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);

                WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
            });
        }

        [RelayCommand]
        private Task ButtonClicked()
        {
            if (IsBusy)
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                IsBusy = true;
                await Task.Delay(3000);
                IsBusy = false;
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<UserActionWindowViewModel>();
            actionVM.Username = string.Empty;
            actionVM.Password = string.Empty;
            actionVM.Email = string.Empty;
            actionVM.Phone = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddUser = true;
            actionVM.Title = I18nManager.GetString("CreateNewUser"); 
            var actionWindow = App.Views.CreateView<UserActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateUser(User user)
        {
            var userActionVM = App.ServiceProvider.GetService<UserActionWindowViewModel>();
            userActionVM.SelectedUser = user;
            userActionVM.Username = user.Username;
            userActionVM.Password = user.Password;
            userActionVM.Email = user.Email;
            userActionVM.Phone = user.Phone;
            userActionVM.UpdateInfo = string.Empty;
            userActionVM.ClearVertifyErrors();
            userActionVM.IsAddUser = false;
            userActionVM.Title = I18nManager.GetString("UpdateUserInfo");
            //var updateWindow = App.ServiceProvider.GetService<UserActionWindow>();
            var userActionWindow = App.Views.CreateView<UserActionWindowViewModel>(App.ServiceProvider) as Window;
            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                userActionWindow?.ShowDialog(owner);
            }
            else
            {
                userActionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }


        [RelayCommand]
        private void DeleteUser()
        {
            var selectedCount = Users.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
               .WithTitle(I18nManager.GetString("DeleteUserPrompt"))
               .WithContent(I18nManager.GetString("SelectDeleteUser"))
               .Dismiss().ByClickingBackground()
               .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
               .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
               .TryShow();
            }
            else
            {
                var builder = _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteUser"))
                .WithContent(I18nManager.GetString("SureWantToDeleteUser"))
                .WithActionButton(I18nManager.GetString("Sure"), dialog => SureDeleteAsync(dialog), false)//true
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
            var selectedIds = Users.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _userDao.DeleteRangeAsync<User>(new List<int>(selectedIds));

            if (result > 0)
            {

                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteUserPrompt");
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
                                    CurrentModelType = typeof(User),
                                    Title = I18nManager.GetString("DeleteUserPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                    WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
                });
            }
        }

        #region 分页

        [RelayCommand]
        private Task FirstPage()
        {
            return Task.Run(async () => {
                CurrentPageIndex = 1;
                //Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize));
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
                    //Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize));
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
                    //Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize));
                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                }
            });
        }

        [RelayCommand]
        private Task LastPage()
        {
            return Task.Run(async () => {
                CurrentPageIndex = PageCount;
                //Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize));
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
                    //Users = new ObservableCollection<User>(_userDao.GetAllPaged(CurrentPageIndex, PageSize));
                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                }
            });
        }
        #endregion

    }
}
