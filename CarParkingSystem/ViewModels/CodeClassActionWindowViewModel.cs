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
using FluentAvalonia.Core;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RegularExpressionAttribute = CarParkingSystem.I18n.RegularExpressionAttribute;
using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为新增、修改用户信息界面提供数据的 ViewModel
    /// </summary>
    public partial class CodeClassActionWindowViewModel : ViewModelValidatorBase
    {
        private AppDbContext _appDbContext;
        private CodeClassDao _codeClassDao;
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }

        [ObservableProperty] private string _title;//窗口标题
        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）

        [ObservableProperty] private bool? _isAddCodeClass;// true=Add; false=Update
        [ObservableProperty] private bool _isBusy;
        //[ObservableProperty] private bool _isEnabled = true;
        [ObservableProperty] private CodeClass _selectedCodeClass;

        [ObservableProperty]
        private System.Int32 _id;
        [Required(StringResourceKey.ProjectNameCannoBeEmpty)]
        [ObservableProperty]
        private System.String _projectName = "CarParkingSystem";
        [Required(StringResourceKey.TableNameCannotBeEmpty)]
        [ObservableProperty]
        private System.String _tableName;
        [Required(StringResourceKey.ClassNameCannotBeEmpty)]
        [ObservableProperty]
        private System.String _className;
        //[Required(StringResourceKey.FieldsCannotBeEmpty)]
        [ObservableProperty]
        private string _fields;//字段字段的长字符串（需要进行解释）

        public CodeClassActionWindowViewModel(AppDbContext appDbContext, CodeClassDao codeClassDao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)
        {
            _appDbContext = appDbContext;
            _codeClassDao = codeClassDao;
            ToastManager = toastManager;
            DialogManager = dialogManager;
        }



        [ObservableProperty] private string _idValidationMessage;
        [ObservableProperty] private string _projectNameValidationMessage;
        [ObservableProperty] private string _tableNameValidationMessage;
        [ObservableProperty] private string _classNameValidationMessage;
        [ObservableProperty] private string _fieldsValidationMessage;


        public void ClearVertifyErrors()
        {
            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）
            //清除验证错误信息
            UpdateValidationMessage(nameof(ProjectName));
            UpdateValidationMessage(nameof(TableName));
            UpdateValidationMessage(nameof(ClassName));
            UpdateValidationMessage(nameof(Fields));
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
                    UpdateValidationMessage(nameof(ProjectName));
                    UpdateValidationMessage(nameof(TableName));
                    UpdateValidationMessage(nameof(ClassName));
                    //UpdateValidationMessage(nameof(Fields)); //更新不需要显示 Fields，因为可以点击“配置”按钮进行操作字段信息
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
                .WithTitle(I18nManager.GetString("UpdateCodeClassPrompt"))
                .WithContent(I18nManager.GetString("CodeClassExists"))
                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon
                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();
                                });
                return;
                }


                var tempCodeClass = _codeClassDao.GetById(SelectedCodeClass.Id);
                if (tempCodeClass != null)
                {
                    string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    tempCodeClass.ProjectName = ProjectName;
                    tempCodeClass.TableName = TableName;
                    tempCodeClass.ClassName = ClassName;
                    int result = _codeClassDao.Update(tempCodeClass);
                    if (result >= 0)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            WeakReferenceMessenger.Default.Send(
                                 new ToastMessage
                                 {
                                     CurrentModelType = typeof(CodeClass),
                                     Title = I18nManager.GetString("UpdateCodeClassPrompt"),
                                     Content = I18nManager.GetString("UpdateSuccessfully"),
                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                     NeedRefreshData = true
                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);

                            WeakReferenceMessenger.Default.Send("Close CodeClassActionWindow", TokenManage.CODE_CLASS_ACTION_WINDOW_CLOSE_TOKEN);
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
                    UpdateValidationMessage(nameof(ProjectName));
                    UpdateValidationMessage(nameof(TableName));
                    UpdateValidationMessage(nameof(ClassName));
                    UpdateValidationMessage(nameof(Fields)); 
                    IsBusy = false;
                    return;
                }

                //验证成功后，需要将之前的错误信息清空
                ClearErrors();
                //ProjectNameValidationMessage = string.Empty;
                //TableNameValidationMessage = string.Empty;
                //ClassNameValidationMessage = string.Empty;
                //FieldsValidationMessage = string.Empty;
                UpdateInfo = string.Empty;

                
                //对数据的唯一性进行验证，这里需要测试来修正
                var tempCodeClass = _codeClassDao.GetByTableName(TableName);
                if (tempCodeClass != null)
                {
                    TableNameValidationMessage = String.Format(I18nManager.GetString(Language.TableAlreadyExists), TableName);
                    return;
                }
                
                //string tempDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var result = _codeClassDao.Add(new CodeClass
                {
                    ProjectName = ProjectName,
                    TableName = TableName,
                    ClassName = ClassName,
                    CodeFields = GetCodeFields()
                });
                if (result > 0)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(
                            new ToastMessage
                            {
                                CurrentModelType = typeof(CodeClass),
                                Title = I18nManager.GetString("CreateCodeClassPrompt"),
                                Content = I18nManager.GetString("CreateSuccessfully"),
                                NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,
                                NeedRefreshData = true
                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);
                        WeakReferenceMessenger.Default.Send("Close CodeClassActionWindow", TokenManage.CODE_CLASS_ACTION_WINDOW_CLOSE_TOKEN);
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

        public List<CodeField> GetCodeFields()
        {
            List<CodeField> codeFields = new List<CodeField>();
            if (string.IsNullOrWhiteSpace(Fields)) return codeFields;

            if (IsNormalSqlStyleFields())
            {
                ParseNormalSqlStyleFields(codeFields);
            }
            else if (IsSqliteStyleFields())
            {
                ParseSqliteStyleFields(codeFields);
            }



            return codeFields;
        }

        /// <summary>
        /// 这种格式固定，5列：
        /// 字段名
        /// 数据类型
        /// 最大长度
        /// 是否允许为空
        /// 字段注释
        /// 
        /// 特点：包含 nvarchar, int 类型
        /// </summary>
        /// <returns></returns>
        public bool IsNormalSqlStyleFields()
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(Fields)
                && (Fields.Contains("nvarchar", StringComparison.OrdinalIgnoreCase)
                || Fields.Contains("int", StringComparison.OrdinalIgnoreCase)))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 特点：包含 TEXT, INTEGER 类型
        /// </summary>
        /// <returns></returns>
        public bool IsSqliteStyleFields()
        {
            bool result = false;

            if (!string.IsNullOrWhiteSpace(Fields)
                && (Fields.Contains("TEXT", StringComparison.OrdinalIgnoreCase)
                || Fields.Contains("INTEGER", StringComparison.OrdinalIgnoreCase)))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 解释 sqlite 字段语句的，如：
        /// Id INTEGER PRIMARY KEY AUTOINCREMENT,
        /// ProjectName TEXT NOT NULL,                 -- 项目名称
        /// TableName TEXT NOT NULL,             	 -- 表名
        /// ClassName TEXT NOT NULL		 -- 类名
        /// </summary>
        /// <param name="codeFields"></param>
        public void ParseSqliteStyleFields(List<CodeField> codeFields)
        {
            string[] array = Fields.Split('\n');
            int index = 0;
            foreach (string str in array)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    string[] row = str.Trim().Replace("\r", "").Replace("\t", "").Split(" ");//'\t'
                    if (row.Length >= 2)//==3
                    {
                        string fieldName = row[0];
                        string fieldType = row[1];
                        if (fieldType.EndsWith(","))
                        {
                            fieldType = fieldType.Substring(0, fieldType.Length - 1);
                        }

                        string fieldRemark = string.Empty;
                        string defaultValue = string.Empty;
                        bool isNoNull = false;
                        bool isMainKey = false;
                        bool isAutoIncrement = false;
                        bool isUnique = false;
                        if (str.Contains("--"))
                        {
                            fieldRemark = str.Split("--")[1].Trim();
                        }
                        if (str.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase))
                        {
                            isNoNull = true;
                        }
                        if (str.Contains("DEFAULT", StringComparison.OrdinalIgnoreCase))
                        {
                            //defaultValue = str.Split("DEFAULT")[1].Trim();
                            var tempIndex = row.IndexOf("DEFAULT") + 1;
                            defaultValue = row[tempIndex];
                            if (defaultValue.EndsWith(","))
                            {
                                defaultValue = defaultValue.Substring(0, defaultValue.Length - 1);
                            }
                        }
                        if (str.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
                        {
                            isMainKey = true;
                        }
                        if (str.Contains("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase))
                        {
                            isAutoIncrement = true;
                        }
                        if (str.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                        {
                            isUnique = true;
                        }

                        CodeField obj = new CodeField();
                        //obj.ClassId = entity.AysnId;  //对应类型
                        obj.FieldName = fieldName;  //字段名称
                        obj.FieldType = fieldType;  //字段类型
                                                   //obj.FieldLength = length;  //类型长度
                        obj.FieldRemark = fieldRemark;  //备注
                        obj.IsMainKey = isMainKey;  //是否为主键
                        obj.IsAutoIncrement = isAutoIncrement;
                        obj.IsAllowNull = !isNoNull;
                        obj.IsUnique = isUnique;
                        obj.DefaultValue = defaultValue;
                        //obj.IsRequired = true;//默认先全部打开
                        //codeClass.CodeFields.Add(obj);//每个字段添加一次
                        codeFields.Add(obj);//每个字段添加一次

                        index = index + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 解释普通 sql 字段语言，如：
        /// 字段名 数据类型 最大长度 是否允许为空 字段解释
        /// aysnid	  int	   NULL	    NO	       主键
        /// </summary>
        /// <param name="codeFields"></param>
        public void ParseNormalSqlStyleFields(List<CodeField> codeFields)
        {
            string[] array = Fields.Split('\n');
            int index = 0;
            foreach (string str in array)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    string[] row = str.Trim().Replace("\r", "").Split("\t");
                    if (row.Length == 5)//固定长度，都是 5
                    {
                        string fieldName = row[0];
                        string fieldType = row[1];
                        int fieldLenght = 0;
                        if (!row[2].Equals("NULL", StringComparison.OrdinalIgnoreCase))
                        {
                            fieldLenght = System.Convert.ToInt32(row[2]);  
                        }

                        string fieldRemark = row[4];

                        string defaultValue = string.Empty;
                        bool isNoNull = false;
                        bool isMainKey = false;
                        bool isAutoIncrement = false;
                        bool isUnique = false;
                        
                        if (row[3].Contains("NO", StringComparison.OrdinalIgnoreCase))
                        {
                            isNoNull = true;
                        }
                        if (fieldType.Contains("datetime", StringComparison.OrdinalIgnoreCase))
                        {
                            defaultValue = "(datetime('now','localtime'))";
                        }
                        if (fieldName.Contains("AysnId", StringComparison.OrdinalIgnoreCase)
                            || fieldRemark.Contains("主键", StringComparison.OrdinalIgnoreCase))
                        {
                            isMainKey = true;
                            isAutoIncrement = true;//是主键，就自增长
                        }
                        //if (str.Contains("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    isAutoIncrement = true;
                        //}
                        //if (str.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    isUnique = true;
                        //}

                        CodeField obj = new CodeField();
                        //obj.ClassId = entity.AysnId;  //对应类型
                        obj.FieldName = Functions.ConvertToSnakeCase(fieldName);  //字段名称
                        obj.FieldType = SqliteTypeConverter.GetSqliteType(fieldType);  //字段类型
                                                   //obj.FieldLength = length;  //类型长度
                        obj.FieldRemark = fieldRemark;  //备注
                        obj.IsMainKey = isMainKey;  //是否为主键
                        obj.IsAutoIncrement = isAutoIncrement;
                        obj.IsAllowNull = !isNoNull;
                        obj.IsUnique = isUnique;
                        obj.DefaultValue = defaultValue;
                        //obj.IsRequired = true;//默认先全部打开
                        //codeClass.CodeFields.Add(obj);//每个字段添加一次
                        codeFields.Add(obj);//每个字段添加一次

                        index = index + 1;
                    }
                }
            }
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.CODE_CLASS_ACTION_WINDOW_CLOSE_TOKEN);
        }

        #endregion
    }
}

