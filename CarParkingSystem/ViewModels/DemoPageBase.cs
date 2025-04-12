using AvaloniaExtensions.Axaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace CarParkingSystem.ViewModels;

//abstract (string displayName, MaterialIconKind icon, int index = 0), ObservableValidator

/// <summary>
/// ��������ͨ�� ViewModel��
/// ���Ǹ����˵�ÿһ���ṩ���ݵ� ViewModel
/// </summary>
public partial class DemoPageBase : ViewModelValidatorBase
{
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private MaterialIconKind _icon;
    [ObservableProperty] private int _index;
    [ObservableProperty] private int _pid;//��һ��� pid ��Ϊ 0
    [ObservableProperty] private int _id;
    [ObservableProperty] private bool _showEvenOneFoor = false;
    [ObservableProperty] private bool _isHiddenPage = false;//���ص�Page���������˵���ʾ������ȴ���ڣ�
    [ObservableProperty] private bool _isExpanded = false;


    //[ObservableProperty] private bool _isSubPage = false;
    //[ObservableProperty] private string _parentName;
    public ObservableCollection<DemoPageBase> SubPages { get; set; }

    //bool isSubPage = false, string parentName = ""
    /// <summary>
    /// 
    /// </summary>
    /// <param name="displayName">��ʾ��������ı���</param>
    /// <param name="icon">ͼ��</param>
    /// <param name="pid">��id</param>
    /// <param name="id">id</param>
    /// <param name="index">���</param>
    /// <param name="showEvenOneFoor">ֻ��һ��Ҳ��ʾ</param>
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