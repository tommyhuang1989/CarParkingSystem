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
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegularExpressionAttribute = CarParkingSystem.I18n.RegularExpressionAttribute;
using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为新增、修改用户信息界面提供数据的 ViewModel
    /// </summary>
    public partial class CodeFieldActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private CodeFieldDao _codeFieldDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddCodeField;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private CodeField _selectedCodeField;
        [ObservableProperty] private List<string> _sqliteTypes;

        [ObservableProperty]
        private System.Int32 _id;
        [ObservableProperty]
        private System.Int32 _cid;
        [ObservableProperty]
        private System.String _fieldName;
        [ObservableProperty]
        private System.String _fieldType;
        [ObservableProperty]
        private System.Int32 _fieldLength;
        [ObservableProperty]
        private System.String _fieldRemark;
        [ObservableProperty]
        private System.Boolean _isMainKey;
        [ObservableProperty]
        private System.Boolean _isAllowNull;
        [ObservableProperty]
        private System.Boolean _isAutoIncrement;
        [ObservableProperty]
        private System.Boolean _isUnique;
        [ObservableProperty]
        private System.String _defaultValue;

        public CodeFieldActionWindowViewModel(AppDbContext appDbContext, CodeFieldDao codeFieldDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _codeFieldDao = codeFieldDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;

            SqliteTypes = SqliteTypeConverter.GetSqliteTypes();
        }

        [ObservableProperty] private string _idValidationMessage; 
        [ObservableProperty] private string _fieldNameValidationMessage;
        [ObservableProperty] private string _fieldTypeValidationMessage;
        [ObservableProperty] private string _fieldLengthValidationMessage;
        [ObservableProperty] private string _fieldRemarkValidationMessage;
        [ObservableProperty] private string _isMainKeyValidationMessage;
        [ObservableProperty] private string _isAllowNullValidationMessage;
        [ObservableProperty] private string _isAutoIncrementValidationMessage;
        [ObservableProperty] private string _isUniqueValidationMessage;
        [ObservableProperty] private string _defaultValueValidationMessage;

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
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                var hasSameRecord = false;
                if ((bool)hasSameRecord)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        ToastManager.CreateToast()
                .WithTitle(I18nManager.GetString("UpdateCodeFieldPrompt"))
                .WithContent(I18nManager.GetString("CodeFieldExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }
                var tempCodeField = _codeFieldDao.GetById(SelectedCodeField.Id);
                if (tempCodeField != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempCodeField.Cid = Cid;
                    tempCodeField.FieldName = FieldName;
                    tempCodeField.FieldType = FieldType;
                    tempCodeField.FieldLength = FieldLength;
                    tempCodeField.FieldRemark = FieldRemark;
                    tempCodeField.IsMainKey = IsMainKey;
                    tempCodeField.IsAllowNull = IsAllowNull;
                    tempCodeField.IsAutoIncrement = IsAutoIncrement;
                    tempCodeField.IsUnique = IsUnique;
                    tempCodeField.DefaultValue = DefaultValue;
                    int result = _codeFieldDao.Update(tempCodeField);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {  
                                     CurrentModelType = typeof(CodeField),
                                     Title = I18nManager.GetString("UpdateCodeFieldPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close CodeFieldActionWindow", TokenManage.CODE_FIELD_ACTION_WINDOW_CLOSE_TOKEN);
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
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                UpdateInfo = string.Empty;

                
                //对数据的唯一性进行验证，这里需要测试来修正
                var tempCodeField = _codeFieldDao.GetByFieldName(FieldName);
                if (tempCodeField != null)
                {
                    FieldNameValidationMessage = String.Format(I18nManager.GetString(Language.FieldAlreadyExists), FieldName);
                    return;
                }
                
                //string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result = _codeFieldDao.Add(new CodeField {
                    Cid = Cid,
                    FieldName = FieldName,
                    FieldType = FieldType,
                    FieldLength = FieldLength,
                    FieldRemark = FieldRemark,
                    IsMainKey = IsMainKey,
                    IsAllowNull = IsAllowNull,
                    IsAutoIncrement = IsAutoIncrement,
                    IsUnique = IsUnique,
                    DefaultValue = DefaultValue,
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage
                            {
                                CurrentModelType = typeof(CodeField),
                                Title = I18nManager.GetString("CreateCodeFieldPrompt"),
                                Content = I18nManager.GetString("CreateSuccessfully"),
                                NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close CodeFieldActionWindow", TokenManage.CODE_FIELD_ACTION_WINDOW_CLOSE_TOKEN);
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
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.CODE_FIELD_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

