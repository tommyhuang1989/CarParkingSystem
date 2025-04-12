using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class CarManagementViewModel : DemoPageBase
    {
        public CarManagementViewModel() : base(Language.CarManagement, MaterialIconKind.Car, pid: 0, id: 4, index: 4)
        {
        }
    }
}
