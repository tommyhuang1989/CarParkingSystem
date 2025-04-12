﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace CarParkingSystem.Common;

/// <summary>
/// 视图加载器，更加 ViewModel 找到对应的 View，并生成实例
/// </summary>
/// <param name="views"></param>
public class ViewLocator(SukiViews views) : IDataTemplate
{
    private readonly Dictionary<object, Control> _controlCache = [];

    public Control Build(object? param)
    {
        if (param is null)
        {
            return CreateText("Data is null.");
        }

        if (_controlCache.TryGetValue(param, out var control))
        {
            return control;
        }

        if (views.TryCreateView(param, out var view))
        {
            _controlCache.Add(param, view);

            return view;
        }

        return CreateText($"No View For {param.GetType().Name}.");
    }

    public bool Match(object? data) => data is ObservableObject;

    private static TextBlock CreateText(string text) => new TextBlock { Text = text };
}