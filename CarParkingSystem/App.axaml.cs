using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using CarParkingSystem.ViewModels;
using CarParkingSystem.Views;
using Avalonia.Controls;
using CarParkingSystem.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using Microsoft.EntityFrameworkCore;
using CarParkingSystem.Dao;
using System.Linq.Expressions;
using AvaloniaExtensions.Axaml.Markup;
using System.Globalization;
using System.Collections.Generic;
using CarParkingSystem.Models;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CarParkingSystem.Services;
using CarParkingSystem.ViewModels.Theming;
using Avalonia.Threading;
using System.Diagnostics;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using CarParkingSystem.Messages;

namespace CarParkingSystem;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; set; }
    public static ServiceCollection Services { get; set; }
    public static SukiViews Views { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    #region new logic
    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            I18nManager.Instance.Culture = new CultureInfo("zh-CN");//en

            //Global catching Exceptions
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;// for UI
            //AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;// for no UI

            DisableAvaloniaDataAnnotationValidation();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var services = new ServiceCollection();

                services.AddSingleton(desktop);

                //var views = ConfigureViews(services);
                //var provider = ConfigureServices(services);

                //App.ServiceProvider = provider;
                //App.Views = views;
                App.Services = services;//for global

                RefreshRequiredServices();//20250327, tommy, add

                DataTemplates.Add(new Common.ViewLocator(App.Views));
                //desktop.MainWindow = views.CreateView<MainWindowViewModel>(provider) as Window;
                desktop.MainWindow = App.Views.CreateView<LoginWindowViewModel>(App.ServiceProvider) as Window;
            }

            base.OnFrameworkInitializationCompleted();

            //    Shadcn.Configure(Application.Current, ThemeVariant.Dark);
            AppDomain.CurrentDomain.UnhandledException += HandleGlobalException;
            //AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }
        catch (Exception e)
        {
            ILogService logger = App.ServiceProvider.GetService<ILogService>();
            logger.LogError(Functions.GetExceptionMessage(e), I18nManager.GetString("AnExceptionOccurredDuringProgramOperation"));
            Environment.Exit(0);
        }
    }

    private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void HandleGlobalException(object sender, UnhandledExceptionEventArgs e)
    {
        ILogService logger = App.ServiceProvider.GetService<ILogService>();
        try
        {
            Exception exception = (Exception)e.ExceptionObject;
            logger.LogError(Functions.GetExceptionMessage(exception), "HandleGlobalException");
        }
        catch (Exception ex)
        {
            logger.LogError(Functions.GetExceptionMessage(ex), "HandleGlobalException");
        }
    }

    private void CurrentDomain_ProcessExit2(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            ILogService logger = App.ServiceProvider.GetService<ILogService>();

            Exception exception = (Exception)e.ExceptionObject;
            logger.LogFatal(Functions.GetExceptionMessage(exception), I18nManager.GetString("DomainUnhandledException"));

            WeakReferenceMessenger.Default.Send(
                               new ToastMessage
                               {
                                   Title = I18nManager.GetString("DomainUnhandledException"),
                                   Content = Functions.GetExceptionMessage(exception),
                                   NotifyType = Avalonia.Controls.Notifications.NotificationType.Error
                               }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
        }
        catch(Exception ex)
        {
            ILogService logger = App.ServiceProvider.GetService<ILogService>();
            logger.LogError(Functions.GetExceptionMessage(ex), I18nManager.GetString("DomainUnhandledException"));
        }
    }

    public static Control CreateViewByViewModel<TViewModel>()
        where TViewModel : ObservableObject
    {
        var view = App.Views.CreateView<TViewModel>(App.ServiceProvider);
        //if (view is Window window)
        //{
        //    window.Show();
        //}

        return view;
    }

    public static void RefreshRequiredServices()
    {
        if (App.Services == null) return;

        var views = ConfigureViews(App.Services);
        var provider = ConfigureServices(App.Services);

        App.ServiceProvider = provider;
        App.Views = views;
    }

    private static SukiViews ConfigureViews(ServiceCollection services)
    {
        return new SukiViews()

            // Add main view
            .AddView<MainWindow, MainWindowViewModel>(services)
            .AddView<LoginWindow, LoginWindowViewModel>(services)
            .AddView<UserActionWindow, UserActionWindowViewModel>(services)
            .AddView<RoleActionWindow, RoleActionWindowViewModel>(services)

            // Add pages
            // first menus
            .AddView<BasicInfomationView, BasicInfomationViewModel>(services)
            .AddView<UserManagementView, UserManagementViewModel>(services)
            .AddView<FeeManagementView, FeeManagementViewModel>(services)
            .AddView<CarManagementView, CarManagementViewModel>(services)
            .AddView<InAndOutRecordView, InAndOutRecordViewModel>(services)
            .AddView<StatisticsView, StatisticsViewModel>(services)
            .AddView<SystemManagementView, SystemManagementViewModel>(services)

            // Normal pages
            .AddView<UserView, UserViewModel>(services)
.AddView<FeeRuleSectionView, FeeRuleSectionViewModel>(services)

.AddView<FeeRuleSectionActionWindow, FeeRuleSectionActionWindowViewModel>(services)

.AddView<ValueCarView, ValueCarViewModel>(services)

.AddView<ValueCarActionWindow, ValueCarActionWindowViewModel>(services)

.AddView<LongTermRentalCarView, LongTermRentalCarViewModel>(services)

.AddView<LongTermRentalCarActionWindow, LongTermRentalCarActionWindowViewModel>(services)

.AddView<ValueCardTypeView, ValueCardTypeViewModel>(services)

.AddView<ValueCardTypeActionWindow, ValueCardTypeActionWindowViewModel>(services)

.AddView<LongTermRentalCardTypeView, LongTermRentalCardTypeViewModel>(services)

.AddView<LongTermRentalCardTypeActionWindow, LongTermRentalCardTypeActionWindowViewModel>(services)

.AddView<ValueCardActionView, ValueCardActionViewModel>(services)

.AddView<ValueCardActionActionWindow, ValueCardActionActionWindowViewModel>(services)

.AddView<DelayCardActionView, DelayCardActionViewModel>(services)

.AddView<DelayCardActionActionWindow, DelayCardActionActionWindowViewModel>(services)

.AddView<FeeRuleView, FeeRuleViewModel>(services)

.AddView<FeeRuleActionWindow, FeeRuleActionWindowViewModel>(services)

.AddView<BlackCarView, BlackCarViewModel>(services)

.AddView<BlackCarActionWindow, BlackCarActionWindowViewModel>(services)

.AddView<CarFreeView, CarFreeViewModel>(services)

.AddView<CarFreeActionWindow, CarFreeActionWindowViewModel>(services)

.AddView<ParkingAbnormalView, ParkingAbnormalViewModel>(services)
.AddView<ParkingAbnormalDetailsWindow, ParkingAbnormalDetailsWindowViewModel>(services)

.AddView<ParkingAbnormalActionWindow, ParkingAbnormalActionWindowViewModel>(services)

.AddView<ParkingOutRecordView, ParkingOutRecordViewModel>(services)

.AddView<ParkingOutRecordActionWindow, ParkingOutRecordActionWindowViewModel>(services)

.AddView<ParkingInRecordView, ParkingInRecordViewModel>(services)

.AddView<ParkingInRecordActionWindow, ParkingInRecordActionWindowViewModel>(services)

.AddView<OpenGateReasonView, OpenGateReasonViewModel>(services)

.AddView<OpenGateReasonActionWindow, OpenGateReasonActionWindowViewModel>(services)

.AddView<HandOverView, HandOverViewModel>(services)

.AddView<HandOverActionWindow, HandOverActionWindowViewModel>(services)

.AddView<UserWayView, UserWayViewModel>(services)

.AddView<UserWayActionWindow, UserWayActionWindowViewModel>(services)

.AddView<OpenGateRecordView, OpenGateRecordViewModel>(services)

.AddView<OpenGateRecordActionWindow, OpenGateRecordActionWindowViewModel>(services)

.AddView<CalendarView, CalendarViewModel>(services)

.AddView<CalendarActionWindow, CalendarActionWindowViewModel>(services)


.AddView<CarTypeParaView, CarTypeParaViewModel>(services)

.AddView<CarTypeParaActionWindow, CarTypeParaActionWindowViewModel>(services)

.AddView<CarConvertView, CarConvertViewModel>(services)

.AddView<CarConvertActionWindow, CarConvertActionWindowViewModel>(services)

.AddView<ParkSettingCardView, ParkSettingCardViewModel>(services)

.AddView<ParkSettingCardActionWindow, ParkSettingCardActionWindowViewModel>(services)

.AddView<CarVisitorView, CarVisitorViewModel>(services)

.AddView<CarVisitorActionWindow, CarVisitorActionWindowViewModel>(services)

.AddView<ParkingArrearsView, ParkingArrearsViewModel>(services)

.AddView<ParkingArrearsActionWindow, ParkingArrearsActionWindowViewModel>(services)

.AddView<OrderRefundView, OrderRefundViewModel>(services)

.AddView<OrderRefundActionWindow, OrderRefundActionWindowViewModel>(services)

.AddView<OrderView, OrderViewModel>(services)

.AddView<OrderActionWindow, OrderActionWindowViewModel>(services)


.AddView<SumView, SumViewModel>(services)

.AddView<SumActionWindow, SumActionWindowViewModel>(services)

.AddView<ParkAreaView, ParkAreaViewModel>(services)

.AddView<ParkAreaActionWindow, ParkAreaActionWindowViewModel>(services)

//不需要，这是 Tab 的
//.AddView<ParkInfoView, ParkInfoViewModel>(services)
//.AddView<ParkInfoActionWindow, ParkInfoActionWindowViewModel>(services)

            //车场设置 TabControl
            .AddView<ParkSettingsMenuView, ParkSettingsMenuViewModel>(services)
            .AddView<PlatformSettingsView, PlatformSettingsViewModel>(services)
            .AddView<ParkSettingView, ParkSettingViewModel>(services)
            .AddView<CarSettingsView, CarSettingsViewModel>(services)

            //车道管理 TabControl
            .AddView<ParkWayMenuView, ParkWayMenuViewModel>(services)
.AddView<ParkWayStopTimeView, ParkWayStopTimeViewModel>(services)
.AddView<ParkWayStopTimeActionWindow, ParkWayStopTimeActionWindowViewModel>(services)

            //车辆管理 TabControl
            .AddView<LongTermRentalCarMenuView, LongTermRentalCarMenuViewModel>(services)
            .AddView<ValueCarMenuView, ValueCarMenuViewModel>(services)

            //设备管理 TabControl
            .AddView<ParkDeviceMenuView, ParkDeviceMenuViewModel>(services)
            .AddView<LicensePlateRecognitionView, LicensePlateRecognitionViewModel>(services)
            .AddView<LicensePlateRecognitionActionWindow, LicensePlateRecognitionActionWindowViewModel>(services)
            .AddView<CardSwipeDeviceView, CardSwipeDeviceViewModel>(services)
            .AddView<CardSwipeDeviceActionWindow, CardSwipeDeviceActionWindowViewModel>(services)

.AddView<ParkWayGroupView, ParkWayGroupViewModel>(services)
.AddView<ParkWayGroupActionWindow, ParkWayGroupActionWindowViewModel>(services)

.AddView<ParkWayCardView, ParkWayCardViewModel>(services)
.AddView<ParkWayCardActionWindow, ParkWayCardActionWindowViewModel>(services)

.AddView<ParkWayView, ParkWayViewModel>(services)
.AddView<ParkWayActionWindow, ParkWayActionWindowViewModel>(services)

.AddView<ParkWayVoiceView, ParkWayVoiceViewModel>(services)
.AddView<ParkWayVoiceActionWindow, ParkWayVoiceActionWindowViewModel>(services)

            .AddView<CodeClassView, CodeClassViewModel>(services)
            .AddView<CodeClassActionWindow, CodeClassActionWindowViewModel>(services)
            .AddView<CodeFieldView, CodeFieldViewModel>(services)
            .AddView<CodeFieldActionWindow, CodeFieldActionWindowViewModel>(services)

            //7.System Management
            .AddView<RoleView, RoleViewModel>(services)
            .AddView<PermissionConfigurationView, PermissionConfigurationViewModel>(services)
            .AddView<CodeGenerateWindow, CodeGenerateWindowViewModel>(services)
            .AddView<SystemSettingsView, SystemSettingsViewModel>(services)

            .AddView<UpdateRoleInfoView, UpdateRoleInfoViewModel>(services)
            .AddView<ChangeUserPasswordView, ChangeUserPasswordViewModel>(services)
            .AddView<CustomThemeDialogView, CustomThemeDialogViewModel>(services)
            .AddView<CreateFirstFloorWindow, CreateFirstFloorWindowViewModel>(services);
    }

    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        //services.AddSingleton<ClipboardService>();
        //services.AddSingleton<PageNavigationService>();
        services.TryAddSingleton<AppDbContext, AppDbContext>();
        //services.AddSingleton<ISukiToastManager, SukiToastManager>();
        //services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.TryAddTransient<ISukiToastManager, SukiToastManager>();
        services.TryAddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.TryAddSingleton<ILogService, LogService>();
        services.TryAddSingleton<ThemingViewModel, ThemingViewModel>();

        services.TryAddSingleton<UserDao, UserDao>();
