using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Metadata;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Controls;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CarParkingSystem.Views;
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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CarParkingSystem.ViewModels.MainWindowViewModel;
using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为修改用户密码界面提供数据的 ViewModel
    /// </summary>
    public partial class ChangeUserPasswordViewModel : DemoPageBase
    {
        [Required(StringResourceKey.OldPasswordRequired)]
        [ObservableProperty] 
        private string _oldPassword;
        [Required(StringResourceKey.NewPasswordRequired)]
        [CustomValidation(typeof(ChangeUserPasswordViewModel), nameof(ValidateNewOldPassword))]
        [ObservableProperty] 
        private string _newPassword;
        [Required(StringResourceKey.RepeatPasswordRequired)]
        [CustomValidation(typeof(ChangeUserPasswordViewModel), nameof(ValidateConfirmPassword))]
        [ObservableProperty] 
        private string _confirmPassword;
        [ObservableProperty] private string _oldPasswordValidationMessage;
        [ObservableProperty] private string _newPasswordValidationMessage;
        [ObservableProperty] private string _confirmPasswordValidationMessage;
        [ObservableProperty] private string _updateInfo;


        public ISukiToastManager ToastManager { get; }
        public MainWindowViewModel MainVM { get; }
        public UserDao _userDao { get; }

        //pid: 2, id: 5, index: 5
        public ChangeUserPasswordViewModel(UserDao userDao, ISukiToastManager toastManager) : base("ChangeRolePassword", MaterialIconKind.Mace, pid: 2, id: 55, index: 55)
        {
            //MainVM = mainVM;
            _userDao = userDao;
            ToastManager = toastManager;
        }

        public static ValidationResult ValidateNewOldPassword(string value, ValidationContext context)
        {
            var instance = (ChangeUserPasswordViewModel)context.ObjectInstance;
            return value.Equals(instance.OldPassword)
                ? new ValidationResult(I18nManager.GetString("NewPasswordCannotBeSameWithOldPassword")) : ValidationResult.Success;
        }

        public static ValidationResult ValidateConfirmPassword(string value, ValidationContext context)
        {
            var instance = (ChangeUserPasswordViewModel)context.ObjectInstance;
            return value == instance.NewPassword
                ? ValidationResult.Success
                : new ValidationResult(I18nManager.GetString("PasswordsNotMatch"));
        }

        [RelayCommand]
        private Task Save()
        {
            return Task.Run(async () =>
            {
                ValidateAllProperties();
                //前端验证
                if (HasErrors)
                {
                    UpdateValidationMessage(nameof(OldPassword));
                    UpdateValidationMessage(nameof(NewPassword));
                    UpdateValidationMessage(nameof(ConfirmPassword));
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;
                OldPasswordValidationMessage = string.Empty;
                NewPasswordValidationMessage = string.Empty;
                ConfirmPasswordValidationMessage = string.Empty;

                var mainVM = App.ServiceProvider.GetService<MainWindowViewModel>();

                //后端验证
                var tempUser = _userDao.GetByUsername(mainVM.CurrentUser.Username);
                bool verify_success = false;
                //old password, 需要正确才行
                verify_success = PasswordHelper.VerifyPassword(OldPassword, tempUser.Password, tempUser.Salt);

                if (verify_success)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //如果密码直接等于数据库中加密的数据，说明用户没有修改密码，此时就不需要重新加密
                    //只有跟原来的不一样才进行加密，这样就不会导致用户只是修改了其他字段，但是密码却也被更改了
                    //if (!tempUser.Password.Equals(NewPassword))
                    //{
                        string tempSalt = PasswordHelper.GenerateSalt();
                        string tempPassword = PasswordHelper.HashPassword(NewPassword, tempSalt);
                        tempUser.Salt = tempSalt;
                        tempUser.Password = tempPassword;
                    //}

                    tempUser.UpdatedAt = tempDt;

                    //int result = _appDbContext.SaveChanges();
                    int result = _userDao.Update(tempUser);//返回数量为影响的记录条数

                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     Title = I18nManager.GetString("UpdateUserPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                        });
                    }
                    else
                    {
                        UpdateInfo = I18nManager.GetString("UpdateFailed");
                    }
                    await Task.Delay(2000);
                    //UpdateInfo = "";

                }
                else
                {
                    OldPasswordValidationMessage = I18nManager.GetString("IncorrectOldPassord"); 
                }
            });
        }
    }
}
