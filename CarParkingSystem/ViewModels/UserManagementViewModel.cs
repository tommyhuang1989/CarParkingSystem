using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class UserManagementViewModel : DemoPageBase
    {
        public UserManagementViewModel() : base(Language.UserManagement, MaterialIconKind.User, pid: 0, id: 2, index: 2)
        {
        }
    }
}
