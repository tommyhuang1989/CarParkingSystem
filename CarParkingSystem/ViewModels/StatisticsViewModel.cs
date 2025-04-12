using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class StatisticsViewModel : DemoPageBase
    {
        public StatisticsViewModel() : base(Language.Statistics, MaterialIconKind.Summation, pid: 0, id: 6, index: 6)
        {
        }
    }
}
