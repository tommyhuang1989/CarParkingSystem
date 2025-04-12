using CarParkingSystem.Common;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    /// <summary>
    /// 用于记录生成代码所需基本信息，包括：
    /// 1.表名
    /// 2.类型
    /// 3.字段信息
    /// </summary>
    public partial class CodeClass : BaseEntity
    {
        //public int Id { get; set; }
        public string ProjectName { get; set; }
        public string TableName { get; set; }
        public string ClassName { get; set; }

        [NotMapped]
        public string Title { get; set; }

        [NotMapped]
        public MaterialIconKind Icon { get; set; }

        [NotMapped]
        public int Pid { get; set; }

        [NotMapped]
        public int Index { get; set; }

        [NotMapped]
        public List<CodeField> CodeFields { get; set; } = new();
        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCodeClassMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CODE_CLASS_TOKEN);
        }
        #endregion 
    }
}
