using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Data;


// ReSharper disable once CheckNamespace
namespace CarParkingSystem.Unities.I18nEx;

public class ArgCollection(I18n2Binding owner) : Collection<object>
{
    public List<(bool IsBinding, int Index)> Indexes { get; } = [];

    protected override void InsertItem(int index, object item)
    {
        if (item is BindingBase binding)
        {
            Indexes.Add((true, owner.Bindings.Count));
            owner.Bindings.Add(binding);
        }
        else
        {
            Indexes.Add((false, Count));
            base.InsertItem(index, item);
        }
    }
}
