using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class PermissionConfigurationViewModel : DemoPageBase
    {
        public PermissionConfigurationViewModel() : base(Language.PermissionConfiguration, MaterialIconKind.Settings, 7, 38, 38)
        {
        }
    }
}
