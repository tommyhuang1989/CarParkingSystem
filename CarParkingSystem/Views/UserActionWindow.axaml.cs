using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;
using System;

namespace CarParkingSystem;

/// <summary>
/// 增加、修改用户信息的界面
/// </summary>
public partial class UserActionWindow : SukiWindow
{
    public UserActionWindow()
    {
        InitializeComponent(); 
        
        WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.USER_ACTION_WINDOW_CLOSE_TOKEN, Recieve);
    }

    private void Recieve(object recipient, string message)
    {
        WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(
                new MaskLayerMessage
                {
                    IsNeedShow = false,
                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);

        this.Close();
    }
}