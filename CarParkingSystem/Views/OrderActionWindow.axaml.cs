using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CarParkingSystem.Messages;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;
using System;

namespace CarParkingSystem
{
public partial class OrderActionWindow : SukiWindow
{
    public OrderActionWindow()
    {
        InitializeComponent(); 
        
        WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.ORDER_ACTION_WINDOW_CLOSE_TOKEN, Recieve);
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
}

