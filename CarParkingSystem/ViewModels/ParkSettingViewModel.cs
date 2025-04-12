using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
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
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SukiUI.Content;
using SukiUI.ColorTheme;
using Avalonia.Xaml.Interactions.Core;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 车辆管理菜单里的 Tab 车辆管理（表单）
    /// </summary>
    public partial class ParkSettingViewModel : ParkSettingsTabViewModel
    {
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<ParkSetting> _parkSettings;
        [ObservableProperty] private ParkSetting _selectedParkSetting;
        [ObservableProperty] private string _updateInfo;
        private ParkSettingDao _parkSettingDao;
        private AppDbContext _appDbContext;
        public ISukiDialogManager _dialogManager { get; }
        public ExpressionStarter<ParkSetting> _predicate { get; set; }
        public Expression<Func<ParkSetting, object>> _expression { get; set; }
        public bool _isOrderByDesc { get; set; }
        [ObservableProperty] private int _allCount;
        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条
        [ObservableProperty] private int _pageCount;
        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页
        [ObservableProperty] private int _indexToGo;
        [ObservableProperty] private bool? _isSelectedAll = false;
        [ObservableProperty] private System.Int32 _searchAbnormalSetting;
        [ObservableProperty] private System.Int32 _searchAutoMatch;
        [ObservableProperty] private System.Int32 _searchCarUpperLimit;
        [ObservableProperty] private System.Int32 _searchCarUpperLimitProcess;
        [ObservableProperty] private System.Int32 _searchChangeTempCar;
        [ObservableProperty] private System.Int32 _searchDefaultCardId;
        [ObservableProperty] private System.Int32 _searchDelayBySpace;
        [ObservableProperty] private System.Int32 _searchDelayTime;
        [ObservableProperty] private System.Int32 _searchEntryWayWaittingCar;
        [ObservableProperty] private System.Int32 _searchFreeTime;
        [ObservableProperty] private System.Int32 _searchIsNeedReason;
        [ObservableProperty] private System.Int32 _searchIsSelfEntry;
        [ObservableProperty] private System.Int32 _searchLeaveDate;
        [ObservableProperty] private System.Int32 _searchMotorbikeDefaultCard;
        [ObservableProperty] private System.Int32 _searchMulSpace;
        [ObservableProperty] private System.Int32 _searchAysnId;
        [ObservableProperty] private System.Int32 _searchMulSpaceExpired;
        [ObservableProperty] private System.Int32 _searchOneLotMoreCarEnter;
        [ObservableProperty] private System.Int32 _searchOneLotMoreCarTempCar;
        [ObservableProperty] private System.Int32 _searchParkingFull;
        [ObservableProperty] private System.Int32 _searchResCanOpenTime;
        [ObservableProperty] private System.Int32 _searchShowTodayIncome;
        [ObservableProperty] private System.Int32 _searchTempCarManager;
        [ObservableProperty] private System.Int32 _searchUnlicensedModel;
        [ObservableProperty] private System.Int32 _searchUnsaveInAbnormal;
        [ObservableProperty] private System.Int32 _searchUnsaveManualAbnormal;
        [ObservableProperty] private System.Int32 _searchUnsaveOutAbnormal;
        [ObservableProperty] private System.Int32 _searchValueCardDeduction;
        [ObservableProperty] private string _searchStartDateTime;
        [ObservableProperty] private string _searchEndDateTime;


        [Required(StringResourceKey.AbnormalSettingRequired)]
        [ObservableProperty]
        private System.Int32 _abnormalSetting;
        [Required(StringResourceKey.AutoMatchRequired)]
        [ObservableProperty]
        private System.Int32 _autoMatch;
        [Required(StringResourceKey.CarUpperLimitRequired)]
        [ObservableProperty]
        private System.Int32 _carUpperLimit;
        [Required(StringResourceKey.CarUpperLimitProcessRequired)]
        [ObservableProperty]
        private System.Int32 _carUpperLimitProcess;
        [Required(StringResourceKey.ChangeTempCarRequired)]
        [ObservableProperty]
        private System.Int32 _changeTempCar;
        [Required(StringResourceKey.DefaultCardIdRequired)]
        [ObservableProperty]
        private System.Int32 _defaultCardId;
        [Required(StringResourceKey.DelayBySpaceRequired)]
        [ObservableProperty]
        private System.Int32 _delayBySpace;
        [Required(StringResourceKey.DelayTimeRequired)]
        [ObservableProperty]
        private System.Int32 _delayTime;
        [Required(StringResourceKey.EntryWayWaittingCarRequired)]
        [ObservableProperty]
        private System.Int32 _entryWayWaittingCar;
        [Required(StringResourceKey.FreeTimeRequired)]
        [ObservableProperty]
        private System.Int32 _freeTime;
        [Required(StringResourceKey.IsNeedReasonRequired)]
        [ObservableProperty]
        private System.Int32 _isNeedReason;
        [Required(StringResourceKey.IsSelfEntryRequired)]
        [ObservableProperty]
        private System.Int32 _isSelfEntry;
        [Required(StringResourceKey.LeaveDateRequired)]
        [ObservableProperty]
        private System.Int32 _leaveDate;
        [Required(StringResourceKey.MotorbikeDefaultCardRequired)]
        [ObservableProperty]
        private System.Int32 _motorbikeDefaultCard;
        [Required(StringResourceKey.MulSpaceRequired)]
        [ObservableProperty]
        private System.Int32 _mulSpace;
        [Required(StringResourceKey.MulSpaceExpiredRequired)]
        [ObservableProperty]
        private System.Int32 _mulSpaceExpired;
        [Required(StringResourceKey.OneLotMoreCarEnterRequired)]
        [ObservableProperty]
        private System.Int32 _oneLotMoreCarEnter;
        [Required(StringResourceKey.OneLotMoreCarTempCarRequired)]
        [ObservableProperty]
        private System.Int32 _oneLotMoreCarTempCar;
        [Required(StringResourceKey.ParkingFullRequired)]
        [ObservableProperty]
        private System.Int32 _parkingFull;
        [Required(StringResourceKey.ResCanOpenTimeRequired)]
        [ObservableProperty]
        private System.Int32 _resCanOpenTime;
        [Required(StringResourceKey.ShowTodayIncomeRequired)]
        [ObservableProperty]
        private System.Int32 _showTodayIncome;
        [Required(StringResourceKey.TempCarManagerRequired)]
        [ObservableProperty]
        private System.Int32 _tempCarManager;
        [Required(StringResourceKey.UnlicensedModelRequired)]
        [ObservableProperty]
        private System.Int32 _unlicensedModel;
        [Required(StringResourceKey.UnsaveInAbnormalRequired)]
        [ObservableProperty]
        private System.Int32 _unsaveInAbnormal;
        [Required(StringResourceKey.UnsaveManualAbnormalRequired)]
        [ObservableProperty]
        private System.Int32 _unsaveManualAbnormal;
        [Required(StringResourceKey.UnsaveOutAbnormalRequired)]
        [ObservableProperty]
        private System.Int32 _unsaveOutAbnormal;
        [Required(StringResourceKey.ValueCardDeductionRequired)]
        [ObservableProperty]
        private System.Int32 _valueCardDeduction;

        public ParkSettingViewModel(ParkSettingDao parkSettingDao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(Language.ParkSettings)
        {
            _parkSettingDao = parkSettingDao;
            RefreshData();
            GetData();
            _appDbContext = appDbContext;
            _dialogManager = dialogManager;
            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);
            WeakReferenceMessenger.Default.Register<SelectedParkSettingMessage, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN, ReciveRefreshSummarySelectedParkSetting);
        }

        [ObservableProperty] private string _abnormalSettingValidationMessage;
        [ObservableProperty] private string _autoMatchValidationMessage;
        [ObservableProperty] private string _carUpperLimitValidationMessage;
        [ObservableProperty] private string _carUpperLimitProcessValidationMessage;
        [ObservableProperty] private string _changeTempCarValidationMessage;
        [ObservableProperty] private string _defaultCardIdValidationMessage;
        [ObservableProperty] private string _delayBySpaceValidationMessage;
        [ObservableProperty] private string _delayTimeValidationMessage;
        [ObservableProperty] private string _entryWayWaittingCarValidationMessage;
        [ObservableProperty] private string _freeTimeValidationMessage;
        [ObservableProperty] private string _isNeedReasonValidationMessage;
        [ObservableProperty] private string _isSelfEntryValidationMessage;
        [ObservableProperty] private string _leaveDateValidationMessage;
        [ObservableProperty] private string _motorbikeDefaultCardValidationMessage;
        [ObservableProperty] private string _mulSpaceValidationMessage;
        [ObservableProperty] private string _mulSpaceExpiredValidationMessage;
        [ObservableProperty] private string _oneLotMoreCarEnterValidationMessage;
        [ObservableProperty] private string _oneLotMoreCarTempCarValidationMessage;
        [ObservableProperty] private string _parkingFullValidationMessage;
        [ObservableProperty] private string _resCanOpenTimeValidationMessage;
        [ObservableProperty] private string _showTodayIncomeValidationMessage;
        [ObservableProperty] private string _tempCarManagerValidationMessage;
        [ObservableProperty] private string _unlicensedModelValidationMessage;
        [ObservableProperty] private string _unsaveInAbnormalValidationMessage;
        [ObservableProperty] private string _unsaveManualAbnormalValidationMessage;
        [ObservableProperty] private string _unsaveOutAbnormalValidationMessage;
        [ObservableProperty] private string _valueCardDeductionValidationMessage;

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(AbnormalSetting));
            UpdateValidationMessage(nameof(AutoMatch));
            UpdateValidationMessage(nameof(CarUpperLimit));
            UpdateValidationMessage(nameof(CarUpperLimitProcess));
            UpdateValidationMessage(nameof(ChangeTempCar));
            UpdateValidationMessage(nameof(DefaultCardId));
            UpdateValidationMessage(nameof(DelayBySpace));
            UpdateValidationMessage(nameof(DelayTime));
            UpdateValidationMessage(nameof(EntryWayWaittingCar));
            UpdateValidationMessage(nameof(FreeTime));
            UpdateValidationMessage(nameof(IsNeedReason));
            UpdateValidationMessage(nameof(IsSelfEntry));
            UpdateValidationMessage(nameof(LeaveDate));
            UpdateValidationMessage(nameof(MotorbikeDefaultCard));
            UpdateValidationMessage(nameof(MulSpace));
            UpdateValidationMessage(nameof(MulSpaceExpired));
            UpdateValidationMessage(nameof(OneLotMoreCarEnter));
            UpdateValidationMessage(nameof(OneLotMoreCarTempCar));
            UpdateValidationMessage(nameof(ParkingFull));
            UpdateValidationMessage(nameof(ResCanOpenTime));
            UpdateValidationMessage(nameof(ShowTodayIncome));
            UpdateValidationMessage(nameof(TempCarManager));
            UpdateValidationMessage(nameof(UnlicensedModel));
            UpdateValidationMessage(nameof(UnsaveInAbnormal));
            UpdateValidationMessage(nameof(UnsaveManualAbnormal));
            UpdateValidationMessage(nameof(UnsaveOutAbnormal));
            UpdateValidationMessage(nameof(ValueCardDeduction));
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
        private void ReciveRefreshSummarySelectedParkSetting(object recipient, SelectedParkSettingMessage message)
        {
            var selectedCount = ParkSettings?.Count(x => x.IsSelected);
            if (selectedCount == 0)
            {
                IsSelectedAll = false;
            }
            else if (selectedCount == ParkSettings.Count())
            {
                IsSelectedAll = true;
            }
            else
            {
                IsSelectedAll = null;
            }
        }

        private void Recive(object recipient, ToastMessage message)
        {
            //20250402, 不是相同类型发送的消息不处理
            if (message.CurrentModelType != typeof(ParkSetting)) return;

            if (message.NeedRefreshData)
            {
                RefreshData(_predicate, _expression, _isOrderByDesc);
            }
        }

        private void GetData()
        {
            var tempParkingSettings = _parkSettingDao.GetAll().FirstOrDefault();
            if (tempParkingSettings != null)
            {
                AbnormalSetting = tempParkingSettings.AbnormalSetting;
                AutoMatch = tempParkingSettings.AutoMatch;
                CarUpperLimit = tempParkingSettings.CarUpperLimit;
                CarUpperLimitProcess = tempParkingSettings.CarUpperLimitProcess;
                ChangeTempCar = tempParkingSettings.ChangeTempCar;
                DefaultCardId = tempParkingSettings.DefaultCardId;
                DelayBySpace = tempParkingSettings.DelayBySpace;
                DelayTime = tempParkingSettings.DelayTime;
                EntryWayWaittingCar = tempParkingSettings.EntryWayWaittingCar;
                FreeTime = tempParkingSettings.FreeTime;
                IsNeedReason = tempParkingSettings.IsNeedReason;
                IsSelfEntry = tempParkingSettings.IsSelfEntry;
                LeaveDate = tempParkingSettings.LeaveDate;
                MotorbikeDefaultCard = tempParkingSettings.MotorbikeDefaultCard;
                MulSpace = tempParkingSettings.MulSpace;
                MulSpaceExpired = tempParkingSettings.MulSpaceExpired;
                OneLotMoreCarEnter = tempParkingSettings.OneLotMoreCarEnter;
                OneLotMoreCarTempCar = tempParkingSettings.OneLotMoreCarTempCar;
                ParkingFull = tempParkingSettings.ParkingFull;
                ResCanOpenTime = tempParkingSettings.ResCanOpenTime;
                ShowTodayIncome = tempParkingSettings.ShowTodayIncome;
                TempCarManager = tempParkingSettings.TempCarManager;
                UnlicensedModel = tempParkingSettings.UnlicensedModel;
                UnsaveInAbnormal = tempParkingSettings.UnsaveInAbnormal;
                UnsaveManualAbnormal = tempParkingSettings.UnsaveManualAbnormal;
                UnsaveOutAbnormal = tempParkingSettings.UnsaveOutAbnormal;
                ValueCardDeduction = tempParkingSettings.ValueCardDeduction;
            }
        }
        private void RefreshData(ExpressionStarter<ParkSetting> predicate = null, Expression<Func<ParkSetting, object>> expression = null, bool isDesc = false)
        {
            ParkSettings = new ObservableCollection<ParkSetting>(_parkSettingDao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));
            AllCount = _parkSettingDao.Count(predicate);
            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
        }

        private Task RefreshDataAsync(ExpressionStarter<ParkSetting> predicate = null, Expression<Func<ParkSetting, object>> expression = null, bool isDesc = false)
        {
            return Task.Run(async () => {
                var result = await _parkSettingDao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);
                ParkSettings = new ObservableCollection<ParkSetting>(result);
                AllCount = _parkSettingDao.Count(predicate);
                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);
            });
        }
        [RelayCommand]
        private Task Export(Window window)
        {
            return Task.Run(async () =>
            {
                var filePath = await ExcelService.ExportToExcelAsync(window, ParkSettings.ToList(), "ParkSettings.xlsx", I18nManager.GetString("ParkSettingInfo"));
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

            if (IsSelectedAll == false)
            {
                foreach (var item in ParkSettings)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in ParkSettings)
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
                    case "AbnormalSetting":
                        _expression = u => u.AbnormalSetting; break;
                    case "AutoMatch":
                        _expression = u => u.AutoMatch; break;
                    case "CarUpperLimit":
                        _expression = u => u.CarUpperLimit; break;
                    case "CarUpperLimitProcess":
                        _expression = u => u.CarUpperLimitProcess; break;
                    case "ChangeTempCar":
                        _expression = u => u.ChangeTempCar; break;
                    case "DefaultCardId":
                        _expression = u => u.DefaultCardId; break;
                    case "DelayBySpace":
                        _expression = u => u.DelayBySpace; break;
                    case "DelayTime":
                        _expression = u => u.DelayTime; break;
                    case "EntryWayWaittingCar":
                        _expression = u => u.EntryWayWaittingCar; break;
                    case "FreeTime":
                        _expression = u => u.FreeTime; break;
                    case "IsNeedReason":
                        _expression = u => u.IsNeedReason; break;
                    case "IsSelfEntry":
                        _expression = u => u.IsSelfEntry; break;
                    case "LeaveDate":
                        _expression = u => u.LeaveDate; break;
                    case "MotorbikeDefaultCard":
                        _expression = u => u.MotorbikeDefaultCard; break;
                    case "MulSpace":
                        _expression = u => u.MulSpace; break;
                    case "Id":
                        _expression = u => u.Id; break;
                    case "MulSpaceExpired":
                        _expression = u => u.MulSpaceExpired; break;
                    case "OneLotMoreCarEnter":
                        _expression = u => u.OneLotMoreCarEnter; break;
                    case "OneLotMoreCarTempCar":
                        _expression = u => u.OneLotMoreCarTempCar; break;
                    case "ParkingFull":
                        _expression = u => u.ParkingFull; break;
                    case "ResCanOpenTime":
                        _expression = u => u.ResCanOpenTime; break;
                    case "ShowTodayIncome":
                        _expression = u => u.ShowTodayIncome; break;
                    case "TempCarManager":
                        _expression = u => u.TempCarManager; break;
                    case "UnlicensedModel":
                        _expression = u => u.UnlicensedModel; break;
                    case "UnsaveInAbnormal":
                        _expression = u => u.UnsaveInAbnormal; break;
                    case "UnsaveManualAbnormal":
                        _expression = u => u.UnsaveManualAbnormal; break;
                    case "UnsaveOutAbnormal":
                        _expression = u => u.UnsaveOutAbnormal; break;
                    case "ValueCardDeduction":
                        _expression = u => u.ValueCardDeduction; break;
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
        private Task SearchParkSetting()
        {
            return Task.Run(async () =>
            {
                _predicate = PredicateBuilder.New<ParkSetting>(true);
                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示
                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);
                WeakReferenceMessenger.Default.Send<SelectedParkSettingMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN);
            });
        }

        [RelayCommand]
        private void ResetSearch()
        {
            SearchStartDateTime = null;
            SearchEndDateTime = null;
            _predicate = null;//条件查询的 表达式 也进行重置
            _expression = null;
            _isOrderByDesc = false;
            WeakReferenceMessenger.Default.Send<SelectedParkSettingMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private Task SelectionChanged(int pageSize)
        {
            return Task.Run(async () =>
            {
                PageSize = pageSize;

                CurrentPageIndex = 1;//更改页面数量应该从第一页显示

                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);

                WeakReferenceMessenger.Default.Send<SelectedParkSettingMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN);
            });
        }


        [RelayCommand]
        private Task Save()
        {
            if (IsBusy)
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                IsBusy = true;

                ValidateAllProperties();
                //前端验证
                if (HasErrors)
                {
                    UpdateValidationMessage(nameof(AbnormalSetting));
                    UpdateValidationMessage(nameof(AutoMatch));
                    UpdateValidationMessage(nameof(CarUpperLimit));
                    UpdateValidationMessage(nameof(CarUpperLimitProcess));
                    UpdateValidationMessage(nameof(ChangeTempCar));
                    UpdateValidationMessage(nameof(DefaultCardId));
                    UpdateValidationMessage(nameof(DelayBySpace));
                    UpdateValidationMessage(nameof(DelayTime));
                    UpdateValidationMessage(nameof(EntryWayWaittingCar));
                    UpdateValidationMessage(nameof(FreeTime));
                    UpdateValidationMessage(nameof(IsNeedReason));
                    UpdateValidationMessage(nameof(IsSelfEntry));
                    UpdateValidationMessage(nameof(LeaveDate));
                    UpdateValidationMessage(nameof(MotorbikeDefaultCard));
                    UpdateValidationMessage(nameof(MulSpace));
                    UpdateValidationMessage(nameof(MulSpaceExpired));
                    UpdateValidationMessage(nameof(OneLotMoreCarEnter));
                    UpdateValidationMessage(nameof(OneLotMoreCarTempCar));
                    UpdateValidationMessage(nameof(ParkingFull));
                    UpdateValidationMessage(nameof(ResCanOpenTime));
                    UpdateValidationMessage(nameof(ShowTodayIncome));
                    UpdateValidationMessage(nameof(TempCarManager));
                    UpdateValidationMessage(nameof(UnlicensedModel));
                    UpdateValidationMessage(nameof(UnsaveInAbnormal));
                    UpdateValidationMessage(nameof(UnsaveManualAbnormal));
                    UpdateValidationMessage(nameof(UnsaveOutAbnormal));
                    UpdateValidationMessage(nameof(ValueCardDeduction));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                /*
                var hasSameRecord = false;
                if ((bool)hasSameRecord)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        ToastManager.CreateToast()
                .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                .WithContent(I18nManager.GetString("ParkSettingExists"))
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                    });
                    return;
                }
                */

                //var tempParkSetting = _parkSettingDao.GetById(SelectedParkSetting.Id);
                var tempParkSetting = _parkSettingDao.GetAll().FirstOrDefault();
                if (tempParkSetting != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempParkSetting.AbnormalSetting = AbnormalSetting;
                    tempParkSetting.AutoMatch = AutoMatch;
                    tempParkSetting.CarUpperLimit = CarUpperLimit;
                    tempParkSetting.CarUpperLimitProcess = CarUpperLimitProcess;
                    tempParkSetting.ChangeTempCar = ChangeTempCar;
                    tempParkSetting.DefaultCardId = DefaultCardId;
                    tempParkSetting.DelayBySpace = DelayBySpace;
                    tempParkSetting.DelayTime = DelayTime;
                    tempParkSetting.EntryWayWaittingCar = EntryWayWaittingCar;
                    tempParkSetting.FreeTime = FreeTime;
                    tempParkSetting.IsNeedReason = IsNeedReason;
                    tempParkSetting.IsSelfEntry = IsSelfEntry;
                    tempParkSetting.LeaveDate = LeaveDate;
                    tempParkSetting.MotorbikeDefaultCard = MotorbikeDefaultCard;
                    tempParkSetting.MulSpace = MulSpace;
                    tempParkSetting.MulSpaceExpired = MulSpaceExpired;
                    tempParkSetting.OneLotMoreCarEnter = OneLotMoreCarEnter;
                    tempParkSetting.OneLotMoreCarTempCar = OneLotMoreCarTempCar;
                    tempParkSetting.ParkingFull = ParkingFull;
                    tempParkSetting.ResCanOpenTime = ResCanOpenTime;
                    tempParkSetting.ShowTodayIncome = ShowTodayIncome;
                    tempParkSetting.TempCarManager = TempCarManager;
                    tempParkSetting.UnlicensedModel = UnlicensedModel;
                    tempParkSetting.UnsaveInAbnormal = UnsaveInAbnormal;
                    tempParkSetting.UnsaveManualAbnormal = UnsaveManualAbnormal;
                    tempParkSetting.UnsaveOutAbnormal = UnsaveOutAbnormal;
                    tempParkSetting.ValueCardDeduction = ValueCardDeduction;
                    int result = _parkSettingDao.Update(tempParkSetting);
                    if (result >= 0)
                    {

                        Dispatcher.UIThread.Invoke(() =>
                        {
                            //WeakReferenceMessenger.Default.Send(
                            //     new ToastMessage
                            //     {
                            //         CurrentModelType = typeof(ParkSetting),
                            //         Title = I18nManager.GetString("UpdateParkSettingPrompt"),
                            //         Content = I18nManager.GetString("UpdateSuccessfully"),
                            //         NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            //         NeedRefreshData = true
                            //     }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            //WeakReferenceMessenger.Default.Send("Close ParkSettingActionWindow", TokenManage.PARKSETTING_ACTION_WINDOW_CLOSE_TOKEN);

                            _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                        });
                    }
                    else
                    {
                        //var message = I18nManager.GetString("UpdateFailed");
                        //UpdateInfo = message;
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString(Language.UpdateParkSettingPrompt))
                .WithContent(I18nManager.GetString(Language.UpdateFailed))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
                        });
                    }
                    await Task.Delay(2000);

                    IsBusy = false;
                }
                else
                {
                    //没有记录就插入
                    tempParkSetting = new ParkSetting
                    {
                        AbnormalSetting = AbnormalSetting,
                        AutoMatch = AutoMatch,
                        CarUpperLimit = CarUpperLimit,
                        CarUpperLimitProcess = CarUpperLimitProcess,
                        ChangeTempCar = ChangeTempCar,
                        DefaultCardId = DefaultCardId,
                        DelayBySpace = DelayBySpace,
                        DelayTime = DelayTime,
                        EntryWayWaittingCar = EntryWayWaittingCar,
                        FreeTime = FreeTime,
                        IsNeedReason = IsNeedReason,
                        IsSelfEntry = IsSelfEntry,
                        LeaveDate = LeaveDate,
                        MotorbikeDefaultCard = MotorbikeDefaultCard,
                        MulSpace = MulSpace,
                        MulSpaceExpired = MulSpaceExpired,
                        OneLotMoreCarEnter = OneLotMoreCarEnter,
                        OneLotMoreCarTempCar = OneLotMoreCarTempCar,
                        ParkingFull = ParkingFull,
                        ResCanOpenTime = ResCanOpenTime,
                        ShowTodayIncome = ShowTodayIncome,
                        TempCarManager = TempCarManager,
                        UnlicensedModel = UnlicensedModel,
                        UnsaveInAbnormal = UnsaveInAbnormal,
                        UnsaveManualAbnormal = UnsaveManualAbnormal,
                        UnsaveOutAbnormal = UnsaveOutAbnormal,
                        ValueCardDeduction = ValueCardDeduction,
                    };

                    int result = _parkSettingDao.Add(tempParkSetting);
                    if (result >= 0)
                    {

                        Dispatcher.UIThread.Invoke(() =>
                        {
                            //WeakReferenceMessenger.Default.Send(
                            //     new ToastMessage
                            //     {
                            //         CurrentModelType = typeof(ParkSetting),
                            //         Title = I18nManager.GetString("UpdateParkSettingPrompt"),
                            //         Content = I18nManager.GetString("UpdateSuccessfully"),
                            //         NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                            //         NeedRefreshData = true
                            //     }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            //WeakReferenceMessenger.Default.Send("Close ParkSettingActionWindow", TokenManage.PARKSETTING_ACTION_WINDOW_CLOSE_TOKEN);

                            _dialogManager.CreateDialog()
                    .WithTitle(I18nManager.GetString("UpdateParkSettingPrompt"))
                    .WithContent(I18nManager.GetString("UpdateSuccessfully"))
                    .Dismiss().ByClickingBackground()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                    .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                    .TryShow();
                        });
                    }
                    else
                    {
                        //var message = I18nManager.GetString("UpdateFailed");
                        //UpdateInfo = message;
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString(Language.UpdateParkSettingPrompt))
                .WithContent(I18nManager.GetString(Language.UpdateFailed))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
                        });
                    }
                }
            });
        }

        [RelayCommand]
        private void DeleteParkSetting()
        {
            var selectedCount = ParkSettings.Count(u => u.IsSelected);
            if (selectedCount == 0)
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkSettingPrompt"))
                .WithContent(I18nManager.GetString("SelectDeleteParkSetting"))
                .Dismiss().ByClickingBackground()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)
                .WithActionButton(I18nManager.GetString("Dismiss"), _ => { }, true)
                .TryShow();
            }
            else
            {
                _dialogManager.CreateDialog()
                .WithTitle(I18nManager.GetString("DeleteParkSetting"))
                .WithContent(I18nManager.GetString("SureWantToDeleteParkSetting"))
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
            var selectedIds = ParkSettings.Where(u => u.IsSelected).Select(u => u.Id);
            int result = await _parkSettingDao.DeleteRangeAsync<ParkSetting>(new List<int>(selectedIds));

            if (result > 0)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    //20250401, 删除成功也用 dialog 提示
                    dialog.Icon = Icons.Check;
                    dialog.IconColor = NotificationColor.SuccessIconForeground;
                    dialog.Title = I18nManager.GetString("DeleteParkSettingPrompt");
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
                                    CurrentModelType = typeof(ParkSetting),
                                    Title = I18nManager.GetString("DeleteParkSettingPrompt"),
                                    Content = I18nManager.GetString("DeleteSuccessfully"),
                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                    NeedRefreshData = true,
                                    NeedShowToast = false,
                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);


                    WeakReferenceMessenger.Default.Send<SelectedParkSettingMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN);
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
