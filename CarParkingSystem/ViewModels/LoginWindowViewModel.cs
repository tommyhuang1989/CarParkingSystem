using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Common;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CarParkingSystem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为注册界面提供数据的 ViewModel
    /// </summary>
    public partial class LoginWindowViewModel : ViewModelValidatorBase
    {
        #region 属性
        [ObservableProperty] private bool _isLoggingIn;

        [I18n.Required(StringResourceKey.UsernameRequired)]
        //[CustomValidation(typeof)]
        [ObservableProperty] 
        private string _username;

        [I18n.Required(StringResourceKey.PasswordRequired)]
        [ObservableProperty] 
        private string _password;

        [ObservableProperty] private bool _isRevealPassword = false;

        [ObservableProperty] private string _usernameValidationMessage;
        [ObservableProperty] private string _passwordValidationMessage;
        [ObservableProperty] private string _loginInfo;

        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }
        public MainWindowViewModel MainVM { get; }
        public UserDao _userDao { get; }
        public List<LanguageInfo> Languages { get; set; }
        #endregion

        //, ISukiToastManager toastManager, ISukiDialogManager dialogManager
        public LoginWindowViewModel(MainWindowViewModel mainVM, UserDao userDao)
        {
            MainVM = mainVM;
            _userDao = userDao;

            ToastManager = new SukiToastManager();//区分于主界面的 Toast，如果共用好像会报错
            DialogManager = new SukiDialogManager(); ;

            Languages = new List<LanguageInfo>
            { 
                new LanguageInfo("us", "en"),
                new LanguageInfo("cn", "zh-CN"),
                new LanguageInfo("jp", "ja-JP"),
            };
        }

        #region 命令
        [RelayCommand]
        private void Close()
        {
            //通过发布消息来实现右上角的关闭按钮
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.LOGIN_WINDOW_CLOSE_TOKEN);
        }


        partial void OnUsernameChanged(string value)
        {
            ValidateProperty(value, nameof(Username));
            UpdateValidationMessage(nameof(Username));
        }

        partial void OnPasswordChanged(string value)
        {
            ValidateProperty(value, nameof(Password));
            UpdateValidationMessage(nameof(Password));
        }

        [RelayCommand]
        private Task Login(Window window)
        {
            IsLoggingIn = true;
            return Task.Run(async () =>
            {
                await Task.Delay(500);

                ValidateAllProperties();
                //前端验证
                if (HasErrors)
                {
                    //只能更改 Email 和 Phone
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(Password));
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UsernameValidationMessage = string.Empty;
                PasswordValidationMessage = string.Empty;
                LoginInfo = string.Empty;

                Dispatcher.UIThread.Invoke(() =>
                {
                    var user = _userDao.GetByUsername(Username);

                    if (user == null)//test, realy for ==
                    {
                        UsernameValidationMessage = I18nManager.GetString("UsernameNotExists");
                    }
                    else
                    {
                        bool verify_success = false;
                        verify_success = PasswordHelper.VerifyPassword(Password, user.Password, user.Salt);

                        if (verify_success)
                        {
                            ToastManager.CreateToast()
                            .WithTitle(I18nManager.GetString("LoginPrompt"))
                            .WithContent(I18nManager.GetString("LoginSuccessfully"))
                            //.OfType(Avalonia.Controls.Notifications.NotificationType.Success)//20250402,不要 icon
                            .Dismiss().After(TimeSpan.FromSeconds(3))
                            .Dismiss().ByClicking()
                            .Queue();



                            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                            {
                                desktop.MainWindow = App.Views.CreateView<MainWindowViewModel>(App.ServiceProvider) as Window;
                                MainVM.CurrentUser = new User { Username = Username, Password = Password};//用于后期修改密码
                                desktop.MainWindow.Show();

                                //发布登录成功的消息
                                WeakReferenceMessenger.Default.Send<string, string>("Login Success", TokenManage.LOGIN_WINDOW_CLOSE_TOKEN);
                            }
                        }
                        else
                        {
                            //ToastManager.CreateToast()
                            //.WithTitle("登录提示")
                            //.WithContent("用户名或密码不正确")
                            //.OfType(Avalonia.Controls.Notifications.NotificationType.Warning)
                            //.Dismiss().After(TimeSpan.FromSeconds(3))
                            //.Dismiss().ByClicking()
                            //.Queue();

                            //LoginInfo = "登录失败";
                            PasswordValidationMessage = I18nManager.GetString("IncorrectPassword");
                        }
                    }
                });

                IsLoggingIn = false;
            });
        }


        [RelayCommand]
        private void RevealPassword(Window window)
        {
            IsRevealPassword = !IsRevealPassword;
        }

        [RelayCommand]
        private void ChangeLanguage(LanguageInfo language)
        {
            try
            {
                I18nManager.Instance.Culture = new CultureInfo(language.Name);
            }
            catch (Exception)
            {
                I18nManager.Instance.Culture = new CultureInfo("en");
            }
        }

        [RelayCommand]
        private void Chinese(Window window)
        {
            //string abc = I18nManager.GetString(Language.InputUsername);
            I18nManager.Instance.Culture = new CultureInfo("zh-CN"); 
            //abc = I18nManager.GetString(Language.InputUsername);
        }

        [RelayCommand]
        private void English(Window window)
        {
            I18nManager.Instance.Culture = new CultureInfo("en");
        }

        [RelayCommand]
        private void Japanese(Window window)
        {
            I18nManager.Instance.Culture = new CultureInfo("ja-JP");
        }
        #endregion
    }
}
