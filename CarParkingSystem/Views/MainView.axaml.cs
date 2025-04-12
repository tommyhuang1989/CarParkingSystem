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
/// ������� View
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
        // ����չ���߼���������½��������
        Console.WriteLine("Expander ��չ��");
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
                //ֻ��Ҫ�ж�һ���˵�
                if (pageVM.SubPages.Count > 0)
                {
                    //1.���ȣ���������һ���˵������������һ���˵���
                    if (pageVM != curPageVM)
                    {
                        //2.���ȣ�ͬʱҲ���Ǹ�һ���˵����Ӳ˵��������
                        if (pageVM.Id != curPageVM.Pid)
                        {
                            treeViewItem.IsExpanded = false;
                        }
                        else//3.����Լ��Ķ����˵�
                        {
                            treeViewItem.IsExpanded = true;
                        }
                    }
                    else //����Լ�ʱ���л�״̬��ʵ�ֵ���չ��/����
                    {
                        treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
                    }
                }
                //else // �����˵��������丸�� һ���˵�
                //{

                //}
            }
        }
    }

}