using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.I18n;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarParkingSystem.Controls;

/// <summary>
/// 分页工具
/// </summary>
public partial class PagerBar : UserControl
{
    //public AvaloniaList<string> _comboBoxItems { get; } = [];
    public AvaloniaList<ComboItem> _comboBoxItems { get; } = [];
    public List<Control> _allControls;

    private readonly ControlTheme _normalBtnTheme;
    private readonly ControlTheme _holderBtnTheme;

    //private PagerButton btn1;
    //private PagerButton btn2;
    //private PagerButton btn3;
    //private PagerButton btn4;
    //private PagerButton btn5;
    //private PagerButton btn6;
    //private PagerButton btn7;
    public PagerBar()
    {
        InitializeComponent();

        _normalBtnTheme = Application.Current.FindResource("PagerButtonTheme") as ControlTheme;
        _holderBtnTheme = Application.Current.FindResource("PagerButtonHolderTheme") as ControlTheme;

        //_comboBoxItems = new AvaloniaList<string> { 
        //    "15 条/页",
        //    "25 条/页",
        //    "50 条/页",
        //    "100 条/页",
        //    //"2 条/页",
        //    //"5 条/页",
        //}; 
        _comboBoxItems = new AvaloniaList<ComboItem> {
           new ComboItem {DisplayName = I18nManager.GetString(Language.PerPageItems15), SelectedValue = 15 },
           new ComboItem {DisplayName = I18nManager.GetString(Language.PerPageItems25), SelectedValue = 25 },
           new ComboItem {DisplayName = I18nManager.GetString(Language.PerPageItems50), SelectedValue = 50 },
           new ComboItem {DisplayName = I18nManager.GetString(Language.PerPageItems100), SelectedValue = 100 },
        };

        cb.ItemsSource = _comboBoxItems;
        cb.SelectedItem = _comboBoxItems[0];//必须要在设置数据源之后，设置 SelectedItem 才会生效
        //cb.SelectedIndex = 0;
        //this.DataContext = this;

        //btn1 = this.Get<PagerButton>("btn1");
        //btn2 = this.Get<PagerButton>("btn2");
        //btn3 = this.Get<PagerButton>("btn3");
        //btn4 = this.Get<PagerButton>("btn4");
        //btn5 = this.Get<PagerButton>("btn5");
        //btn6 = this.Get<PagerButton>("btn6");
        //btn7 = this.Get<PagerButton>("btn7");

        btn1.Theme = _normalBtnTheme;
        btn2.Theme = _normalBtnTheme;
        btn3.Theme = _normalBtnTheme;
        btn4.Theme = _normalBtnTheme;
        btn5.Theme = _normalBtnTheme;
        btn6.Theme = _normalBtnTheme;
        btn7.Theme = _normalBtnTheme;

        _allControls = new List<Control>();

        _allControls.Add(btnFirst);
        _allControls.Add(btnPrev);
        _allControls.Add(btn1);
        _allControls.Add(btn2);
        _allControls.Add(btn3);
        _allControls.Add(btn4);
        _allControls.Add(btn5);
        _allControls.Add(btn6);
        _allControls.Add(btn7);
        _allControls.Add(btnNext);
        _allControls.Add(btnLast);

        ////20250309，总数量，总页数，页量选择 永远可见，所以不添加到控件列表中进行管理
        //_allControls.Add(txtAllCountTitle); 
        //_allControls.Add(txtAllCount);
        //_allControls.Add(txtPageCountTitle);
        //_allControls.Add(txtPageCount);
        //_allControls.Add(cb);

        _allControls.Add(txtGoToTitle);
        _allControls.Add(txtGoTo);
        _allControls.Add(btnGoTo);

    }

    static PagerBar()
    {
        CurrentPageIndexProperty.Changed.AddClassHandler<PagerBar>(OnCurrentPageIndexPropertyChanged);
        PageCountProperty.Changed.AddClassHandler<PagerBar>(OnPageCountPropertyChanged);
    }

