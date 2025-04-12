using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;

using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using System.Linq;
using CarParkingSystem.ViewModels;

namespace CarParkingSystem.Views;

/// <summary>
/// 主界面的 View
/// </summary>
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Image_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
    }

    private void Expander_Expanded(object sender, EventArgs e)
    {
        var expander = sender as Expander;
        // 处理展开逻辑，例如更新界面或数据
        Console.WriteLine("Expander 已展开");
    }

    private void TreeView_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        var a = sender as TreeView;
        var c = a.GetLogicalChildren() as List<TreeViewItem>;
        var b = (e.Source as Control).Parent;
        var d = 123;
        //foreach (var tvi in (sender as TreeView).GetVisualDescendants().OfType<TreeViewItem>())
        //{
        //    if (nvi.Tag != tvi.Tag)
        //    {
        //        tvi.IsExpanded = false;
        //    }
        //    if (nvi != tvi)
        //    {
        //        tvi.IsExpanded = false;
        //    }
        //}
    }

    private void TreeView_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var treeView = sender as TreeView;
        var curPageVM = treeView.SelectedItem as DemoPageBase; ;
        var tvis = treeView.GetVisualDescendants().OfType<TreeViewItem>();
        if (curPageVM != null)//20250327, tommy, add
        {
            foreach (var treeViewItem in tvis)
            {
                var pageVM = treeViewItem.DataContext as DemoPageBase;
                //只需要判断一级菜单
                if (pageVM.SubPages.Count > 0)
                {
                    //1.不等，就是其他一级菜单；（点击其他一级菜单）
                    if (pageVM != curPageVM)
                    {
                        //2.不等，同时也不是该一级菜单的子菜单（点击）
                        if (pageVM.Id != curPageVM.Pid)
                        {
                            treeViewItem.IsExpanded = false;
                        }
                        else//3.点击自己的二级菜单
                        {
                            treeViewItem.IsExpanded = true;
                        }
                    }
                    else //点击自己时，切换状态（实现单击展开/收起）
                    {
                        treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
                    }
                }
                //else // 二级菜单，关联其父类 一级菜单
                //{

                //}
            }
        }
    }

}