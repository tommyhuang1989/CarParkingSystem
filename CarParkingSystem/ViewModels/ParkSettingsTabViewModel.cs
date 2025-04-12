using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CarParkingSystem.ViewModels;

/// <summary>
/// ���Ǹ� ParkSettings ÿһ���ṩ���ݵ� ViewModel
/// </summary>
public partial class ParkSettingsTabViewModel : ViewModelValidatorBase
{
    [ObservableProperty] private string _displayName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayName">��ʾ��������ı���</param>
    public ParkSettingsTabViewModel(string displayName)
    {
        _displayName = displayName;
    }

}