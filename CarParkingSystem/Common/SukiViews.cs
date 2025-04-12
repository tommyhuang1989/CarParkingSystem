using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CarParkingSystem.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CarParkingSystem.Common;

/// <summary>
/// 用来绑定 ViewModel 和 View 的
/// </summary>
public class SukiViews
{
    private readonly Dictionary<Type, Type> _vmToViewMap = [];

    public SukiViews AddView<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(ServiceCollection services)
        where TView : ContentControl
        where TViewModel : ObservableObject
    {
        var viewType = typeof(TView);
        var viewModelType = typeof(TViewModel);

        _vmToViewMap.Add(viewModelType, viewType);

        if (viewModelType.IsAssignableTo(typeof(DemoPageBase)))
        {
            //这里需要判断, 不能简单的过滤重复的类型
            //services.AddSingleton(typeof(DemoPageBase), viewModelType);
            //实际类型不能相同，所以需要判断是否已经存在
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if(a == null){
                services.AddSingleton(typeof(DemoPageBase), viewModelType);
            }
        }
        else if (viewModelType.IsAssignableTo(typeof(ParkSettingsTabViewModel)))//20250407, add
        {
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(ParkSettingsTabViewModel), viewModelType);
            }
        }
        else if (viewModelType.IsAssignableTo(typeof(ParkWayTabViewModel)))//20250408, add
        {
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(ParkWayTabViewModel), viewModelType);
            }
        }
        else if (viewModelType.IsAssignableTo(typeof(LongTermRentalCarTabViewModel)))//20250408, add
        {
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(LongTermRentalCarTabViewModel), viewModelType);
            }
        }
        else if (viewModelType.IsAssignableTo(typeof(ValueCarTabViewModel)))//20250408, add
        {
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(ValueCarTabViewModel), viewModelType);
            }
        }
        else if (viewModelType.IsAssignableTo(typeof(ParkDeviceTabViewModel)))//20250409, add
        {
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(ParkDeviceTabViewModel), viewModelType);
            }
        }
        else
        {
            services.TryAddSingleton(viewModelType);
        }

        return this;
    }

    public SukiViews AddView(Type viewType, Type viewModelType, ServiceCollection services)
    {
        _vmToViewMap.Add(viewModelType, viewType);

        if (viewModelType.IsAssignableTo(typeof(DemoPageBase)))
        {
            //这里需要判断, 不能简单的过滤重复的类型
            //services.AddSingleton(typeof(DemoPageBase), viewModelType);
            //实际类型不能相同，所以需要判断是否已经存在
            var a = services.FirstOrDefault(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(viewModelType));
            if (a == null)
            {
                services.AddSingleton(typeof(DemoPageBase), viewModelType);
            }
        }
        else
        {
            services.TryAddSingleton(viewModelType);
        }
        return this;
    }

    public bool TryCreateView(IServiceProvider provider, Type viewModelType, [NotNullWhen(true)] out Control? view)
    {
        var viewModel = provider.GetRequiredService(viewModelType);

        return TryCreateView(viewModel, out view);
    }

    public bool TryCreateView(object? viewModel, [NotNullWhen(true)] out Control? view)
    {
        view = null;

        if (viewModel == null)
        {
            return false;
        }

        var viewModelType = viewModel.GetType();

        if (_vmToViewMap.TryGetValue(viewModelType, out var viewType))
        {
            view = Activator.CreateInstance(viewType) as Control;

            if (view != null)
            {
                view.DataContext = viewModel;
            }
        }

        return view != null;
    }

    public Control CreateView<TViewModel>(IServiceProvider provider) where TViewModel : ObservableObject
    {
        var viewModelType = typeof(TViewModel);

        if (TryCreateView(provider, viewModelType, out var view))
        {
            return view;
        }

        throw new InvalidOperationException();
    }

}