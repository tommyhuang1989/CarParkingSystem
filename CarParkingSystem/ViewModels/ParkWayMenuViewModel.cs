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
    /// 只是对应左侧菜单栏“车道设置”的二级菜单, 所以增加 Menu 关键字
    /// </summary>
    public partial class ParkWayMenuViewModel : DemoPageBase
    {
        [ObservableProperty] ObservableCollection<ParkWayTabViewModel> _parkTabPages;

        private ParkWayTabViewModel? _activePage;

        public ParkWayTabViewModel? ActivePage
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
        public ParkWayMenuViewModel(IEnumerable<ParkWayTabViewModel> parkTabPages) : base(Language.ParkWayManagement, MaterialIconKind.Settings, pid: 1, id: 10, index: 10)
        {
            ParkTabPages = new ObservableCollection<ParkWayTabViewModel>(parkTabPages);
            ActivePage = ParkTabPages?.FirstOrDefault();//默认选中第一个
        }
    }
}