    /// <summary>
    /// PageCount 变动，根据 PageCount 的值，显示不同的按钮
    /// </summary>
    /// <param name="one"></param>
    /// <param name="args"></param>
    private static void OnPageCountPropertyChanged(PagerBar one, AvaloniaPropertyChangedEventArgs args)
    {
        one.OnPageCountChanged((int)args.NewValue);
    }

    private void OnPageCountChanged(int newPageCount)
    {
        //int newPageCount = (int)args.NewValue;
        if (_allControls == null) return;
        //20250326, tommy, 初始化时（此时还没有数据），让所有按钮都先隐藏
        if (newPageCount == 0 || newPageCount == -1)
        {
            _allControls.ForEach(element => element.IsVisible = false);
            return;
        }

        switch (newPageCount)
        {
            case 1:
                {
                    _allControls.ForEach(element => element.IsVisible = false);
                    btn1.IsVisible = true;//只有一行只显示第一个按钮
                    //btn1.IsEnabled = true;//只有一行只显示第一个按钮
                    //20250327, tommy, 初始化时，添加第一条记录时，给第一个按钮赋值
                    btn1.Content = $"1";
                    break;
                }
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                {
                    //_btn1.Style = _normalBtnStyle;
                    //_btn5.Style = _normalBtnStyle;
                    btn2.Theme = _normalBtnTheme;//btn1
                    btn6.Theme = _normalBtnTheme;//btn5
                    SetButtonsVisible(newPageCount);
                    break;
                }
            default:
                {
                    SetButtonsVisible(7);
                    btn7.Content = $"{newPageCount}";
                    //_btn1.Style = _normalBtnStyle;
                    //_btn5.Style = _holderBtnStyle;
                    btn2.Theme = _normalBtnTheme;
                    btn6.Theme = _holderBtnTheme;
                    //_allControls.GetRange(10, 2).ForEach(e => e.IsVisible = true);//GetRange(10, 2)//这里不需要，因为 ToGo 都会显示
                    break;
                }
        }

        //cb.IsVisible = true;
    }

    /// <summary>
    /// CurrentPageIndex 变动，根据 CurrentPageIndex 的值，设置按钮显示的内容
    /// 以及设置按钮是否可用
    /// </summary>
    /// <param name="one"></param>
    /// <param name="args"></param>
    private static void OnCurrentPageIndexPropertyChanged(PagerBar one, AvaloniaPropertyChangedEventArgs args)
    {
        one.OnCurrentPageIndexChanged((int)args.NewValue);
    }

