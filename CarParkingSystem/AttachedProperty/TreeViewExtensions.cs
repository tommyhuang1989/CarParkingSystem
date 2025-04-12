using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.AttachedProperty
{
    /// <summary>
    /// TreeView 的附加属性
    /// </summary>
    public static class TreeViewExtensions
    {
        public static readonly AttachedProperty<bool> SuppressToggleProperty =
            AvaloniaProperty.RegisterAttached<TreeView, bool>("SuppressToggle", typeof(TreeViewExtensions));
        public static bool GetSuppressToggle(TreeView treeView) =>
       treeView.GetValue(SuppressToggleProperty);

        public static void SetSuppressToggle(TreeView treeView, bool value) =>
            treeView.SetValue(SuppressToggleProperty, value);
        static TreeViewExtensions()
        {
            TreeView.PointerPressedEvent.AddClassHandler<TreeView>((treeView, e) =>
            {
                if (GetSuppressToggle(treeView))
                {
                    var node = treeView.SelectedItem;
                    if (node != null && treeView.SelectedItem == node)
                    {
                        e.Handled = true; // 阻止默认的取消选中逻辑
                    }
                }
            });
        }
    }
}
