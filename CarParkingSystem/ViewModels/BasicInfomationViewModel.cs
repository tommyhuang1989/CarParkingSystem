using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class BasicInfomationViewModel : DemoPageBase
    {
        public BasicInfomationViewModel() : base(Language.BasicInformation, MaterialIconKind.Information, pid: 0, id: 1, index: 1)
        {
        }
    }
}
