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
    //: DemoPageBase
    public partial class CodeFieldViewModel : DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<CodeField> _codeFields;
        [ObservableProperty] private CodeField _selectedCodeField;
        [ObservableProperty] private string _updateInfo;
        private CodeFieldDao _codeFieldDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<CodeField> _predicate { get; set; }
        public Expression<Func<CodeField, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _cid;//20250328, tommy, add

        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchId;
        [ObservableProperty] private System.String _searchFieldName;
        [ObservableProperty] private System.String _searchFieldType;
        [ObservableProperty] private System.Int32 _searchFieldLength;
        [ObservableProperty] private System.String _searchFieldRemark;
        [ObservableProperty] private System.Boolean _searchIsMainKey;
        [ObservableProperty] private System.Boolean _searchIsAllowNull;
        [ObservableProperty] private System.Boolean _searchIsAutoIncrement;
        [ObservableProperty] private System.Boolean _searchIsUnique;
        [ObservableProperty] private System.String _searchDefaultValue;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
        public CodeFieldViewModel(CodeFieldDao codeFieldDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base("CodeFieldManagement", MaterialIconKind.Grade, pid: 7, id: 43, index: 43, showEvenOneFoor: false, isHiddenPage: true)
        {
            _codeFieldDao = codeFieldDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
            WeakReferenceMessenger.Default.Register<SelectedCodeFieldMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN, ReciveRefreshSummarySelectedCodeField);
        }

        // CodeFieldView belond to its parent CodeClass, so did not need show in mainPage
        //public CodeFieldViewModel()
        //{
        //    _codeFieldDao = App.ServiceProvider.GetService<CodeFieldDao>();
        //    _dialogManager = App.ServiceProvider.GetService<ISukiDialogManager>();
        //    RefreshData();
        //    //_appDbContext = appDbContext;
        //    //_dialogManager = dialogManager;
        //    WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
        //    WeakReferenceMessenger.Default.Register<SelectedCodeFieldMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN, ReciveRefreshSummarySelectedCodeField);
        //}

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
        private void ReciveRefreshSummarySelectedCodeField(object recipient, SelectedCodeFieldMessage message)
        {
          var selectedCount = CodeFields?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == CodeFields.Count())
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
            if (message.CurrentModelType != typeof(CodeField)) return;

            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        public void RefreshData(ExpressionStarter<CodeField> predicate = null, Expression<Func<CodeField, object>> expression = null, bool isDesc = false)
        {
            predicate = SetCodeClassId(predicate);
            CodeFields = new ObservableCollection<CodeField>(_codeFieldDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _codeFieldDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        public Task RefreshDataAsync(ExpressionStarter<CodeField> predicate = null, Expression<Func<CodeField, object>> expression = null, bool isDesc = false)
        {
            predicate = SetCodeClassId(predicate);
            return Task.Run(async () => {
              var result = await _codeFieldDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              CodeFields = new ObservableCollection<CodeField>(result);
                AllCount = _codeFieldDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }

        /// <summary>
        /// CodeField 需要指定 Cid，否则就会将所有的 CodeField 都查询出来了
        /// 
        /// eg:p => p.Cid.Equals(value(CarParkingSystem.ViewModels.CodeFieldViewModel).Cid)
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private ExpressionStarter<CodeField> SetCodeClassId(ExpressionStarter<CodeField> predicate)
        {
            if (predicate == null)
            {
                predicate = PredicateBuilder.New<CodeField>(true);
            }
            if (!predicate.ToString().Contains("p.Cid.Equals"))//避免重复添加条件
            {
                predicate = predicate.And(p => p.Cid.Equals(Cid));
            }
            return predicate;
        }

        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, CodeFields.ToList(), "CodeFields.xlsx", I18nManager.GetString("CodeFieldInfo"));
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
                foreach (var item in CodeFields)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in CodeFields)
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
                    case "FieldName":
                        _expression = u => u.FieldName; break;
                    case "FieldType":
                        _expression = u => u.FieldType; break;
                    case "FieldLength":
                        _expression = u => u.FieldLength; break;
                    case "FieldRemark":
                        _expression = u => u.FieldRemark; break;
                    case "IsMainKey":
                        _expression = u => u.IsMainKey; break;
                    case "IsAllowNull":
                        _expression = u => u.IsAllowNull; break;
                    case "IsAutoIncrement":
                        _expression = u => u.IsAutoIncrement; break;
                    case "IsUnique":
                        _expression = u => u.IsUnique; break;
                    case "DefaultValue":
                        _expression = u => u.DefaultValue; break;
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
        private Task SearchCodeField()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<CodeField>(true);
                if (!String.IsNullOrEmpty(SearchFieldName)) 
                {
                  _predicate = _predicate.And(p => p.FieldName.Contains(SearchFieldName));
                }
                if (!String.IsNullOrEmpty(SearchFieldType)) 
                {
                  _predicate = _predicate.And(p => p.FieldType.Contains(SearchFieldType));
                }
                if (!String.IsNullOrEmpty(SearchFieldLength.ToString())) 
                {
                  _predicate = _predicate.And(p => p.FieldLength.ToString().Contains(SearchFieldLength.ToString()));
                }
                if (!String.IsNullOrEmpty(SearchFieldRemark))
                {
                    _predicate = _predicate.And(p => p.FieldRemark.Contains(SearchFieldRemark));
                }
                //if (!String.IsNullOrEmpty(SearchIsMainKey))
                //{
                //    _predicate = _predicate.And(p => p.IsMainKey.Contains(SearchIsMainKey));
                //}
                //if (!String.IsNullOrEmpty(SearchClassName))
                //{
                //    _predicate = _predicate.And(p => p.IsAllowNull.Contains(SearchClassName));
                //}
                //if (!String.IsNullOrEmpty(SearchClassName))
                //{
                //    _predicate = _predicate.And(p => p.IsAutoIncrement.Contains(SearchClassName));
                //}
                //if (!String.IsNullOrEmpty(SearchClassName))
                //{
                //    _predicate = _predicate.And(p => p.IsUnique.Contains(SearchClassName));
                //}
                if (!String.IsNullOrEmpty(SearchDefaultValue))
                {
                    _predicate = _predicate.And(p => p.DefaultValue.Contains(SearchDefaultValue));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchFieldName = string.Empty;
            SearchFieldType = string.Empty;
            SearchFieldLength = 0;
            SearchFieldRemark = string.Empty;
            SearchIsMainKey = false;
            SearchIsAllowNull = false;
            SearchIsAutoIncrement = false;
            SearchIsUnique = false;
            SearchDefaultValue = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN);
        }

        /// <summary>
        /// index:0、1、2、3 == 15条、25条、50条、100条
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

                WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<CodeFieldActionWindowViewModel>();
            actionVM.Cid = Cid;//需要指定所属的 CodeClassId
            actionVM.FieldName = string.Empty;
            actionVM.FieldType = string.Empty;
            actionVM.FieldLength = 0;
            actionVM.FieldRemark = string.Empty;
            actionVM.IsMainKey = false;
            actionVM.IsAllowNull = false;
            actionVM.IsUnique = false;
            actionVM.DefaultValue = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.IsAddCodeField = true;
            actionVM.Title = I18nManager.GetString("CreateNewCodeField"); 
            var actionWindow = App.Views.CreateView<CodeFieldActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateCodeField(CodeField codeField)
        {
            var actionVM = App.ServiceProvider.GetService<CodeFieldActionWindowViewModel>();
            actionVM.Cid = Cid;//需要指定所属的 CodeClassId
            actionVM.SelectedCodeField = codeField;
            actionVM.FieldName = codeField.FieldName;
            actionVM.FieldType = codeField.FieldType;
            actionVM.FieldLength = codeField.FieldLength;
            actionVM.FieldRemark = codeField.FieldRemark;
            actionVM.IsMainKey = codeField.IsMainKey;
            actionVM.IsAllowNull = codeField.IsAllowNull;
            actionVM.IsAutoIncrement = codeField.IsAutoIncrement;
            actionVM.IsUnique = codeField.IsUnique;
            actionVM.DefaultValue = codeField.DefaultValue;
            actionVM.IsAddCodeField = false;
            actionVM.Title = I18nManager.GetString("UpdateCodeFieldInfo");
            var actionWindow = App.Views.CreateView<CodeFieldActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteCodeField()
        {
            var selectedCount = CodeFields.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
               .WithTitle(I18nManager.GetString("DeleteCodeFieldPrompt"))
               .WithContent(I18nManager.GetString("SelectDeleteCodeField"))
               .Dismiss().ByClickingBackground()
               .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
               .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
               .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCodeField"))
                .WithContent(I18nManager.GetString("SureWantToDeleteCodeField"))
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
            var selectedIds = CodeFields.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _codeFieldDao.DeleteRangeAsync<CodeField>(new List<int>(selectedIds));

            if (result > 0)
            {//20250401, 删除成功也用 dialog 提示
                dialog.Icon = Icons.Check;
                dialog.IconColor = NotificationColor.SuccessIconForeground;
                dialog.Title = I18nManager.GetString("DeleteCodeFieldPrompt");
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
                                    CurrentModelType = typeof(CodeField),
                                    Title = I18nManager.GetString("DeleteCodeFieldPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                    WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_FIELD_TOKEN);
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

