using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    /// <summary>
    /// 用于记录生成一级菜单代码所需基本信息，包括：
    /// 1.表名
    /// 2.类型
    /// 3.字段信息
    /// </summary>
    public class CodeClassFirstFloor
    {
        public string ProjectName { get; set; }
        public string ClassName { get; set; }
        public string DisplayName { get; set; }
        public MaterialIconKind Icon { get; set; }
        public int Id { get; set; }
        public int Pid { get; set; }
        public int Index { get; set; }
    }
}
