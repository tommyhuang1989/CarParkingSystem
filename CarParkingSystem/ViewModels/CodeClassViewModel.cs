using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Services;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.Core;
using LinqKit;
using Material.Icons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.ColorTheme;
using SukiUI.Content;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace CarParkingSystem.ViewModels
{
  public partial class CodeClassViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<CodeClass> _codeClasses;
        [ObservableProperty] private CodeClass _selectedCodeClass;
        [ObservableProperty] private string _updateInfo;
        private CodeClassDao _codeClassDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<CodeClass> _predicate { get; set; }
        public Expression<Func<CodeClass, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchId;
        [ObservableProperty] private System.String _searchProjectName;
        [ObservableProperty] private System.String _searchTableName;
        [ObservableProperty] private System.String _searchClassName;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public CodeClassViewModel(CodeClassDao codeClassDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.CodeGeneration, MaterialIconKind.Grade, pid: 7, id: 39, index: 39)
        {
          _codeClassDao = codeClassDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedCodeClassMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN, ReciveRefreshSummarySelectedCodeClass);
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
        private void ReciveRefreshSummarySelectedCodeClass(object recipient, SelectedCodeClassMessage message)
        {
          var selectedCount = CodeClasses?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == CodeClasses.Count())
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
            if (message.CurrentModelType != typeof(CodeClass)) return;

            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<CodeClass> predicate = null, Expression<Func<CodeClass, object>> expression = null, bool isDesc = false)
        {
          CodeClasses = new ObservableCollection<CodeClass>(_codeClassDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _codeClassDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<CodeClass> predicate = null, Expression<Func<CodeClass, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _codeClassDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              CodeClasses = new ObservableCollection<CodeClass>(result);
                AllCount = _codeClassDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, CodeClasses.ToList(), "CodeClasses.xlsx", I18nManager.GetString("CodeClassInfo"));
                ExcelService.HighlightFileInExplorer(filePath);
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
                foreach (var item in CodeClasses)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in CodeClasses)
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
                    case "ProjectName":
                        _expression = u => u.ProjectName; break;
                    case "TableName":
                        _expression = u => u.TableName; break;
                    case "ClassName":
                        _expression = u => u.ClassName; break;
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
        private Task SearchCodeClass()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<CodeClass>(true);
                if (!String.IsNullOrEmpty(SearchProjectName)) 
                {
                  _predicate = _predicate.And(p => p.ProjectName.Contains(SearchProjectName));
                }
                if (!String.IsNullOrEmpty(SearchTableName)) 
                {
                  _predicate = _predicate.And(p => p.TableName.Contains(SearchTableName));
                }
                if (!String.IsNullOrEmpty(SearchClassName)) 
                {
                  _predicate = _predicate.And(p => p.ClassName.Contains(SearchClassName));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchProjectName = string.Empty;
            SearchTableName = string.Empty;
            SearchClassName = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private Task SelectionChanged(int pageSize)
        {
            return Task.Run(async () =>
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

                WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<CodeClassActionWindowViewModel>();
            //actionVM.ProjectName = string.Empty;
            actionVM.TableName = string.Empty;
            actionVM.ClassName = string.Empty;
            actionVM.Fields = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();//20250329，add
            actionVM.IsAddCodeClass = true;
            actionVM.Title = I18nManager.GetString("CreateNewCodeClass"); 
            var actionWindow = App.Views.CreateView<CodeClassActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateCodeClass(CodeClass codeClass)
        {
            var actionVM = App.ServiceProvider.GetService<CodeClassActionWindowViewModel>();
            actionVM.SelectedCodeClass = codeClass;
            actionVM.ProjectName = codeClass.ProjectName;
            actionVM.TableName = codeClass.TableName;
            actionVM.ClassName = codeClass.ClassName;
            actionVM.UpdateInfo = string.Empty;
            actionVM.IsAddCodeClass = false;
            actionVM.ClearVertifyErrors();//20250329，add
            actionVM.Title = I18nManager.GetString("UpdateCodeClassInfo");
            var actionWindow = App.Views.CreateView<CodeClassActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void Configure(CodeClass codeClass)
        {
            //var actionVM = App.ServiceProvider.GetService<CodeFieldViewModel>();
            //actionVM.SelectedCodeClass = codeClass;
            //actionVM.ProjectName = codeClass.ProjectName;
            //actionVM.TableName = codeClass.TableName;
            //actionVM.ClassName = codeClass.ClassName;
            //actionVM.UpdateInfo = string.Empty;
            //actionVM.IsAddCodeClass = false;
            //actionVM.Title = I18nManager.GetString("UpdateCodeClassInfo");
            //var actionWindow = App.Views.CreateView<CodeClassActionWindowViewModel>(App.ServiceProvider) as Window;
            //var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            //if (owner != null)
            //{
            //    actionWindow?.ShowDialog(owner);
            //}
            //else
            //{
            //    actionWindow?.Show();
            //}

            WeakReferenceMessenger.Default.Send<ConfigureCodeFieldMessage, string>(
                new ConfigureCodeFieldMessage
                {
                    Cid = codeClass.Id,
                }, TokenManage.SHOW_CODE_FIELD_TOKEN);
        }

        [RelayCommand]
        private void DeleteCodeClass()
        {
            var selectedCount = CodeClasses.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
               .WithTitle(I18nManager.GetString("DeleteCodeClassPrompt"))
               .WithContent(I18nManager.GetString("SelectDeleteCodeClass"))
               .Dismiss().ByClickingBackground()
               .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
               .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
               .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCodeClass"))
                .WithContent(I18nManager.GetString("SureWantToDeleteCodeClass"))
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
            var selectedIds = CodeClasses.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _codeClassDao.DeleteRangeAsync<CodeClass>(new List<int>(selectedIds));

            if (result > 0)
            {//20250401, 删除成功也用 dialog 提示
                dialog.Icon = Icons.Check;
                dialog.IconColor = NotificationColor.SuccessIconForeground;
                dialog.Title = I18nManager.GetString("DeleteCodeClassPrompt");
                dialog.Content = I18nManager.GetString("DeleteSuccessfully");
                if (dialog.ActionButtons.Count > 1)
                {
                    (dialog.ActionButtons[0] as Button).IsVisible = false;//隐藏第一个按钮

                    var button = dialog.ActionButtons[1] as Button;//将取消按钮赋值为“确定”
                    button.Content = I18nManager.GetString("Submit");
                }

                Dispatcher.UIThread.Invoke(() =>
                {
                    WeakReferenceMessenger.Default.Send(
                                new ToastMessage
                                {
                                    CurrentModelType = typeof(CodeClass),
                                    Title = I18nManager.GetString("DeleteCodeClassPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                    WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
                });
            }
        }

        [RelayCommand]
        private void Generate()
        {
            var selectedCount = CodeClasses.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
               .WithTitle(I18nManager.GetString("GenerateCodeClassPrompt"))
               .WithContent(I18nManager.GetString("SelectGenerateCodeClass"))
               .Dismiss().ByClickingBackground()
               .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
               .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
               .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("GenerateCodeClass"))
                .WithContent(I18nManager.GetString("SureWantToGenerateCodeClass"))
                .WithActionButton(I18nManager.GetString("Sure"), _ => SureGenerateAsync(), true)
                .WithActionButton(I18nManager.GetString("Cancel"), _ => CancelGenerate(), true)
                .OfType(Avalonia.Controls.Notifications.NotificationType.Warning)
                .TryShow();
            }
        }

        private void CancelGenerate()
        {
            return;
        }

        private async Task<Task> SureGenerateAsync()
        {
            //1.开始生成（显示生成过程--进度条或者其他）
            //2.生成结果提示（成功还是失败）

            return Task.Run(() =>
            {
                StringBuilder classStr = new StringBuilder();
                Dictionary<string, bool> generateResult = new Dictionary<string, bool>();
                foreach (var item in CodeClasses.Where(x => x.IsSelected))
                {
                    //20250331,这里到时需要从数据库读取出来
                    if (item != null)
                    {
                        var entry = _appDbContext.Entry(item).Collection(a => a.CodeFields);
                        if (!entry.IsLoaded)
                        {
                            entry.Load();  // 仅当未加载时执行查询
                        }
                    }
                    bool result = GenerateCodeHelper.Run(item);
                    string message = result ? item.TableName +" "+I18nManager.GetString("GenerateSuccessfully") : item.TableName + " " + I18nManager.GetString("GenerateFailed"); 
                    classStr.AppendLine(message);

                    generateResult.Add(item.ClassName, result);

                    if (result)
                    {
                        WeakReferenceMessenger.Default.Send("Add new second menu", TokenManage.MAIN_WINDOW_REFRESH_MENU_TOKEN);

                        string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                        string filename = item.ProjectName + "\\Sql\\Generate\\" + item.TableName + ".sql";//20250403,无须再加 s
                        string filePath = System.IO.Path.Combine(basePath, filename);
                        if (System.IO.File.Exists(filePath))
                        {
                            var sql = System.IO.File.ReadAllText(filePath);

                            _appDbContext.Database.ExecuteSqlRaw(sql);
                        }
                    }
                }
                //classStr.AppendLine(I18nManager.GetString("RestartIsRequiredToTakeEffect"));
                string generateTitle = string.Empty;
                Avalonia.Controls.Notifications.NotificationType resultIcon = Avalonia.Controls.Notifications.NotificationType.Success;
                if (generateResult.Values.All(x => x == true))
                {
                    generateTitle = I18nManager.GetString("AllGeneratedSuccessfully");
                    classStr.AppendLine(I18nManager.GetString("YouNeedToRestartTheProgramToTakeEffect"));
                }
                else
                {
                    resultIcon = Avalonia.Controls.Notifications.NotificationType.Error;
                    generateTitle = I18nManager.GetString("SomeBuildsFailed");
                    classStr.AppendLine(I18nManager.GetString("PleaseCheckTheCodeAndViewTheLog"));
                }

                    //var selectedIds = CodeClasses.Where(u => u.IsSelected).Select(u => u.Id);
                    //int result = await _codeClassDao.DeleteRangeAsync<CodeClass>(new List<int>(selectedIds));

                    //if (result > 0)
                    //{
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        _dialogManager.CreateDialog()
                       .WithTitle(generateTitle)
                       .WithContent(classStr.ToString())
                       .Dismiss().ByClickingBackground()
                       .OfType(resultIcon)
                       .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                       .TryShow();

                        WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
                    });
            });
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

