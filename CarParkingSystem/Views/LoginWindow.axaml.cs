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
/// ��¼����
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

    //�϶�����
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.Pointer.Type == PointerType.Mouse)
            BeginMoveDrag(e); // ֱ����Ӧ���ڲ㼶����갴���¼�
    }
}