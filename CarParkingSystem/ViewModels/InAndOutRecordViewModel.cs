using CarParkingSystem.I18n;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public class InAndOutRecordViewModel : DemoPageBase
    {
        public InAndOutRecordViewModel() : base(Language.InAndOutRecords, MaterialIconKind.Record, pid: 0, id: 5, index: 5)
        {
        }
    }
}
