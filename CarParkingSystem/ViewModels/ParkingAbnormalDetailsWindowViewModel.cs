using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public partial class ParkingAbnormalDetailsWindowViewModel : ViewModelValidatorBase
    {
        [ObservableProperty]
        private ParkingAbnormal _selectedParkingAbnormal;

        [RelayCommand]
        private void Close()
        {
            //通过发布消息来实现右上角的关闭按钮
            WeakReferenceMessenger.Default.Send<string, string>("Close", TokenManage.PARKING_ABNORMAL_DETAILS_WINDOW_CLOSE_TOKEN);
        }
    }
}
