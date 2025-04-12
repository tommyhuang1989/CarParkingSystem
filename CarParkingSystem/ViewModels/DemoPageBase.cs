using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CarParkingSystem.ViewModels;

//abstract (string displayName, MaterialIconKind icon, int index = 0), ObservableValidator

/// <summary>
/// 区别于普通的 ViewModel，
/// 这是给左侧菜单每一项提供数据的 ViewModel
/// </summary>
public partial class DemoPageBase : ViewModelValidatorBase
{
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private MaterialIconKind _icon;
    [ObservableProperty] private int _index;
    [ObservableProperty] private int _pid;//第一层的 pid 均为 0
    [ObservableProperty] private int _id;
    [ObservableProperty] private bool _showEvenOneFoor = false;
    [ObservableProperty] private bool _isHiddenPage = false;//隐藏的Page（不在左侧菜单显示，但是却存在）
    [ObservableProperty] private bool _isExpanded = false;


    //[ObservableProperty] private bool _isSubPage = false;
    //[ObservableProperty] private string _parentName;
    public ObservableCollection<DemoPageBase> SubPages { get; set; }

    //bool isSubPage = false, string parentName = ""
    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayName">显示在左侧栏的标题</param>
    /// <param name="icon">图标</param>
    /// <param name="pid">父id</param>
    /// <param name="id">id</param>
    /// <param name="index">序号</param>
    /// <param name="showEvenOneFoor">只有一层也显示</param>
    public DemoPageBase(string displayName, MaterialIconKind icon, int pid, int id, int index = 0, bool showEvenOneFoor=false, bool isHiddenPage = false)
    {
        _displayName = displayName;
        _icon = icon;
        _index = index;

        _pid = pid;
        _id = id;
        _showEvenOneFoor = showEvenOneFoor;
        _isHiddenPage = isHiddenPage;
        //_isSubPage = isSubPage;
        //_parentName = parentName;
    }

    public static ObservableCollection<DemoPageBase> GetSubPages(int pid, ObservableCollection<DemoPageBase> pages)
    {
        ObservableCollection<DemoPageBase> firstPages = new ObservableCollection<DemoPageBase>(pages.Where(p => p.Pid == pid).OrderBy(x => x.Id).ToList());
        ObservableCollection<DemoPageBase> otherPages = new ObservableCollection<DemoPageBase>(pages.Where(p => p.Pid != pid).OrderBy(x => x.Id).ToList());
        foreach (var page in firstPages)
        {
            if (page.SubPages == null)
            {
                page.SubPages = new ObservableCollection<DemoPageBase>();
            }
            page.SubPages = GetSubPages(page.Id, otherPages);
        }

        return firstPages;
    }
}