using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class SystemManagementViewModel : DemoPageBase
    {
        public SystemManagementViewModel() : base(Language.SystemManagement, MaterialIconKind.WritingSystemThai, pid: 0, id: 7, index: 7)
        {
        }
    }
}
