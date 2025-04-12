using CarParkingSystem.Dao;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Vml.Office;
using FluentAvalonia.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public partial class CodeGenerateWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _projectName = "CarParkingSystem";

        [ObservableProperty]
        private string _tableName;

        [ObservableProperty]
        private string _className;

        [ObservableProperty]
        private string _fields;

        [ObservableProperty]
        private CodeField _curCodeField;

        [ObservableProperty]
        private string _message;


        private AppDbContext _appDbContext;
        public ObservableCollection<DemoPageBase> DemoPages { get; }
        public CodeGenerateWindowViewModel(IEnumerable<DemoPageBase> demoPages, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            DemoPages = new ObservableCollection<DemoPageBase>(demoPages);
            //DemoPages = DemoPageBase.GetSubPages(0, new ObservableCollection<DemoPageBase>(demoPages));
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                CodeClass codeClass = new CodeClass();
                codeClass.ProjectName = ProjectName;
                codeClass.TableName = TableName;
                codeClass.ClassName = ClassName;
                codeClass.Icon = Material.Icons.MaterialIconKind.Star;
                codeClass.Pid = 6;//
                var maxId = DemoPages.Max(x => x.Id);
                var maxIndex = DemoPages.Max(x => x.Index);
                codeClass.Id = maxId + 1;
                codeClass.Index = maxId + 1;

                string[] array = Fields.Split('\n');
                int index = 0;
                foreach (string str in array)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string[] row = str.Trim().Replace("\r", "").Replace("\t", "").Split(" ");//'\t'
                        if (row.Length >= 2)//==3
                        {
                            string filedName = row[0];
                            string fileType = row[1];
                            if (fileType.EndsWith(","))
                            {
                                fileType = fileType.Substring(0, fileType.Length - 1);
                            }

                            string fileRemark = string.Empty;
                            string defaultValue = string.Empty;
                            bool isNoNull = false;
                            bool isMainKey = false;
                            bool isAutoIncrement = false;
                            bool isUnique = false;
                            if (str.Contains("--"))
                            {
                                fileRemark = str.Split("--")[1].Trim();
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
                            obj.FieldName = filedName;  //字段名称
                            obj.FieldType = fileType;  //字段类型
                            //obj.FieldLength = length;  //类型长度
                            obj.FieldRemark = fileRemark;  //备注
                            obj.IsMainKey = isMainKey;  //是否为主键
                            obj.IsAutoIncrement = isAutoIncrement;
                            obj.IsAllowNull = !isNoNull;
                            obj.IsUnique = isUnique;
                            obj.DefaultValue = defaultValue;
                            //obj.IsRequired = true;//默认先全部打开
                            codeClass.CodeFields.Add(obj);//每个字段添加一次

                            index = index + 1;
                        }
                    }
                }

                var result = GenerateCodeHelper.Run(codeClass);
                if (result)
                {
                    WeakReferenceMessenger.Default.Send("Add new second menu", TokenManage.MAIN_WINDOW_REFRESH_MENU_TOKEN);

                    string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                    string filename = codeClass.ProjectName + "\\Sql\\Generate\\" + codeClass.TableName + ".sql";
                    string filePath = System.IO.Path.Combine(basePath, filename);
                    if (System.IO.File.Exists(filePath))
                    {
                        var sql = System.IO.File.ReadAllText(filePath);
                        _appDbContext.Database.ExecuteSqlRaw(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }

    #region 旧的逻辑,仅备份
    /*foreach (string str in array)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string[] row = str.Replace("\r", "").Split(" ");//'\t'
                        if (row.Length >= 2)//==3
                        {
                            string filedName = row[0];
                            string fileType = row[1];
                            string fileRemark = row[2];

                            int length = 0;
                            if (fileType.ToLower().IndexOf("nvarchar") != -1)
                            {
                                string lengtStr = fileType.ToLower().Replace("nvarchar", "").Replace("(", "").Replace(")", "");
                                length = Convert.ToInt32(lengtStr);
                                fileType = "nvarchar";
                            }

                            CodeField obj = new CodeField();
                            //obj.ClassId = entity.AysnId;  //对应类型
                            obj.FieldName = filedName;  //字段名称
                            obj.FieldType = fileType;  //字段类型
                            obj.FieldLength = length;  //类型长度
                            obj.FieldRemark = fileRemark;  //备注
                            obj.IsMainKey = index == 0;  //是否为主键
                            obj.IsAllowNull = length > 0 || fileType == "ntext";  //是否允许空
                            //CodeFields.Create(obj);
                            codeClass.CodeFields.Add(obj);//每个字段添加一次

                            index = index + 1;
                        }
                    }
                }*/
    #endregion
}
