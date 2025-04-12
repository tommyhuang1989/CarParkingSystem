using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Dao;
using CarParkingSystem.I18n;
using CarParkingSystem.Messages;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;
using System.Threading.Tasks;
using RegularExpressionAttribute = CarParkingSystem.I18n.RegularExpressionAttribute;
using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为新增、修改用户信息界面提供数据的 ViewModel
    /// </summary>
    public partial class UserActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private UserDao _userDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddUser;// true=AddUser; false=UpdateUser
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private User _selectedUser;

        //[Required(ErrorMessage = "用户名不能为空")]
        [Required(StringResourceKey.UsernameRequired)]
        //[MinLength(6)]
        //[MaxLength(50)]
        [ObservableProperty] 
        private string _username;

        [Required(StringResourceKey.PasswordRequired)]
        //[MinLength(6)]
        //[MaxLength(80)]
        [ObservableProperty] 
        private string _password;

        [Required(StringResourceKey.EmailRequired)]
        //[EmailAddress(ErrorMessage = "邮箱格式不对")]
        [RegularExpression(StringResourceKey.IncorrectEmailFormat, @"^[\w!#$%&'*+/=?^_{|}~-]+(?:\.[\w!#$%&'*+/=?^_{|}~-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$")]
        [ObservableProperty] 
        private string _email;

        [Required(StringResourceKey.PhoneRequired)]
        [RegularExpression(StringResourceKey.IncorrectPhoneFormat ,@"^1[3-9]\d{9}$")]
        [ObservableProperty] 
        private string _phone;


        [ObservableProperty] private string _usernameValidationMessage;
        [ObservableProperty] private string _passwordValidationMessage;
        [ObservableProperty] private string _emailValidationMessage;
        [ObservableProperty] private string _phoneValidationMessage;

        //[ObservableProperty] private ActionPageType _actionPageType;
        //[ObservableProperty] private bool? _dialogResult;

        public UserActionWindowViewModel(AppDbContext appDbContext, UserDao userDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _userDao = userDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }

        #region OnPropertyChanged 事件(用来实时验证)


        //partial void OnUsernameChanged(string value)
        //{
        //    ValidateProperty(value, nameof(Username));
        //    UpdateValidationMessage(nameof(Username));
        //}

        //partial void OnPasswordChanged(string value)
        //{
        //    ValidateProperty(value, nameof(Password));
        //    UpdateValidationMessage(nameof(Password));
        //}
        //partial void OnEmailChanged(string value)
        //{
        //    ValidateProperty(value, nameof(Email));
        //    UpdateValidationMessage(nameof(Email));
        //}
        //partial void OnPhoneChanged(string value)
        //{
        //    ValidateProperty(value, nameof(Phone));
        //    UpdateValidationMessage(nameof(Phone));
        //}
        #endregion

        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(Username));
            UpdateValidationMessage(nameof(Password));
            UpdateValidationMessage(nameof(Email));
            UpdateValidationMessage(nameof(Phone));
        }

        #region 命令
        /// <summary>
        /// 更新时，先判断：
        /// 1.该用户是否存在
        /// 2.修改后的用户信息会不会跟已经存在的信息冲突
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private Task Update()
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
                    //只能更改 Email 和 Phone
                    //UpdateValidationMessage(nameof(Username));
                    //UpdateValidationMessage(nameof(Password));
                    UpdateValidationMessage(nameof(Email));
                    UpdateValidationMessage(nameof(Phone)); 
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                var hasSameRecord = _userDao.HasSameRecord(SelectedUser.Id, Username, Email);
                if ((bool)hasSameRecord)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        //UpdateInfo = "更新信息失败";
                        ToastManager.CreateToast()
                .WithTitle(I18nManager.GetString("UpdateUserPrompt"))
                .WithContent(I18nManager.GetString("UsernameExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                    });

                    return;
                }

                //后端验证
                var tempUser = _userDao.GetById(SelectedUser.Id);
                if (tempUser != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //如果密码直接等于数据库中加密的数据，说明用户没有修改密码，此时就不需要重新加密
                    //只有跟原来的不一样才进行加密，这样就不会导致用户只是修改了其他字段，但是密码却也被更改了
                    if (!tempUser.Password.Equals(Password))
                    {
                        string tempSalt = PasswordHelper.GenerateSalt();
                        string tempPassword = PasswordHelper.HashPassword(Password, tempSalt);
                        tempUser.Password = tempPassword;
                    }

                    tempUser.Username = Username;
                    tempUser.Email = Email;
                    tempUser.Phone = Phone;//.Replace(" ", "");
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
                                     CurrentModelType = typeof(User),
                                     Title = I18nManager.GetString("UpdateUserPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close UserActionWindow", TokenManage.USER_ACTION_WINDOW_CLOSE_TOKEN);
                        });
                    }
                    else
                    {
                        var message = I18nManager.GetString("UpdateFailed");
                        UpdateInfo = message;
                    }
                    await Task.Delay(2000);

                    IsBusy = false;
                }
            });
        }

        [RelayCommand]
        private Task Add()
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
                    UpdateValidationMessage(nameof(Username));
                    UpdateValidationMessage(nameof(Password));
                    UpdateValidationMessage(nameof(Email));
                    UpdateValidationMessage(nameof(Phone));
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                //后端验证
                var tempUser = _userDao.GetByUsername(Username);
                if (tempUser != null)
                {
                    UsernameValidationMessage = I18nManager.GetString("UsernameExists");
                    return;
                }

                tempUser = _userDao.GetByEmail(Email);
                if (tempUser != null)
                {
                    EmailValidationMessage = I18nManager.GetString("EmailExists");
                    return;
                }

                //符合要求的新用户，可以插入
                string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string tempSalt = PasswordHelper.GenerateSalt();
                string tempPassword = PasswordHelper.HashPassword(Password, tempSalt);
                var result = _userDao.Add(new User
                {
                    Username = Username,
                    Password = tempPassword,
                    Email = Email,
                    Phone = Phone,//.Replace(" ", ""),
                    Status = 1,
                    Salt = tempSalt,
                    CreatedAt = tempDt,
                    UpdatedAt = tempDt,
                    LastLoginTime = "0000-00-00 00:00:00"
                });//测试使用 AddUser

                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage
                            {
                                CurrentModelType = typeof(User),
                                Title = I18nManager.GetString("CreateUserPrompt"),
                                Content= I18nManager.GetString("CreateSuccessfully"),
                                NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                        WeakReferenceMessenger.Default.Send("Close UserActionWindow", TokenManage.USER_ACTION_WINDOW_CLOSE_TOKEN);
                    });
                }
                else
                {
                    UpdateInfo = I18nManager.GetString("CreateFailed");
                }
                await Task.Delay(2000);

                IsBusy = false;
            });
        }

        [RelayCommand]
        private void Close()
        {
            //通过发布消息来实现右上角的关闭按钮
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.USER_ACTION_WINDOW_CLOSE_TOKEN);
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

        #endregion
    }
}
