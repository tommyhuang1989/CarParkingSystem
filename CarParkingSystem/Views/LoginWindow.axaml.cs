using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;
using System;

namespace CarParkingSystem;

/// <summary>
/// 登录界面
/// </summary>
public partial class LoginWindow : SukiWindow
{
    public LoginWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.LOGIN_WINDOW_CLOSE_TOKEN, (o, e) => {
            this.Close();
        });
    }

    //拖动窗口
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.Pointer.Type == PointerType.Mouse)
            BeginMoveDrag(e); // 直接响应窗口层级的鼠标按下事件
    }
}