services.TryAddSingleton<FeeRuleSectionDao, FeeRuleSectionDao>();

services.TryAddSingleton<ValueCarDao, ValueCarDao>();

services.TryAddSingleton<LongTermRentalCarDao, LongTermRentalCarDao>();

services.TryAddSingleton<ValueCardTypeDao, ValueCardTypeDao>();

services.TryAddSingleton<LongTermRentalCardTypeDao, LongTermRentalCardTypeDao>();

services.TryAddSingleton<ValueCardActionDao, ValueCardActionDao>();

services.TryAddSingleton<DelayCardActionDao, DelayCardActionDao>();

services.TryAddSingleton<FeeRuleDao, FeeRuleDao>();

services.TryAddSingleton<BlackCarDao, BlackCarDao>();

services.TryAddSingleton<CarFreeDao, CarFreeDao>();

services.TryAddSingleton<ParkingAbnormalDao, ParkingAbnormalDao>();

services.TryAddSingleton<ParkingOutRecordDao, ParkingOutRecordDao>();

services.TryAddSingleton<ParkingInRecordDao, ParkingInRecordDao>();

services.TryAddSingleton<ParkWayStopTimeDao, ParkWayStopTimeDao>();

services.TryAddSingleton<ParkWayGroupDao, ParkWayGroupDao>();