    private void OnCurrentPageIndexChanged(int curPageIndex)
    {
        //int curPageIndex = (int)args.NewValue;
        if (_allControls == null) return;
        if (curPageIndex > PageCount) return;

        foreach (var frameworkElement in _allControls.GetRange(0, 10))//_allControls.Take(9)
        {
            if (frameworkElement is PagerButton btn)
            {
                btn.IsActive = false;
            }
        }

        //20250312, tommy, 如果第一次载入时就只有一页，给第一个按钮赋值
        if (PageCount == 1)
        {
            btn1.Content = $"1";
        }

        if (PageCount <= 7)
        {
            //需要加 1，因为前面还有首页和上一页，所以数组按钮时从下标为2开始的
            if (_allControls[curPageIndex + 1] is PagerButton pb)//_allControls[curPageIndex],
            {
                pb.IsActive = true;
            }
        }
        else //大于 7 页
        {
            if (curPageIndex < 4)
            {
                //需要加 1，因为前面还有首页和上一页，所以数组按钮时从下标为2开始的
                if (_allControls[curPageIndex + 1] is PagerButton btn)
                {
                    btn.IsActive = true;
                }
                //_btn1.Style = _normalBtnStyle;
                //_btn5.Style = _holderBtnStyle;
                btn2.Theme = _normalBtnTheme;
                btn6.Theme = _holderBtnTheme;

                //btn1.Content = $"2";
                btn2.Content = $"2";
                btn3.Content = $"3";
                btn4.Content = $"4";
                btn5.Content = $"5";
                //btn6.Content = $"6";

            }
            else if (curPageIndex == 4)
            {
                btn4.IsActive = true;//btn4 是中间那个按钮
                //_btn1.Style = _normalBtnStyle;
                //_btn5.Style = _holderBtnStyle;
                btn2.Theme = _normalBtnTheme;
                btn6.Theme = _holderBtnTheme;

                SetButtonText(curPageIndex);
            }
            else if (curPageIndex > 4 && curPageIndex < PageCount - 3)
            {
                btn4.IsActive = true;//btn4 是中间那个按钮
                //_btn1.Style = _holderBtnStyle;
                //_btn5.Style = _holderBtnStyle;
                btn2.Theme = _holderBtnTheme;
                btn6.Theme = _holderBtnTheme;

                SetButtonText(curPageIndex);
            }
            else if (curPageIndex == PageCount - 3)
            {
                btn4.IsActive = true;//btn4 是中间那个按钮
                //_btn1.Style = _holderBtnStyle;
                //_btn5.Style = _normalBtnStyle;
                btn2.Theme = _holderBtnTheme;
                btn6.Theme = _normalBtnTheme;

                SetButtonText(curPageIndex);
            }
            else
            {
                //需要加 1，因为前面还有首页和上一页，所以数组按钮时从下标为2开始的
                if (_allControls[7 - (PageCount - curPageIndex) + 1] is PagerButton btn)
                {
                    btn.IsActive = true;
                }
                //btn1.Style = _holderBtnStyle;
                //btn5.Style = _normalBtnStyle;
                btn2.Theme = _holderBtnTheme;
                btn6.Theme = _normalBtnTheme;

                btn2.Content = $"{PageCount - 5}";
                btn3.Content = $"{PageCount - 4}";
                btn4.Content = $"{PageCount - 3}";
                btn5.Content = $"{PageCount - 2}";
                btn6.Content = $"{PageCount - 1}";
                //btn7.Content = $"{PageCount - 1}";
            }
        }


        if (curPageIndex == 1)//在第一页的情况
        {
            //当前页码为 1 是，首页、上一页不可用，下一页、尾页可用（页数要求大于1）
            btnFirst.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = true;
            btnLast.IsEnabled = true;
        }
        else if (curPageIndex == PageCount)//在尾页的情况
        {
            btnFirst.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnLast.IsEnabled = false;
        }
        else
        {
            btnFirst.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnLast.IsEnabled = true;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    #region 属性
    /// <summary>
    /// IndexToGo StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<int> IndexToGoProperty =
        AvaloniaProperty.Register<PagerBar, int>(nameof(IndexToGo), -1);

    /// <summary>
    /// Gets or sets the IndexToGo property. This StyledProperty 
    /// indicates ....
    /// </summary>
    public int IndexToGo
    {
        get => this.GetValue(IndexToGoProperty);
        set => SetValue(IndexToGoProperty, value);
    }


    /// <summary>
    /// AllCount StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<int> AllCountProperty =
        AvaloniaProperty.Register<PagerBar, int>(nameof(AllCount), -1);

    /// <summary>
    /// Gets or sets the AllCount property. This StyledProperty 
    /// indicates ....
    /// </summary>
    public int AllCount
    {
        get => this.GetValue(AllCountProperty);
        set => SetValue(AllCountProperty, value);
    }


    /// <summary>
    /// PageCount StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<int> PageCountProperty =
        AvaloniaProperty.Register<PagerBar, int>(nameof(PageCount), -1);

    /// <summary>
    /// Gets or sets the PageCount property. This StyledProperty
    /// indicates ....
    /// </summary>
    public int PageCount
    {
        get => this.GetValue(PageCountProperty);
        set => SetValue(PageCountProperty, value);
    }


    /// <summary>
    /// CurrentPageIndex StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<int> CurrentPageIndexProperty =
        AvaloniaProperty.Register<PagerBar, int>(nameof(CurrentPageIndex), -1);

    /// <summary>
    /// Gets or sets the CurrentPageIndex property. This StyledProperty 
    /// indicates ....
    /// </summary>
    public int CurrentPageIndex
    {
        get => this.GetValue(CurrentPageIndexProperty);
        set => SetValue(CurrentPageIndexProperty, value);
    }


    #endregion

    #region 命令
    /// <summary>
    /// FirstPageCommand StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<ICommand> FirstPageCommandProperty =
        AvaloniaProperty.Register<PagerBar, ICommand>(nameof(FirstPageCommand), default);

    /// <summary>
    /// Gets or sets the FirstPageCommand property. This StyledProperty 
    /// indicates ....
    /// </summary>
    public ICommand FirstPageCommand
    {
        get => this.GetValue(FirstPageCommandProperty);
        set => SetValue(FirstPageCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> PrevPageCommandProperty =
       AvaloniaProperty.Register<PagerBar, ICommand>(nameof(PrevPageCommand), default);
    public ICommand PrevPageCommand
    {
        get => this.GetValue(PrevPageCommandProperty);
        set => SetValue(PrevPageCommandProperty, value);
    }


    public static readonly StyledProperty<ICommand> NextPageCommandProperty =
       AvaloniaProperty.Register<PagerBar, ICommand>(nameof(NextPageCommand), default);
    public ICommand NextPageCommand
    {
        get => this.GetValue(NextPageCommandProperty);
        set => SetValue(NextPageCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> LastPageCommandProperty =
       AvaloniaProperty.Register<PagerBar, ICommand>(nameof(LastPageCommand), default);
    public ICommand LastPageCommand
    {
        get => this.GetValue(LastPageCommandProperty);
        set => SetValue(LastPageCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> GoToPageCommandProperty =
      AvaloniaProperty.Register<PagerBar, ICommand>(nameof(GoToPageCommand), default);

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public ICommand GoToPageCommand
    {
        get => this.GetValue(GoToPageCommandProperty);
        set => SetValue(GoToPageCommandProperty, value);
    }

    // 在 MyUserControl 中添加参数属性
    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<PagerBar, object>(nameof(CommandParameter));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<object> SelectionChangedCommandProperty =
       AvaloniaProperty.Register<PagerBar, object>(nameof(SelectionChangedCommand));

    public object SelectionChangedCommand
    {
        get => GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }
    #endregion

    #region 方法
    /// <summary>
    /// 当前页码改变时，会重新计算并刷新按钮的页码
    /// </summary>
    /// <param name="curIndex"></param>
    private void SetButtonText(int curIndex)
    {
        btn1.Content = $"1";// $"{curIndex - 3}";//btn1 永远是第一页
        btn2.Content = $"{curIndex - 2}";
        btn3.Content = $"{curIndex - 1}";
        btn4.Content = $"{curIndex - 0}";
        btn5.Content = $"{curIndex + 1}";
        btn6.Content = $"{curIndex + 2}";
        btn7.Content = $"{PageCount}";//$"{curIndex + 3}";//btn7 永远是 PageCount（最后一页）；
    }

    /// <summary>
    /// 设置页码按钮的 数字，并设置首页、上一页、下一页、尾页的可见性
    /// </summary>
    /// <param name="count"></param>
    private void SetButtonsVisible(int count)
    {
        _allControls.ForEach(element => element.IsVisible = false);
        var visibleList = _allControls.GetRange(2, count);//GetRange(1, count)
        for (var i = 0; i < visibleList.Count; i++)
        {
            visibleList[i].IsVisible = true;
            if (visibleList[i] is Button btn)
            {
                btn.Content = $"{i + 1}";
            }
        }

        //只要大于 1 页，首页、上一页、下一页、尾页 都需要显示
        btnFirst.IsVisible = true;
        btnPrev.IsVisible = true;
        btnNext.IsVisible = true;
        btnLast.IsVisible = true;

        txtGoToTitle.IsVisible = true;
        txtGoTo.IsVisible = true;
        btnGoTo.IsVisible = true;
    }
    #endregion
}