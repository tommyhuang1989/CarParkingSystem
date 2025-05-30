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
  public partial class CarTypeParaViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<CarTypePara> _carTypeParas;
        [ObservableProperty] private CarTypePara _selectedCarTypePara;
        [ObservableProperty] private string _updateInfo;
        private CarTypeParaDao _carTypeParaDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<CarTypePara> _predicate { get; set; }
        public Expression<Func<CarTypePara, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchCardNo;
        [ObservableProperty] private System.String _searchCarTypeName;
        [ObservableProperty] private System.String _searchHeight;
        [ObservableProperty] private System.String _searchWidth;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public CarTypeParaViewModel(CarTypeParaDao carTypeParaDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.CarTypeSettings, MaterialIconKind.Abacus, pid: 4, id: 26, index: 26)
        {
            _carTypeParaDao = carTypeParaDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedCarTypeParaMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN, ReciveRefreshSummarySelectedCarTypePara);
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
        private void ReciveRefreshSummarySelectedCarTypePara(object recipient, SelectedCarTypeParaMessage message)
        {
          var selectedCount = CarTypeParas?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == CarTypeParas.Count())
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
            if (message.CurrentModelType != typeof(CarTypePara)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<CarTypePara> predicate = null, Expression<Func<CarTypePara, object>> expression = null, bool isDesc = false)
        {
          CarTypeParas = new ObservableCollection<CarTypePara>(_carTypeParaDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _carTypeParaDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<CarTypePara> predicate = null, Expression<Func<CarTypePara, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _carTypeParaDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              CarTypeParas = new ObservableCollection<CarTypePara>(result);
                AllCount = _carTypeParaDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, CarTypeParas.ToList(), "CarTypeParas.xlsx", I18nManager.GetString("CarTypeParaInfo"));
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
                foreach (var item in CarTypeParas)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in CarTypeParas)
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
                    case "CarTypeName":
                        _expression = u => u.CarTypeName; break;
                    case "Height":
                        _expression = u => u.Height; break;
                    case "Width":
                        _expression = u => u.Width; break;
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
        private Task SearchCarTypePara()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<CarTypePara>(true);
                if (!String.IsNullOrEmpty(SearchCarTypeName)) 
                {
                  _predicate = _predicate.And(p => p.CarTypeName.Contains(SearchCarTypeName));
                }
                if (!String.IsNullOrEmpty(SearchHeight)) 
                {
                  _predicate = _predicate.And(p => p.Height.Contains(SearchHeight));
                }
                if (!String.IsNullOrEmpty(SearchWidth)) 
                {
                  _predicate = _predicate.And(p => p.Width.Contains(SearchWidth));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCarTypeName = string.Empty;
            SearchHeight = string.Empty;
            SearchWidth = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<CarTypeParaActionWindowViewModel>();
            actionVM.CarTypeName = string.Empty;
            actionVM.Height = string.Empty;
            actionVM.Width = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarTypePara = true;
            actionVM.Title = I18nManager.GetString("CreateNewCarTypePara"); 
            var actionWindow = App.Views.CreateView<CarTypeParaActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateCarTypePara(CarTypePara carTypePara)
        {
            var actionVM = App.ServiceProvider.GetService<CarTypeParaActionWindowViewModel>();
            actionVM.SelectedCarTypePara = carTypePara;
            actionVM.CarTypeName = carTypePara.CarTypeName;
            actionVM.Height = carTypePara.Height;
            actionVM.Width = carTypePara.Width;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddCarTypePara = false;
            actionVM.Title = I18nManager.GetString("UpdateCarTypeParaInfo");
            var actionWindow = App.Views.CreateView<CarTypeParaActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteCarTypePara()
        {
            var selectedCount = CarTypeParas.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarTypeParaPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteCarTypePara"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteCarTypePara"))
                .WithContent(I18nManager.GetString("SureWantToDeleteCarTypePara"))
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
            var selectedIds = CarTypeParas.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _carTypeParaDao.DeleteRangeAsync<CarTypePara>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteCarTypeParaPrompt");
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
                                    CurrentModelType = typeof(CarTypePara),
                                    Title = I18nManager.GetString("DeleteCarTypeParaPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
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

