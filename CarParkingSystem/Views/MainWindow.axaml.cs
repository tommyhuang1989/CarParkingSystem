using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CarParkingSystem.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Toasts;
using System;

namespace CarParkingSystem.Views;

/// <summary>
/// 主界面
/// </summary>
public partial class MainWindow : SukiWindow
{
    private DemoPageBase _currentPage;
    public MainWindow()
    {
        InitializeComponent();

        // User 和 Role 添加和更新后会关闭窗口，所以这个时候通知主窗口，让它来显示 Toast
        WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, (o, e) => {
            if (e.NeedShowToast)
            {
                this.toastManager.Manager.CreateToast()
                              .WithTitle(e.Title)
                              .WithContent(e.Content)
                              //.OfType(e.NotifyType)//20250402,不要 icon
                              .Dismiss().After(TimeSpan.FromSeconds(3))
                              .Dismiss().ByClicking()
                              .Queue();
            }
        });
    }


    private void ThemeMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (e.Source is not MenuItem mItem) return;
        if (mItem.DataContext is not SukiColorTheme cTheme) return;
        vm.ChangeTheme(cTheme);
    }

    private void BackgroundMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (e.Source is not MenuItem mItem) return;
        if (mItem.DataContext is not SukiBackgroundStyle cStyle) return;
        vm.BackgroundStyle = cStyle;
    }

    private void Image_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        IsMenuVisible = !IsMenuVisible;
    }

    private void MakeFullScreenPressed(object? sender, PointerPressedEventArgs e)
    {
        WindowState = WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
        IsTitleBarVisible = WindowState != WindowState.FullScreen;
    }

    private delegate void NoArgDelegate();

    public void SetActivePage(DemoPageBase curPage)
    {
        MainWindowViewModel mainVM = this.DataContext as MainWindowViewModel;
        mainVM.ActivePage = curPage;
    }

}