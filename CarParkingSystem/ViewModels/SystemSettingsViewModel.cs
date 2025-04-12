using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class SystemSettingsViewModel : DemoPageBase
    {
        public SystemSettingsViewModel() : base(Language.SystemSettings, MaterialIconKind.SettingsApplications, 7, 40, 40)
        {
        }
    }
}
