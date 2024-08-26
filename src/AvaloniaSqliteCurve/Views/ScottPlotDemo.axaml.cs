using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CodeWF.Tools.Extensions;
using ScottPlot.AxisRules;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotDemo : Window
{
    private static readonly string PlotFont = "Noto Sans TC";
    private const int LineCount = 16;
    private const int DisplayMaxPointsCount = 1000;
    private const double MinBottom = -300.0;
    private const double MaxTop = 300.0;
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(10));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(50));

    private int _displayMinuteRange = 5;
    private int _xDivide = 5;
    private int _yDivide = 5;

    private readonly List<DataStreamer> _streamers = new();

    static ScottPlotDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("ʵʱ���߲���");
    }

    public ScottPlotDemo()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "ʵʱ����";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;


        // ����ɫ
        BackgroundColorPicker.Color = Avalonia.Media.Colors.Black;
        GridColorPicker.Color = Avalonia.Media.Colors.White;

        // ���ɫ

        // ����������
        foreach (var linePattern in Enum.GetValues<LinePattern>())
        {
            ComboBoxGridLineType.Items.Add(linePattern);
        }

        ComboBoxGridLineType.SelectedItem = LinePattern.Solid;

        // X����ʾʱ�䷶Χ
        var kinds = Enum.GetValues<DisplayTimeRangeKind>();
        foreach (var kind in kinds)
        {
            ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem()
                { Content = kind.GetDescription(), Tag = (int)kind });
        }

        ComboBoxDisplayTimeRange.SelectedIndex = 0;

        // ���X��Y�ȷ�
        Enumerable.Range(1, 7).ForEach(index =>
        {
            ComboBoxXDivide.Items.Add(index);
            ComboBoxYDivide.Items.Add(index);
        });
        ComboBoxXDivide.SelectedItem = 5;
        ComboBoxYDivide.SelectedItem = 5;

        // ������߼�������
        var start = DateTime.Now;
        for (var i = 0; i < LineCount; i++)
        {
            //AddLimit(Random.Shared.Next(-100, 100), Random.Shared.Next(300, 600), _lines[i].Scatter!.Color);
        }

        // ��������
        for (var i = 0; i < LineCount; i++)
        {
            var streamer = plot.Plot.Add.DataStreamer(DisplayMaxPointsCount);
            streamer.ManageAxisLimits = false;
            streamer.ViewScrollLeft();
            _streamers.Add(streamer);
        }

        plot.Interaction.Disable();
        plot.Plot.Axes.ContinuouslyAutoscale = false;
        plot.Plot.Axes.SetLimits(0, DisplayMaxPointsCount, MinBottom, MaxTop);

        _addNewDataTimer.Elapsed += AddNewDataHandler;
        _updateDataTimer.Elapsed += UpdateDataHandler;

        _addNewDataTimer.Start();
        _updateDataTimer.Start();
    }

    private void AddNewDataHandler(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < LineCount; i++)
        {
            if (DateTime.Now.Millisecond % 5 == 1)
            {
                _streamers[i].Add(Random.Shared.Next(-1000, 1000));
            }
            else
            {
                _streamers[i].Add(Random.Shared.Next(-50, 200));
            }
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
    /// ��������ޱ�ǩ
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
    /// �޸ı���ɫ
    /// </summary>
    private void ChangeBackgroundColor_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = BackgroundColorPicker.Color;
        plot.Plot.DataBackground.Color = selectedColor.ToScottPlotColor();
    }

    /// <summary>
    /// �޸���ɫ
    /// </summary>
    private void GridColorPicker_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = GridColorPicker.Color;
        plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = selectedColor.ToScottPlotColor();
    }

    /// <summary>
    /// �Ƿ���ʾ�߿�
    /// </summary>
    private void ShowGird_OnClick(object? sender, RoutedEventArgs e)
    {
        plot.Plot.Grid.IsVisible = !plot.Plot.Grid.IsVisible;
    }

    /// <summary>
    /// �޸�������
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
    /// �޸�X�ȷ�
    /// </summary>
    private void ComboBoxXDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not int selectedItem) return;
        _xDivide = selectedItem;
        ChangeXDivide();
    }

    /// <summary>
    /// �޸�Y�ȷ�
    /// </summary>
    private void ComboBoxYDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not int selectedItem) return;
        _yDivide = selectedItem;

        const double range = MaxTop - MinBottom;
        var valueRangeOfOnePart = range / _yDivide;

        NumericManual ticks = new();
        for (var i = 0; i <= _yDivide; i++)
        {
            var position = MinBottom + valueRangeOfOnePart * i;
            var label = $"{position:F2}";
            ticks.AddMajor(position, label);
        }

        plot.Plot.Axes.Left.TickGenerator = ticks;
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
        for (var i = 0; i <= _xDivide; i++)
        {
            var minutesIndex = minutesForOnePart * i;
            var pointCountIndex = DisplayMaxPointsCount - i * pointCountForOnePart;
            if (i == 0)
            {
                ticks.AddMajor(pointCountIndex,
                    XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange) + " " +
                    XYLableExtensions.GetTimeUnit(_displayMinuteRange));
            }
            else
            {
                ticks.AddMajor(pointCountIndex, XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange));
            }
        }

        plot.Plot.Axes.Bottom.TickGenerator = ticks;
    }
}