using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CarParkingSystem.ViewModels;

/// <summary>
/// 这是给 ParkDevice 每一项提供数据的 ViewModel
/// </summary>
public partial class ParkDeviceTabViewModel : ViewModelValidatorBase
{
    [ObservableProperty] private string _displayName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayName">显示在左侧栏的标题</param>
    public ParkDeviceTabViewModel(string displayName)
    {
        _displayName = displayName;
    }

}