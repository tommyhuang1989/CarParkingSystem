using CarParkingSystem.I18n;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 只是对应左侧菜单栏“设备”的二级菜单, 所以增加 Menu 关键字
    /// </summary>
    public partial class ParkDeviceMenuViewModel : DemoPageBase
    {
        [ObservableProperty] ObservableCollection<ParkDeviceTabViewModel> _parkTabPages;

        private ParkDeviceTabViewModel? _activePage;

        public ParkDeviceTabViewModel? ActivePage
        {
            get { return _activePage; }
            set
            {
                if (_activePage != value)
                {
                    SetProperty(ref _activePage, value);
                }
            }
        }
        public ParkDeviceMenuViewModel(IEnumerable<ParkDeviceTabViewModel> parkTabPages) : base(Language.ParkDeviceManagement, MaterialIconKind.Settings, pid: 1, id: 11, index: 11)
        {
            ParkTabPages = new ObservableCollection<ParkDeviceTabViewModel>(parkTabPages);
            ActivePage = ParkTabPages?.FirstOrDefault();//默认选中第一个
        }
    }
}
