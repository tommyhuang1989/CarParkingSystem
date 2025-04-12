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
    public partial class ValueCarMenuViewModel : DemoPageBase
    {
        [ObservableProperty] ObservableCollection<ValueCarTabViewModel> _parkTabPages;

        private ValueCarTabViewModel? _activePage;

        public ValueCarTabViewModel? ActivePage
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
        public ValueCarMenuViewModel(IEnumerable<ValueCarTabViewModel> parkTabPages) : base(Language.RechargeCar, MaterialIconKind.Settings, pid: 4, id: 21, index: 21)
        {
            ParkTabPages = new ObservableCollection<ValueCarTabViewModel>(parkTabPages);
            ActivePage = ParkTabPages?.FirstOrDefault();//默认选中第一个
        }
    }
}
