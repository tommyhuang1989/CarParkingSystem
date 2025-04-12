using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarParkingSystem.Controls
{
    public struct ComboxPageSize
    {
        public int Size { get; set; }

        public ComboxPageSize(int size)
        {
            Size = size;
        }

        public override string ToString()
        {
            return $"{Size}/页";
        }
    }
    public class PagerHost : ContentControl
    {
        private readonly Button _btnFirst;
        private readonly Button _btnPrev;
        private readonly PagerButton _btn1;
        private readonly PagerButton _btn2;
        private readonly PagerButton _btn3;
        private readonly PagerButton _btn4;
        private readonly PagerButton _btn5;
        private readonly PagerButton _btn6;
        private readonly PagerButton _btn7;
        private readonly Button _btnNext;
        private readonly Button _btnLast;

        private readonly ComboBox _cb;
        private readonly TextBlock _txtGoToTitle;
        private readonly TextBox _txtGoTo;
        private readonly Button _btnGoTo;
        private readonly List<Control> _allControls;

        private readonly ControlTheme _normalBtnTheme;
        private readonly ControlTheme _holderBtnTheme;
        public PagerHost()
        {
            _normalBtnTheme = Application.Current.FindResource("PagerButtonTheme") as ControlTheme;
            _holderBtnTheme = Application.Current.FindResource("PagerButtonHolderTheme") as ControlTheme;

            //InitializeComponent();

            _btnFirst = new Button() { Theme = _normalBtnTheme, Content = "<<" };
            _btnPrev = new Button() { Theme = _normalBtnTheme, Content = "<" };
            _btn1 = new PagerButton() { Theme = _normalBtnTheme, Content = "1" };
            _btn2 = new PagerButton() { Theme = _normalBtnTheme };
            _btn3 = new PagerButton() { Theme = _normalBtnTheme };
            _btn4 = new PagerButton() { Theme = _normalBtnTheme };
            _btn5 = new PagerButton() { Theme = _normalBtnTheme };
            _btn6 = new PagerButton() { Theme = _normalBtnTheme };
            _btn7 = new PagerButton() { Theme = _normalBtnTheme, Content = $"{PageCount}" };
            _btnNext = new Button() { Theme = _normalBtnTheme, Content = ">" };
            _btnLast = new Button() { Theme = _normalBtnTheme, Content = ">>" };
            _btnGoTo = new Button() { Theme = _normalBtnTheme, Content = "确定" };

            //    _comboBoxItems = new AvaloniaList<string> {
            //    "10 条/页",
            //    "20 条/页",
            //    "50 条/页",
            //    "100 条/页",
            //    "2 条/页",
            //    "5 条/页",
            //};

            _cb = new ComboBox();
            _cb.Items.Add(new ComboxPageSize(10));
            _cb.Items.Add(new ComboxPageSize(20));
            _cb.Items.Add(new ComboxPageSize(50));
            _cb.Items.Add(new ComboxPageSize(100));
            _cb.Items.Add(new ComboxPageSize(2));
            _cb.Items.Add(new ComboxPageSize(5));
            _cb.SelectedIndex = 0;

            _txtGoToTitle = new TextBlock() { Text = "跳转到" };
            _txtGoTo = new TextBox();

            var panel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            panel.Children.Add(_btnFirst);
            panel.Children.Add(_btnPrev);
            panel.Children.Add(_btn1);
            panel.Children.Add(_btn2);
            panel.Children.Add(_btn3);
            panel.Children.Add(_btn4);
            panel.Children.Add(_btn5);
            panel.Children.Add(_btn6);
            panel.Children.Add(_btn7);
            panel.Children.Add(_btnNext);
            panel.Children.Add(_btnLast);

            panel.Children.Add(_cb);
            panel.Children.Add(_txtGoToTitle);
            panel.Children.Add(_txtGoTo);

            this.Content = panel;

            _allControls = new List<Control>
            {
                _btnFirst,
                _btnPrev,
                _btn1,
                _btn2,
                _btn3,
                _btn4,
                _btn5,
                _btn6,
                _btn7,
                _btnNext,
                _btnLast,
                //_cb,
                _txtGoToTitle,
                _txtGoTo,
                _btnGoTo
            };

            _btnFirst.Click += (sender, args) => { CurrentPageIndex = 1; };
            _btnPrev.Click += (sender, args) => { if (CurrentPageIndex > 1) CurrentPageIndex--; };
            _btn1.Click += (sender, args) => { CurrentPageIndex = 1; };
            _btn1.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn1.Content.ToString()); };
            _btn2.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn2.Content.ToString()); };
            _btn3.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn3.Content.ToString()); };
            _btn4.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn4.Content.ToString()); };
            _btn5.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn5.Content.ToString()); };
            _btn6.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn5.Content.ToString()); };
            _btn7.Click += (sender, args) => { CurrentPageIndex = int.Parse(_btn5.Content.ToString()); };
            _btnNext.Click += (sender, args) => { if (CurrentPageIndex < PageCount) CurrentPageIndex++; };
            _btnLast.Click += (sender, args) => { CurrentPageIndex = PageCount; };

            //_cb.SelectionChanged += (sender, args) =>
            //{
            //    PageSize = ((ComboxPageSize)_cb.SelectedItem).Size;
            //};

            /*
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
            */

            CurrentPageIndexProperty.Changed.AddClassHandler<PagerHost>(OnCurrentPageIndexPropertyChanged);
            PageCountProperty.Changed.AddClassHandler<PagerHost>(OnPageCountPropertyChanged);

        }

        #region 事件

        /// <summary>
        /// PageCount 变动，根据 PageCount 的值，显示不同的按钮
        /// </summary>
        /// <param name="one"></param>
        /// <param name="args"></param>
        private void OnPageCountPropertyChanged(PagerHost one, AvaloniaPropertyChangedEventArgs args)
        {
            int newPageCount = (int)args.NewValue;
            if (_allControls == null || newPageCount == 0 || newPageCount == -1) return;

            switch (newPageCount)
            {
                case 1:
                    {
                        _allControls.ForEach(element => element.IsVisible = false);
                        _btn1.IsVisible = true;//只有一行只显示第一个按钮
                                              //btn1.IsEnabled = true;//只有一行只显示第一个按钮
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
                        _btn2.Theme = _normalBtnTheme;//btn1
                        _btn6.Theme = _normalBtnTheme;//btn5
                        SetButtonsVisible(newPageCount);
                        break;
                    }
                default:
                    {
                        SetButtonsVisible(7);
                        _btn7.Content = $"{newPageCount}";
                        //_btn1.Style = _normalBtnStyle;
                        //_btn5.Style = _holderBtnStyle;
                        _btn2.Theme = _normalBtnTheme;
                        _btn6.Theme = _holderBtnTheme;
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
        private void OnCurrentPageIndexPropertyChanged(PagerHost one, AvaloniaPropertyChangedEventArgs args)
        {
            int curPageIndex = (int)args.NewValue;
            if (_allControls == null) return;
            if (curPageIndex > PageCount) return;

            foreach (var frameworkElement in _allControls.GetRange(0, 10))//_allControls.Take(9)
            {
                if (frameworkElement is PagerButton btn)
                {
                    btn.IsActive = false;
                }
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
                    _btn2.Theme = _normalBtnTheme;
                    _btn6.Theme = _holderBtnTheme;

                    //btn1.Content = $"2";
                    _btn2.Content = $"2";
                    _btn3.Content = $"3";
                    _btn4.Content = $"4";
                    _btn5.Content = $"5";
                    //btn6.Content = $"6";

                }
                else if (curPageIndex == 4)
                {
                    _btn4.IsActive = true;//btn4 是中间那个按钮
                                         //_btn1.Style = _normalBtnStyle;
                                         //_btn5.Style = _holderBtnStyle;
                    _btn2.Theme = _normalBtnTheme;
                    _btn6.Theme = _holderBtnTheme;

                    SetButtonText(curPageIndex);
                }
                else if (curPageIndex > 4 && curPageIndex < PageCount - 3)
                {
                    _btn4.IsActive = true;//btn4 是中间那个按钮
                                         //_btn1.Style = _holderBtnStyle;
                                         //_btn5.Style = _holderBtnStyle;
                    _btn2.Theme = _holderBtnTheme;
                    _btn6.Theme = _holderBtnTheme;

                    SetButtonText(curPageIndex);
                }
                else if (curPageIndex == PageCount - 3)
                {
                    _btn4.IsActive = true;//btn4 是中间那个按钮
                                         //_btn1.Style = _holderBtnStyle;
                                         //_btn5.Style = _normalBtnStyle;
                    _btn2.Theme = _holderBtnTheme;
                    _btn6.Theme = _normalBtnTheme;

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
                    _btn2.Theme = _holderBtnTheme;
                    _btn6.Theme = _normalBtnTheme;

                    _btn2.Content = $"{PageCount - 5}";
                    _btn3.Content = $"{PageCount - 4}";
                    _btn4.Content = $"{PageCount - 3}";
                    _btn5.Content = $"{PageCount - 2}";
                    _btn6.Content = $"{PageCount - 1}";
                    //btn7.Content = $"{PageCount - 1}";
                }
            }


            if (curPageIndex == 1)//在第一页的情况
            {
                //当前页码为 1 是，首页、上一页不可用，下一页、尾页可用（页数要求大于1）
                _btnFirst.IsEnabled = false;
                _btnPrev.IsEnabled = false;
                _btnNext.IsEnabled = true;
                _btnLast.IsEnabled = true;
            }
            else if (curPageIndex == PageCount)//在尾页的情况
            {
                _btnFirst.IsEnabled = true;
                _btnPrev.IsEnabled = true;
                _btnNext.IsEnabled = false;
                _btnLast.IsEnabled = false;
            }
            else
            {
                _btnFirst.IsEnabled = true;
                _btnPrev.IsEnabled = true;
                _btnNext.IsEnabled = true;
                _btnLast.IsEnabled = true;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// IndexToGo StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<int> IndexToGoProperty =
            AvaloniaProperty.Register<PagerHost, int>(nameof(IndexToGo), -1);

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
            AvaloniaProperty.Register<PagerHost, int>(nameof(AllCount), -1);

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
            AvaloniaProperty.Register<PagerHost, int>(nameof(PageCount), -1);

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
            AvaloniaProperty.Register<PagerHost, int>(nameof(CurrentPageIndex), -1);

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
            AvaloniaProperty.Register<PagerHost, ICommand>(nameof(FirstPageCommand), default);

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
           AvaloniaProperty.Register<PagerHost, ICommand>(nameof(PrevPageCommand), default);
        public ICommand PrevPageCommand
        {
            get => this.GetValue(PrevPageCommandProperty);
            set => SetValue(PrevPageCommandProperty, value);
        }


        public static readonly StyledProperty<ICommand> NextPageCommandProperty =
           AvaloniaProperty.Register<PagerHost, ICommand>(nameof(NextPageCommand), default);
        public ICommand NextPageCommand
        {
            get => this.GetValue(NextPageCommandProperty);
            set => SetValue(NextPageCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> LastPageCommandProperty =
           AvaloniaProperty.Register<PagerHost, ICommand>(nameof(LastPageCommand), default);
        public ICommand LastPageCommand
        {
            get => this.GetValue(LastPageCommandProperty);
            set => SetValue(LastPageCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> GoToPageCommandProperty =
          AvaloniaProperty.Register<PagerHost, ICommand>(nameof(GoToPageCommand), default);

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public ICommand GoToPageCommand
        {
            get => this.GetValue(GoToPageCommandProperty);
            set => SetValue(GoToPageCommandProperty, value);
        }

        // 在 MyUserControl 中添加参数属性
        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<PagerHost, object>(nameof(CommandParameter));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly StyledProperty<object> SelectionChangedCommandProperty =
           AvaloniaProperty.Register<PagerHost, object>(nameof(SelectionChangedCommand));

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
            _btn1.Content = $"1";// $"{curIndex - 3}";//btn1 永远是第一页
            _btn2.Content = $"{curIndex - 2}";
            _btn3.Content = $"{curIndex - 1}";
            _btn4.Content = $"{curIndex - 0}";
            _btn5.Content = $"{curIndex + 1}";
            _btn6.Content = $"{curIndex + 2}";
            _btn7.Content = $"{PageCount}";//$"{curIndex + 3}";//btn7 永远是 PageCount（最后一页）；
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
            _btnFirst.IsVisible = true;
            _btnPrev.IsVisible = true;
            _btnNext.IsVisible = true;
            _btnLast.IsVisible = true;

            _txtGoToTitle.IsVisible = true;
            _txtGoTo.IsVisible = true;
            _btnGoTo.IsVisible = true;
        }
        #endregion
    }
}
