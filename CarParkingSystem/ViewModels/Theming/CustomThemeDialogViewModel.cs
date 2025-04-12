using Avalonia.Media;
using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Models;
using SukiUI.Theme.Shadcn;
using System.Linq;

namespace CarParkingSystem.ViewModels.Theming;

public partial class CustomThemeDialogViewModel(SukiTheme theme, ISukiDialog dialog) : ObservableObject
{
    //[ObservableProperty] private string _displayName = "Pink";
    private string _displayName;

    public string DisplayName
    {
        get { return PrimaryColor.ToString();  }
        //set {
        //        SetProperty(ref _displayName, value);
        //}
    }


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    private Color _primaryColor = Colors.DeepPink;
    [ObservableProperty] private Color _accentColor = Colors.Pink;
    [ObservableProperty] private string _message;

    [RelayCommand]
    private void TryCreateTheme()
    {
        if (string.IsNullOrEmpty(DisplayName)) return;
        var theme1 = new SukiColorTheme(DisplayName, PrimaryColor, AccentColor);

        var themes = SukiTheme.GetInstance().ColorThemes.ToList();
        if (themes != null && !themes.Contains(theme1))
        {
            theme.AddColorTheme(theme1);
            theme.ChangeColorTheme(theme1);
            dialog.Dismiss();
        }
        else
        {
            Message = I18nManager.GetString("ThisColorThemeHasAlreadyBeenAdded");
        }
    }

    [RelayCommand]
    private void CloseDialog() => dialog.Dismiss();
}