services.TryAddSingleton<ParkWayCardDao, ParkWayCardDao>();

services.TryAddSingleton<ParkWayDao, ParkWayDao>();

services.TryAddSingleton<ParkWayVoiceDao, ParkWayVoiceDao>();

services.TryAddSingleton<ParkSettingDao, ParkSettingDao>();

services.TryAddSingleton<ParkDeviceDao, ParkDeviceDao>();

services.TryAddSingleton<OpenGateReasonDao, OpenGateReasonDao>();

services.TryAddSingleton<HandOverDao, HandOverDao>();

services.TryAddSingleton<UserWayDao, UserWayDao>();

services.TryAddSingleton<OpenGateRecordDao, OpenGateRecordDao>();

services.TryAddSingleton<CalendarDao, CalendarDao>();

services.TryAddSingleton<CarTypeParaDao, CarTypeParaDao>();

services.TryAddSingleton<CarConvertDao, CarConvertDao>();

services.TryAddSingleton<ParkSettingCardDao, ParkSettingCardDao>();

services.TryAddSingleton<CarVisitorDao, CarVisitorDao>();

services.TryAddSingleton<ParkingArrearsDao, ParkingArrearsDao>();

services.TryAddSingleton<OrderRefundDao, OrderRefundDao>();

services.TryAddSingleton<OrderDao, OrderDao>();


services.TryAddSingleton<SumDao, SumDao>();

services.TryAddSingleton<ParkAreaDao, ParkAreaDao>();

services.TryAddSingleton<ParkInfoDao, ParkInfoDao>();

        services.TryAddSingleton<CodeClassDao, CodeClassDao>();
        services.TryAddSingleton<CodeFieldDao, CodeFieldDao>();
        services.TryAddSingleton<RoleDao, RoleDao>();

        return services.BuildServiceProvider();
    }

    #endregion
}