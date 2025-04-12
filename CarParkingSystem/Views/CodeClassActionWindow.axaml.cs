using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.Messaging;
using SukiUI.Controls;

namespace CarParkingSystem;

public partial class CodeClassActionWindow : SukiWindow
{
    public CodeClassActionWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.CODE_CLASS_ACTION_WINDOW_CLOSE_TOKEN, Recieve);
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