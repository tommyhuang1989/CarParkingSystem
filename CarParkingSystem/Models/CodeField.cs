using CarParkingSystem.Common;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    public partial class CodeField : BaseEntity
    {
        // 导航属性
        public CodeClass CodeClass { get; set; }
        /// <summary>
        /// 关联的 Class 的 Id
        /// </summary>
        public int Cid { get; set; }
        /// <summary>
        /// 字段名称, 正常为小写带下划线，如：hello_world
        /// </summary>
        public string FieldName { get; set; }

        private string fieldNamePascalCase;

        /// <summary>
        /// 因为 FieldName 是小写带下划线的，
        /// FieldNamePascalCase 返回如：HelloWorld
        /// </summary>
        public string FieldNamePascalCase
        {
            get { 
                return Functions.ConvertToPascalCase(FieldName); 
            }
        }
        private string fieldNameCamelCase;

        /// <summary>
        /// 因为 FieldName 是小写带下划线的，
        /// FieldNameCamelCase 返回如：helloWorld
        /// </summary>
        public string FieldNameCamelCase
        {
            get
            {
                return Functions.ConvertToCamelCase(FieldName);
            }
        }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }
        /// <summary>
        /// 类型长度
        /// </summary>
        public int FieldLength { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string FieldRemark { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsMainKey { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsAllowNull { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsAutoIncrement { get; set; }
        /// <summary>
        /// 是否唯一
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否必须（在新增、更改界面显示红色 *）
        /// </summary>
        //[NotMapped]
        //public bool IsRequired { get; set; }
        private bool isRequired = true;//20250331, 默认 IsRequired 全部打开
        [NotMapped]
        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; }
        }
        /// <summary>
        /// 是否支持查询
        /// </summary>
        [NotMapped]
        public bool IsSupportSearch { get; set; } 
        
        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCodeFieldMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
        }
#endregion
    }
}
