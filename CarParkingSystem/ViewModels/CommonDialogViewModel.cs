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

namespace CarParkingSystem.ViewModels;

public partial class CommonDialogViewModel(SukiTheme theme, ISukiDialog dialog) : ObservableObject
{
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _message;

    [RelayCommand]
    private void CloseDialog() => dialog.Dismiss();
}