using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CarParkingSystem.ViewModels;

/// <summary>
/// ���Ǹ� ParkWay(��������) ÿһ���ṩ���ݵ� ViewModel
/// </summary>
public partial class ParkWayTabViewModel : ViewModelValidatorBase
{
    [ObservableProperty] private string _displayName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayName">��ʾ��������ı���</param>
    public ParkWayTabViewModel(string displayName)
    {
        _displayName = displayName;
    }

}