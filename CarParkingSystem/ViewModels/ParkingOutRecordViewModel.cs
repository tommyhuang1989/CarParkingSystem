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
  public partial class ParkingOutRecordViewModel: DemoPageBase
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkingOutRecord> _parkingOutRecords;
        [ObservableProperty] private ParkingOutRecord _selectedParkingOutRecord;
        [ObservableProperty] private string _updateInfo;
        private ParkingOutRecordDao _parkingOutRecordDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkingOutRecord> _predicate { get; set; }
        public Expression<Func<ParkingOutRecord, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool?  _isSelectedAll = false;
        [ObservableProperty] private System.Decimal _searchAmountMoney;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchCardNo;
        [ObservableProperty] private System.String _searchCarNo;
        [ObservableProperty] private System.Int32 _searchChargeType;
        [ObservableProperty] private System.Decimal _searchDiscountMoney;
        [ObservableProperty] private System.Int32 _searchInCarColor;
        [ObservableProperty] private System.Int32 _searchInCarType;
        [ObservableProperty] private System.Int32 _searchInCpChanged;
        [ObservableProperty] private System.String _searchInImg;
        [ObservableProperty] private System.Int32 _searchInOperatorId;
        [ObservableProperty] private System.String _searchInRemark;
        [ObservableProperty] private System.String _searchInTime;
        [ObservableProperty] private System.Int32 _searchInType;
        [ObservableProperty] private System.Int32 _searchInWayId;
        [ObservableProperty] private System.Int32 _searchOpenType;
        [ObservableProperty] private System.String _searchOrderId;
        [ObservableProperty] private System.Int32 _searchOutCarColor;
        [ObservableProperty] private System.Int32 _searchOutCarType;
        [ObservableProperty] private System.Int32 _searchOutCpChanged;
        [ObservableProperty] private System.String _searchOutImg;
        [ObservableProperty] private System.Int32 _searchOutOperatorId;
        [ObservableProperty] private System.String _searchOutTime;
        [ObservableProperty] private System.Int32 _searchOutType;
        [ObservableProperty] private System.Int32 _searchOutWayId;
        [ObservableProperty] private System.Decimal _searchPaidMoney;
        [ObservableProperty] private System.String _searchPlateId;
        [ObservableProperty] private System.Int32 _searchRecStatus;
        [ObservableProperty] private System.String _searchRemark;
        [ObservableProperty] private System.String _searchUpdateDt;
        [ObservableProperty] private System.Int32 _searchUpdateUser;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;
      public ParkingOutRecordViewModel(ParkingOutRecordDao parkingOutRecordDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.ParkingOutRecord, MaterialIconKind.Abacus, pid: 5, id: 28, index: 28)
        {
            _parkingOutRecordDao = parkingOutRecordDao;
            RefreshData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
WeakReferenceMessenger.Default.Register<SelectedParkingOutRecordMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN, ReciveRefreshSummarySelectedParkingOutRecord);
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
        private void ReciveRefreshSummarySelectedParkingOutRecord(object recipient, SelectedParkingOutRecordMessage message)
        {
          var selectedCount = ParkingOutRecords?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ParkingOutRecords.Count())
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
            if (message.CurrentModelType != typeof(ParkingOutRecord)) return;
            
            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void RefreshData(ExpressionStarter<ParkingOutRecord> predicate = null, Expression<Func<ParkingOutRecord, object>> expression = null, bool isDesc = false)
        {
          ParkingOutRecords = new ObservableCollection<ParkingOutRecord>(_parkingOutRecordDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
          AllCount = _parkingOutRecordDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ParkingOutRecord> predicate = null, Expression<Func<ParkingOutRecord, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
              var result = await _parkingOutRecordDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
              ParkingOutRecords = new ObservableCollection<ParkingOutRecord>(result);
                AllCount = _parkingOutRecordDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ParkingOutRecords.ToList(), "ParkingOutRecords.xlsx", I18nManager.GetString("ParkingOutRecordInfo"));
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
                foreach (var item in ParkingOutRecords)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ParkingOutRecords)
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
                    case "AmountMoney":
                        _expression = u => u.AmountMoney; break;
                    case "Id":
                        _expression = u => u.Id; break;
                    case "CardNo":
                        _expression = u => u.CardNo; break;
                    case "CarNo":
                        _expression = u => u.CarNo; break;
                    case "ChargeType":
                        _expression = u => u.ChargeType; break;
                    case "DiscountMoney":
                        _expression = u => u.DiscountMoney; break;
                    case "InCarColor":
                        _expression = u => u.InCarColor; break;
                    case "InCarType":
                        _expression = u => u.InCarType; break;
                    case "InCpChanged":
                        _expression = u => u.InCpChanged; break;
                    case "InImg":
                        _expression = u => u.InImg; break;
                    case "InOperatorId":
                        _expression = u => u.InOperatorId; break;
                    case "InRemark":
                        _expression = u => u.InRemark; break;
                    case "InTime":
                        _expression = u => u.InTime; break;
                    case "InType":
                        _expression = u => u.InType; break;
                    case "InWayId":
                        _expression = u => u.InWayId; break;
                    case "OpenType":
                        _expression = u => u.OpenType; break;
                    case "OrderId":
                        _expression = u => u.OrderId; break;
                    case "OutCarColor":
                        _expression = u => u.OutCarColor; break;
                    case "OutCarType":
                        _expression = u => u.OutCarType; break;
                    case "OutCpChanged":
                        _expression = u => u.OutCpChanged; break;
                    case "OutImg":
                        _expression = u => u.OutImg; break;
                    case "OutOperatorId":
                        _expression = u => u.OutOperatorId; break;
                    case "OutTime":
                        _expression = u => u.OutTime; break;
                    case "OutType":
                        _expression = u => u.OutType; break;
                    case "OutWayId":
                        _expression = u => u.OutWayId; break;
                    case "PaidMoney":
                        _expression = u => u.PaidMoney; break;
                    case "PlateId":
                        _expression = u => u.PlateId; break;
                    case "RecStatus":
                        _expression = u => u.RecStatus; break;
                    case "Remark":
                        _expression = u => u.Remark; break;
                    case "UpdateDt":
                        _expression = u => u.UpdateDt; break;
                    case "UpdateUser":
                        _expression = u => u.UpdateUser; break;
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
        private Task SearchParkingOutRecord()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ParkingOutRecord>(true);
                if (!String.IsNullOrEmpty(SearchCarNo)) 
                {
                  _predicate = _predicate.And(p => p.CarNo.Contains(SearchCarNo));
                }
                if (!String.IsNullOrEmpty(SearchInImg)) 
                {
                  _predicate = _predicate.And(p => p.InImg.Contains(SearchInImg));
                }
                if (!String.IsNullOrEmpty(SearchInRemark)) 
                {
                  _predicate = _predicate.And(p => p.InRemark.Contains(SearchInRemark));
                }
                if (!String.IsNullOrEmpty(SearchInTime)) 
                {
                  _predicate = _predicate.And(p => p.InTime.Contains(SearchInTime));
                }
                if (!String.IsNullOrEmpty(SearchOrderId)) 
                {
                  _predicate = _predicate.And(p => p.OrderId.Contains(SearchOrderId));
                }
                if (!String.IsNullOrEmpty(SearchOutImg)) 
                {
                  _predicate = _predicate.And(p => p.OutImg.Contains(SearchOutImg));
                }
                if (!String.IsNullOrEmpty(SearchOutTime)) 
                {
                  _predicate = _predicate.And(p => p.OutTime.Contains(SearchOutTime));
                }
                if (!String.IsNullOrEmpty(SearchPlateId)) 
                {
                  _predicate = _predicate.And(p => p.PlateId.Contains(SearchPlateId));
                }
                if (!String.IsNullOrEmpty(SearchRemark)) 
                {
                  _predicate = _predicate.And(p => p.Remark.Contains(SearchRemark));
                }
                if (!String.IsNullOrEmpty(SearchUpdateDt)) 
                {
                  _predicate = _predicate.And(p => p.UpdateDt.Contains(SearchUpdateDt));
                }
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchCarNo = string.Empty;
            SearchInImg = string.Empty;
            SearchInRemark = string.Empty;
            SearchInTime = string.Empty;
            SearchOrderId = string.Empty;
            SearchOutImg = string.Empty;
            SearchOutTime = string.Empty;
            SearchPlateId = string.Empty;
            SearchRemark = string.Empty;
            SearchUpdateDt = string.Empty;
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
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

                WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
            });
        }

        [RelayCommand]
        private void Add()
        {
            var actionVM = App.ServiceProvider.GetService<ParkingOutRecordActionWindowViewModel>();
            actionVM.CarNo = string.Empty;
            actionVM.InImg = string.Empty;
            actionVM.InRemark = string.Empty;
            actionVM.InTime = string.Empty;
            actionVM.OrderId = string.Empty;
            actionVM.OutImg = string.Empty;
            actionVM.OutTime = string.Empty;
            actionVM.PlateId = string.Empty;
            actionVM.Remark = string.Empty;
            actionVM.UpdateDt = string.Empty;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkingOutRecord = true;
            actionVM.Title = I18nManager.GetString("CreateNewParkingOutRecord"); 
            var actionWindow = App.Views.CreateView<ParkingOutRecordActionWindowViewModel>(App.ServiceProvider) as Window;

            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (owner != null)
            {
                actionWindow?.ShowDialog(owner);
            }
            else
            {
                actionWindow?.Show();
            }

            WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
            
            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = true,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);
        }

        [RelayCommand]
        private void UpdateParkingOutRecord(ParkingOutRecord parkingOutRecord)
        {
            var actionVM = App.ServiceProvider.GetService<ParkingOutRecordActionWindowViewModel>();
            actionVM.SelectedParkingOutRecord = parkingOutRecord;
            actionVM.CarNo = parkingOutRecord.CarNo;
            actionVM.InImg = parkingOutRecord.InImg;
            actionVM.InRemark = parkingOutRecord.InRemark;
            actionVM.InTime = parkingOutRecord.InTime;
            actionVM.OrderId = parkingOutRecord.OrderId;
            actionVM.OutImg = parkingOutRecord.OutImg;
            actionVM.OutTime = parkingOutRecord.OutTime;
            actionVM.PlateId = parkingOutRecord.PlateId;
            actionVM.Remark = parkingOutRecord.Remark;
            actionVM.UpdateDt = parkingOutRecord.UpdateDt;
            actionVM.UpdateInfo = string.Empty;
            actionVM.ClearVertifyErrors();
            actionVM.IsAddParkingOutRecord = false;
            actionVM.Title = I18nManager.GetString("UpdateParkingOutRecordInfo");
            var actionWindow = App.Views.CreateView<ParkingOutRecordActionWindowViewModel>(App.ServiceProvider) as Window;
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
        private void DeleteParkingOutRecord()
        {
            var selectedCount = ParkingOutRecords.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkingOutRecordPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteParkingOutRecord"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkingOutRecord"))
                .WithContent(I18nManager.GetString("SureWantToDeleteParkingOutRecord"))
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
            var selectedIds = ParkingOutRecords.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _parkingOutRecordDao.DeleteRangeAsync<ParkingOutRecord>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteParkingOutRecordPrompt");
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
                                    CurrentModelType = typeof(ParkingOutRecord),
                                    Title = I18nManager.GetString("DeleteParkingOutRecordPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
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

