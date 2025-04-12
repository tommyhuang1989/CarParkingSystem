using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    public class LanguageInfo
    {
        /// <summary>
        /// eg: cn (匹配国旗时会用到)
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// eg: zh-CN (切换多语言翻译时会用的)
        /// </summary>
        public string Name { get; set; }
        public LanguageInfo(string displayName, string name)
        {
            DisplayName = displayName;
            Name = name;
        }
    }
}
