using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class FeeManagementViewModel : DemoPageBase
    {
        public FeeManagementViewModel() : base(Language.FeeManagement, MaterialIconKind.MagenDavid, pid: 0, id: 3, index: 3)
        {
        }
    }
}
