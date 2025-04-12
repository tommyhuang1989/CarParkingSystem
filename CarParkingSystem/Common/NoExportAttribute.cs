using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Common
{
    /// <summary>
    /// 在导出 excel 时，用来标注哪些属性不导出
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class NoExportAttribute : Attribute { }
}
