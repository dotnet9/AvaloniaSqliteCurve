using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotDemo : Window
{
    private static readonly string PlotFont = "Noto Sans TC";
    private const int LineCount = 16;
    private const int DisplayMaxPointsCount = 1000;
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(10));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(50));

    private int _displayMinuteRange = 5;
    private int _xDivide = 5;
    private int _yDivide = 5;

    private readonly List<DataStreamer> _streamers = new();

    static ScottPlotDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("实时曲线测试");
    }

    public ScottPlotDemo()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;


        // 背景色
        BackgroundColorPicker.Color = Avalonia.Media.Colors.Black;
        GridColorPicker.Color = Avalonia.Media.Colors.White;

        // 风格色

        // 网络线类型
        foreach (var linePattern in Enum.GetValues<LinePattern>())
        {
            ComboBoxGridLineType.Items.Add(linePattern);
        }

        ComboBoxGridLineType.SelectedItem = LinePattern.Solid;

        // X轴显示时间范围
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "5分钟", Tag = 5 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "10分钟", Tag = 10 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "30分钟", Tag = 30 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "1小时", Tag = 60 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "2小时", Tag = 120 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "4小时", Tag = 240 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "8小时", Tag = 480 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "1天", Tag = 1440 });
        ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem() { Content = "2天", Tag = 2880 });
        ComboBoxDisplayTimeRange.SelectedIndex = 0;

        // 添加X、Y等分
        Enumerable.Range(1, 7).ForEach(index =>
        {
            ComboBoxXDivide.Items.Add(index);
            ComboBoxYDivide.Items.Add(index);
        });
        ComboBoxXDivide.SelectedItem = 5;
        ComboBoxYDivide.SelectedItem = 5;

        // 添加曲线及上下限
        var start = DateTime.Now;
        for (var i = 0; i < LineCount; i++)
        {
            //AddLimit(Random.Shared.Next(-100, 100), Random.Shared.Next(300, 600), _lines[i].Scatter!.Color);
        }

        // 生成曲线
        for (var i = 0; i < LineCount; i++)
        {
            var streamer = plot.Plot.Add.DataStreamer(DisplayMaxPointsCount);
            streamer.ViewScrollLeft();
            _streamers.Add(streamer);
        }

        plot.Interaction.Disable();
        plot.Plot.Axes.SetLimitsY(bottom: -50, top: 50);
        plot.Plot.Axes.SetLimitsX(left: 1000, right: 0);

        _addNewDataTimer.Elapsed += AddNewDataHandler;
        _updateDataTimer.Elapsed += UpdateDataHandler;

        _addNewDataTimer.Start();
        _updateDataTimer.Start();
    }

    private void AddNewDataHandler(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < LineCount; i++)
        {
            _streamers[i].Add(Random.Shared.Next(-100, 300));
        }
    }

    private void UpdateDataHandler(object? sender, ElapsedEventArgs e)
    {
        if (_streamers.Count > 0 && _streamers[0].HasNewData)
        {
            plot.Refresh();
        }
    }

    /// <summary>
    /// 添加上下限标签
    /// </summary>
    private void AddLimit(double min, double max, ScottPlot.Color color)
    {
        var textColor = new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B));
        MinItems.Items.Add(new TextBlock()
        {
            Text = $"{min}",
            Foreground = textColor
        });
        MaxItems.Items.Add(new TextBlock()
        {
            Text = $"{max}",
            Foreground = textColor
        });
    }

    /// <summary>
    /// 修改背景色
    /// </summary>
    private void ChangeBackgroundColor_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = BackgroundColorPicker.Color;
        plot.Plot.DataBackground.Color = selectedColor.ToScottPlotColor();
    }

    /// <summary>
    /// 修改线色
    /// </summary>
    private void GridColorPicker_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = GridColorPicker.Color;
        plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = selectedColor.ToScottPlotColor();
    }

    /// <summary>
    /// 是否显示线框
    /// </summary>
    private void ShowGird_OnClick(object? sender, RoutedEventArgs e)
    {
        plot.Plot.Grid.IsVisible = !plot.Plot.Grid.IsVisible;
    }

    /// <summary>
    /// 修改线类型
    /// </summary>
    private void ComboBoxGridLineType_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ComboBoxGridLineType.SelectionBoxItem is LinePattern pattern)
        {
            plot.Plot.Grid.XAxisStyle.MajorLineStyle.Pattern = pattern;
            plot.Plot.Grid.XAxisStyle.MinorLineStyle.Pattern = pattern;
            plot.Plot.Grid.YAxisStyle.MajorLineStyle.Pattern = pattern;
            plot.Plot.Grid.YAxisStyle.MinorLineStyle.Pattern = pattern;
        }
    }

    /// <summary>
    /// 修改X等分
    /// </summary>
    private void ComboBoxXDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not int selectedItem) return;
        _xDivide = selectedItem;
        ChangeXDivide();
    }

    /// <summary>
    /// 修改Y等分
    /// </summary>
    private void ComboBoxYDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }

    private void DisplayTimeRange_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not ComboBoxItem selectedItem) return;

        _displayMinuteRange = int.Parse(selectedItem.Tag!.ToString()!);
        ChangeXDivide();
    }

    private void ChangeXDivide()
    {
        NumericManual ticks = new();
        var pointCountForOnePart = DisplayMaxPointsCount * 1.0 / _xDivide;
        var minutesForOnePart = _displayMinuteRange * 1.0 / _xDivide;
        for (int i = 0; i <= _xDivide; i++)
        {
            var minutesIndex = minutesForOnePart * i;
            var pointCountIndex = DisplayMaxPointsCount - i * pointCountForOnePart;
            if (i == 0)
            {
                ticks.AddMajor(pointCountIndex,
                    XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange) + " " + XYLableExtensions.GetTimeUnit(_displayMinuteRange));
            }
            else
            {
                ticks.AddMajor(pointCountIndex, XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange));
            }
        }

        plot.Plot.Axes.Bottom.TickGenerator = ticks;
    }

    
}