using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CarParkingSystem.Controls;
using CarParkingSystem.Dao;
using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CarParkingSystem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LinqKit;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CarParkingSystem.ViewModels.MainWindowViewModel;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 为更改角色信息界面提供数据的 ViewModel
    /// </summary>
    public partial class UpdateRoleInfoViewModel : DemoPageBase
    {
        public UpdateRoleInfoViewModel() : base("UpdateRoleInfo", MaterialIconKind.Administrator, pid: 2, id: 55, index: 55)
        {
        }

    }
}
