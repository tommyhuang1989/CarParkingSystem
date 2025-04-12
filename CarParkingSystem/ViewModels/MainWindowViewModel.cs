using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Common;
using CarParkingSystem.Dao;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Services;
using CarParkingSystem.Unities;
using CarParkingSystem.ViewModels.Theming;
using CarParkingSystem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Theme.Shadcn;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为主界面提供数据的 ViewModel
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        //用来接收修改用户信息的通知
        public record UpdateMessage(string msg);
        public record CloseMessage(string msg);

        [ObservableProperty] private string _updateInfo;
        /// <summary>
        /// 将会切换到的 Theme
        /// </summary>
        private ThemeVariant _willChangeTheme;

        public ThemeVariant WillChangeTheme
        {
            //get { return _willChangeTheme; }
            //set { 
            //    _willChangeTheme = value; 
            //}

            get { return BaseTheme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; }
        }


        [NotifyPropertyChangedFor(nameof(WillChangeTheme))]
        [ObservableProperty] 
        private ThemeVariant _baseTheme;//显示当前的 Theme
        [ObservableProperty] private bool _windowLocked;
        [ObservableProperty] private bool _titleBarVisible = true;
        [ObservableProperty] private SukiBackgroundStyle _backgroundStyle = SukiBackgroundStyle.GradientSoft;
        [ObservableProperty] private bool _animationsEnabled;
        [ObservableProperty] private string? _customShaderFile;
        [ObservableProperty] private bool _transitionsEnabled;
        [ObservableProperty] private double _transitionTime;

        [ObservableProperty] private bool _showTitleBar = true;
        [ObservableProperty] private bool _showBottomBar = true;
        [ObservableProperty] private bool _isOpenPane = true;
        [ObservableProperty] private bool _isBusy = false;
        [ObservableProperty] private User _currentUser;

        private readonly SukiTheme _theme;
        private readonly ThemingViewModel _theming;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }
        public IAvaloniaReadOnlyList<SukiBackgroundStyle> BackgroundStyles { get; }
        public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }

        //public IAvaloniaReadOnlyList<DemoPageBase> DemoPages { get; }
        [ObservableProperty] ObservableCollection<DemoPageBase> _demoPages;//20250327, add
        //public ObservableCollection<DemoPageBase> DemoPages { get; set; }//20250325, add set;
        //[ObservableProperty] private DemoPageBase? _activePage;
        private DemoPageBase? _activePage;

        public DemoPageBase? ActivePage
        {
            get { return _activePage; }
            set {
                if (_activePage != value)
                {
                    // 有二级菜单时，其一级菜单不需要显示内容，所以不给 ActivePage 赋值
                    if (value != null && value.SubPages.Count == 0) { 
                        SetProperty(ref _activePage, value);
                    }
                }
            }
        }


        public MainWindowViewModel(IEnumerable<DemoPageBase> demoPages, ThemingViewModel themingViewModel, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _theming = themingViewModel;
            _theming.BackgroundStyleChanged += style =>  BackgroundStyle = style;
            _theming.BackgroundAnimationsChanged += enabled => AnimationsEnabled = enabled;
            _theming.CustomBackgroundStyleChanged += shader => CustomShaderFile = shader;
            _theming.BackgroundTransitionsChanged += enabled => TransitionsEnabled = enabled;

            BackgroundStyles = new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
            _theme = SukiTheme.GetInstance();

            ToastManager = toastManager;
            DialogManager = dialogManager;

            DemoPages = DemoPageBase.GetSubPages(0, new ObservableCollection<DemoPageBase>(demoPages));
            //ActivePage = DemoPages?.FirstOrDefault();//默认选中第一个
            ActivePage = demoPages?.FirstOrDefault(x => x.Id == 8);//Parking Settings
            //ActivePage = demoPages?.FirstOrDefault(x => x.Id == 5);//change password//test

            //DemoPages = new AvaloniaList<DemoPageBase>(demoPages);//原来只有一级菜单的情况
            //DemoPages = new AvaloniaList<DemoPageBase>(demoPages.OrderBy(x => x.Index).ThenBy(x => x.DisplayName));//进行排序的情况

            //WeakReferenceMessenger.Default.Register<UpdateMessage>(this, Recive);
            Themes = _theme.ColorThemes;
            BaseTheme = _theme.ActiveBaseTheme;
            //WillChangeTheme = BaseTheme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;

            // Subscribe to the base theme changed events
            _theme.OnBaseThemeChanged += variant =>
            {
                BaseTheme = variant;
                ToastManager.CreateSimpleInfoToast()
                    .WithTitle(I18nManager.GetString("ThemeChanged"))
                    .WithContent(String.Format(I18nManager.GetString("ThemeHasChangedTo"),variant))
                    .Queue();
            };

            // Subscribe to the color theme changed events
            _theme.OnColorThemeChanged += theme => ToastManager.CreateSimpleInfoToast()
                .WithTitle(I18nManager.GetString("ColorChanged"))
                .WithContent(String.Format(I18nManager.GetString("ColorHasChangedTo"), theme.DisplayName))
                .Queue();


            WeakReferenceMessenger.Default.Register<MaskLayerMessage, string>(this, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN, ReciveForMaskLayer);
            WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.MAIN_WINDOW_REFRESH_MENU_TOKEN, ReciveForRefreshMenu);
            WeakReferenceMessenger.Default.Register<ConfigureCodeFieldMessage, string>(this, TokenManage.SHOW_CODE_FIELD_TOKEN, ReciveForConfigureCodeField);
        }

        private void ReciveForConfigureCodeField(object recipient, ConfigureCodeFieldMessage message)
        {
            //var codeFieldVM = (CodeFieldViewModel)DemoPages.FirstOrDefault(x => x is CodeFieldViewModel);
            var codeFieldVM = (CodeFieldViewModel)DemoPages
                                .FirstOrDefault(p => p.Id == 7)?
                                .SubPages?
                                .FirstOrDefault(sp => sp is CodeFieldViewModel);
            codeFieldVM.Cid = message.Cid;
            codeFieldVM.CurrentPageIndex = 1;//20250408,从第一页开始获取，避免上一个表获取了第二页，但这次查询的表结构只有一页，而导致返回的数据为空
            codeFieldVM.RefreshData();
            ActivePage = codeFieldVM;
        }

        private void ReciveForRefreshMenu(object recipient, string message)
        {
            //var demoPageBases = App.ServiceProvider.GetService<IEnumerable<DemoPageBase>>();
            //DemoPages = DemoPageBase.GetSubPages(0, new ObservableCollection<DemoPageBase>(demoPageBases));

            //Console.WriteLine("page count is:" + demoPageBases.Count());


            //App.Views.AddView(GetTypeByName("CarParkingSystem.Views", message + "View"), GetTypeByName("CarParkingSystem.ViewModels", message + "ViewModel"), App.Services);


            //App.Views.AddView(GetTypeByName("CarParkingSystem.Views", message + "View"), GetInstanseByName("CarParkingSystem.ViewModels", message + "ViewModel"));
            //AddViewDynamically("CarParkingSystem.Views." + message + "View", "CarParkingSystem.ViewModels."+ message + "ViewModel", App.Services);


            //Thread.Sleep(3000);

            //var viewModelType = GetTypeByName("CarParkingSystem.ViewModels", message + "ViewModel");
            //if (viewModelType.IsAssignableTo(typeof(DemoPageBase)))
            //{
            //    //这里需要判断, 不能简单的过滤重复的类型
            //    //services.AddSingleton(typeof(DemoPageBase), viewModelType);
            //    //实际类型不能相同，所以需要判断是否已经存在
            //    var a = App.Services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            //    if (a == null)
            //    {
            //        App.Services.AddSingleton(typeof(DemoPageBase), viewModelType);
            //    }
            //}
            //else
            //{
            //    App.Services.TryAddSingleton(viewModelType);
            //}
            //App.ServiceProvider = App.Services.BuildServiceProvider();

            //App.RefreshRequiredServices();


            var demoPageBases2 = App.ServiceProvider.GetService<IEnumerable<DemoPageBase>>();
            DemoPages = DemoPageBase.GetSubPages(0, new ObservableCollection<DemoPageBase>(demoPageBases2));
            //Console.WriteLine("page count is:" + demoPageBases.Count());
        }

        // 动态调用示例
        public static void AddViewDynamically(string viewTypeName, string viewModelTypeName, ServiceCollection services)
        {
            // 1. 加载类型
            Type viewType = Type.GetType(viewTypeName) ?? throw new ArgumentException("Invalid TView type");
            Type viewModelType = Type.GetType(viewModelTypeName) ?? throw new ArgumentException("Invalid TViewModel type");

            // 2. 获取泛型方法
            MethodInfo addViewMethod = typeof(SukiViews).GetMethod("AddView");
            MethodInfo genericAddView = addViewMethod.MakeGenericMethod(viewType, viewModelType);

            // 3. 调用方法
            SukiViews sukiViews = new SukiViews();
            genericAddView.Invoke(sukiViews, new object[] { services });
        }

        public Type GetTypeByName(string WhichNamespace, string typeName)
        {
            string typeFullName = WhichNamespace+"."+typeName;// "命名空间.MyClass"; // 需包含完整命名空间
            Type type = Type.GetType(typeFullName);
            //object instance = Activator.CreateInstance(type); // 无参构造

            // 或带参数的构造
            //object[] args = new object[] { "参数1", 2 };
            //instance = Activator.CreateInstance(type, args);
            //return instance;

            return type;
        }
        public object GetInstanseByName(string WhichNamespace, string typeName)
        {
            string typeFullName = WhichNamespace + "." + typeName;// "命名空间.MyClass"; // 需包含完整命名空间
            Type type = Type.GetType(typeFullName);
            object instance = Activator.CreateInstance(type); // 无参构造

            // 或带参数的构造
            //object[] args = new object[] { "参数1", 2 };
            //instance = Activator.CreateInstance(type, args);
            //return instance;

            return instance;
        }


        private void ReciveForMaskLayer(object recipient, MaskLayerMessage message)
        {
            if (message.IsNeedShow)
            {
                IsBusy = true;
            }
            else
            {
                IsBusy = false;
            }
        }

        private void Recive(object recipient, UpdateMessage message)
        {
            //ToastManager.CreateToast()
            //            .WithTitle("更新用户信息提示")
            //            .WithContent(message)
            //            .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
            //            .Dismiss().After(TimeSpan.FromSeconds(3))
            //            .Dismiss().ByClicking()
            //            .Queue();

            UpdateInfo = message.msg;
        }


        public void ChangeTheme(SukiColorTheme theme) =>
            _theme.ChangeColorTheme(theme);

        [RelayCommand]
        private void CreateCustomTheme()
        {
            DialogManager.CreateDialog()
                .WithViewModel(dialog => new CustomThemeDialogViewModel(_theme, dialog))
                .WithActionButton("Dismiss", _ => { }, true)
                .TryShow();
        }
        [RelayCommand]
        private void ShowCodeField()
        {
            //var actionVM = App.ServiceProvider.GetService<CodeFieldViewModel>();
            //App.ServiceProvider.GetService<ChangeUserPasswordViewModel>();
            //var p = App.ServiceProvider.GetService<CodeFieldViewModel>();
            var codeFieldVM = DemoPages.FirstOrDefault(x => x is CodeFieldViewModel);
            ActivePage = codeFieldVM;
        }

        [RelayCommand]
        private void TreeViewItemExpand(DemoPageBase pageBase)
        {
            pageBase.IsExpanded = !pageBase.IsExpanded;
            foreach (var page in DemoPages)
            {
                if (page.Id != pageBase.Id)
                {
                    page.IsExpanded = !pageBase.IsExpanded;;
                }
            }
        }

        [RelayCommand]
        private void TogglePane()
        {
            IsOpenPane = !IsOpenPane;
        }

        [RelayCommand]
        private void TreeClick(object page)
        {
            ActivePage = (page as DemoPageBase);
            //var a = App.ServiceProvider.GetService<ChangeUserPasswordViewModel>();
            //ActivePage = a;
        }

        [RelayCommand]
        private void ChangeUserPassword()
        {
            //ActivePage = new ChangeUserPasswordViewModel();
            //var a = App.ServiceProvider.GetService<ChangeUserPasswordViewModel>();
            //ActivePage = a;
        }
        [RelayCommand]
        private void UpdateRoleInfo()
        {
            ActivePage = new UpdateRoleInfoViewModel();
            //var b = App.ServiceProvider.GetService<UpdateRoleInfoViewModel>();
            //ActivePage = b;
        }

        [RelayCommand]
        private void ShadCnMode()
        {
            SukiColorTheme whiteTheme = new SukiColorTheme("White", new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
            SukiColorTheme blackTheme = new SukiColorTheme("Black", new Color(byte.MaxValue, 0, 0, 0), new Color(byte.MaxValue, 0, 0, 0));

            var themes = SukiTheme.GetInstance().ColorThemes.ToList();
            if (themes != null && !themes.Contains(whiteTheme) && !themes.Contains(blackTheme))
            {
                Shadcn.Configure(Application.Current, Application.Current.ActualThemeVariant);
            }
            else
            {
                ToastManager.CreateSimpleInfoToast()
                .WithTitle(I18nManager.GetString("ShadcnModeInformation"))
                .WithContent(I18nManager.GetString("ThisColorThemeHasAlreadyBeenAdded"))
                .OfType(Avalonia.Controls.Notifications.NotificationType.Warning)
                .Queue();
            }
        }


        [RelayCommand]
        private void ToggleBaseTheme() =>
        _theme.SwitchBaseTheme();

        [RelayCommand]
        private void ToggleWindowLock()
        {
            WindowLocked = !WindowLocked;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle(WindowLocked ? I18nManager.GetString("Locked") : I18nManager.GetString("Unlocked"))
                .WithContent(WindowLocked ? I18nManager.GetString("WindowHasBeenLocked") : I18nManager.GetString("WindowHasBeenUnlocked"))
                .Queue();
        }
        [RelayCommand]
        private void ToggleAnimations()
        {
            AnimationsEnabled = !AnimationsEnabled;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle(AnimationsEnabled ? I18nManager.GetString("AnimationEnabled") : I18nManager.GetString("AnimationDisabled"))
                .WithContent(AnimationsEnabled ? I18nManager.GetString("BackgroundAnimationsAreNowEnabled") : I18nManager.GetString("BackgroundAnimationsAreNowDisabled"))
                .Queue();
        }

        [RelayCommand]
        private void ToggleTransitions()
        {
            TransitionsEnabled = !TransitionsEnabled;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle(TransitionsEnabled ? I18nManager.GetString("TransitionsEnabled") : I18nManager.GetString("TransitionsDisabled"))
                .WithContent(TransitionsEnabled ? I18nManager.GetString("BackgroundTransitionsAreNowEnabled") : I18nManager.GetString("BackgroundTransitionsAreNowDisabled"))
                .Queue();
        }

        [RelayCommand]
        private void ToggleTitleBackground()
        {
            ShowTitleBar = !ShowTitleBar;
            ShowBottomBar = !ShowBottomBar;
        }

        [RelayCommand]
        private void ToggleTitleBar()
        {
            TitleBarVisible = !TitleBarVisible;
            ToastManager.CreateSimpleInfoToast()
                .WithTitle(TitleBarVisible ? I18nManager.GetString("TitleBarVisible") : I18nManager.GetString("TitleBarHidden"))
                .WithContent(TitleBarVisible ? I18nManager.GetString("WindowTitleBarHasBeenShown") : I18nManager.GetString("WindowTitleBarHasBeenHidden"))
                .Queue();
        }

        [RelayCommand]
        private void ToggleRightToLeft() => _theme.IsRightToLeft = !_theme.IsRightToLeft;


        [RelayCommand]
        private void OpenCreate()
        {
            var createWindow = App.Views.CreateView<CodeGenerateWindowViewModel>(App.ServiceProvider) as Window;
            createWindow.Show();
        }

        [RelayCommand]
        private void CreateFirstFloor()
        {
            var createWindow = App.Views.CreateView<CreateFirstFloorWindowViewModel>(App.ServiceProvider) as Window;
            createWindow.Show();
        }

        [RelayCommand]
        private void ChangeFile()
        {
            //Functions.ReadFileWithStringBuilder("Messages", "SelectedUserMessage.cs");
            //Functions.ReadFileWithStringBuilder("Models", "User.cs");
            //Functions.ReadFileWithStringBuilder("Dao", "UserDao.cs");
            //Functions.ReadFileWithStringBuilder("Views", "UserActionWindow.axaml");
            //Functions.ReadFileWithStringBuilder("Views", "UserActionWindow.axaml.cs");
            //Functions.ReadFileWithStringBuilder("ViewModels", "UserActionWindowViewModel.cs");
            //Functions.ReadFileWithStringBuilder("Views", "UserView.axaml");
            //Functions.ReadFileWithStringBuilder("Views", "UserView.axaml.cs");
            //Functions.ReadFileWithStringBuilder("ViewModels", "UserViewModel.cs");


            Functions.ReadFileWithStringBuilder("Views", "RoleManagementView.axaml");
            Functions.ReadFileWithStringBuilder("Views", "RoleManagementView.axaml.cs");
            Functions.ReadFileWithStringBuilder("ViewModels", "RoleManagementViewModel.cs");
        }

    }
}
