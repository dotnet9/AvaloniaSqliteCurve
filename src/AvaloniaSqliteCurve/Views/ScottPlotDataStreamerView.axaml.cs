using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System;
using System.Collections.Generic;
using System.Timers;
using ReactiveUI;
using Color = Avalonia.Media.Color;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotDataStreamerView : UserControl
{
    private static readonly string PlotFont = "Noto Sans TC";
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(ConstData.AddDataInterval));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(ConstData.UpdateDataInterval));

    private int _displayMinuteRange = 5;
    private int _xDivide = 5;
    private int _yDivide = 5;
    private double _yMin = ConstData.MinBottom;
    private double _yMax = ConstData.MaxTop;

    private readonly Dictionary<int, DataStreamer> _streamers = new();

    static ScottPlotDataStreamerView()
    {
        PlotFont = ScottPlot.Fonts.Detect("ʵʱ���߲���");
    }

    public ScottPlotDataStreamerView()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "ʵʱ����";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;

        foreach (var point in PointListView.ViewModel.Points)
        {
            point.WhenAnyValue(p => p.Visible).Subscribe(_ => Update());
            point.WhenAnyValue(p => p.LineColor).Subscribe(_ => Update());
            point.WhenAnyValue(p => p.LineWidth).Subscribe(_ => Update());
            point.WhenAnyValue(p => p.Min).Subscribe(_ => Update());
            point.WhenAnyValue(p => p.Max).Subscribe(_ => Update());
            point.WhenAnyValue(p => p.WindowIndex).Subscribe(_ => Update());
        }

        notUpdate = false;

        // ��������
        plot.Interaction.Disable();
        plot.Plot.Axes.ContinuouslyAutoscale = false;
        CreateCharts();

        _addNewDataTimer.Elapsed += AddNewDataHandler;
        _updateDataTimer.Elapsed += UpdateDataHandler;

        _addNewDataTimer.Start();
        _updateDataTimer.Start();

        SettingView_OnBackgroundColorChanged(MySettingView.BackgroundColorPicker.Color);
        SettingView_OnGridLineColorChanged(MySettingView.GridColorPicker.Color);
        SettingView_OnXDivideChanged(5);
        SettingView_OnYDivideChanged(5);
    }

    private void AddNewDataHandler(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < ConstData.LineCount; i++)
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

    private bool notUpdate = true;

    private void Update()
    {
        if (notUpdate)
        {
            return;
        }
        CreateCharts();
    }

    private void CreateCharts()
    {
        plot.Plot.Clear();
        _streamers.Clear();
        for (var i = 0; i < ConstData.LineCount; i++)
        {
            var point = PointListView.ViewModel!.Points[i];
            var streamer = plot.Plot.Add.DataStreamer(ConstData.DisplayMaxPointsCount);
            streamer.Color = point.LineColor.Value.ToScottPlotColor();
            streamer.LineWidth = point.LineWidth;
            streamer.ManageAxisLimits = false;
            streamer.ViewScrollLeft();
            _streamers[i] = streamer;
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

    // �޸ı���ɫ
    private void SettingView_OnBackgroundColorChanged(Color color)
    {
        plot.Plot.FigureBackground.Color = Color.FromRgb(230, 232, 234).ToScottPlotColor();
        plot.Plot.DataBackground.Color = color.ToScottPlotColor();
    }

    // �޸ı������ɫ
    private void SettingView_OnGridLineColorChanged(Color color)
    {
        plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = color.ToScottPlotColor();
    }

    // �޸ı���߿ɼ���
    private void SettingView_OnGridLineVisibleChanged(bool visible)
    {
        plot.Plot.Grid.IsVisible = visible;
    }

    // �޸ı��������
    private void SettingView_OnGridLineLinePatternChanged(LinePattern pattern)
    {
        plot.Plot.Grid.XAxisStyle.MajorLineStyle.Pattern = pattern;
        plot.Plot.Grid.XAxisStyle.MinorLineStyle.Pattern = pattern;
        plot.Plot.Grid.YAxisStyle.MajorLineStyle.Pattern = pattern;
        plot.Plot.Grid.YAxisStyle.MinorLineStyle.Pattern = pattern;
    }

    // �޸�X�ȷ�
    private void SettingView_OnXDivideChanged(int divide)
    {
        _xDivide = divide;
        ChangeXDivide();
    }

    // �޸�Y�ȷ�
    private void SettingView_OnYDivideChanged(int divide)
    {
        _yDivide = divide;
        ChangeYRange();
    }

    /// <summary>
    /// �޸�Y��������
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MySettingView_OnYRangeChanged(double min, double max)
    {
        _yMin = min;
        _yMax = max;
        ChangeYRange();
    }

    private void ChangeYRange()
    {
        var range = _yMax - _yMin;
        var valueRangeOfOnePart = range / _yDivide;

        // ����Y������ʾ��Χ
        plot.Plot.Axes.Left.Min = _yMin;
        plot.Plot.Axes.Left.Max = _yMax;

        NumericManual ticks = new();
        for (var i = 0; i <= _yDivide; i++)
        {
            var position = _yMin + valueRangeOfOnePart * i;
            var label = $"{position:F2}";
            ticks.AddMajor(position, label);
        }

        plot.Plot.Axes.Left.TickGenerator = ticks;
    }

    // �޸�X����ʾʱ�䷶Χ
    private void SettingView_OnXDisplayTimeRangeChanged(int displayMinuteRange)
    {
        _displayMinuteRange = displayMinuteRange;
        ChangeXDivide();
    }

    private void ChangeXDivide()
    {
        NumericManual ticks = new();
        var pointCountForOnePart = ConstData.DisplayMaxPointsCount * 1.0 / _xDivide;
        var minutesForOnePart = _displayMinuteRange * 1.0 / _xDivide;
        plot.Plot.Axes.Bottom.Min = 0;
        plot.Plot.Axes.Bottom.Max = ConstData.DisplayMaxPointsCount;
        for (var i = 0; i <= _xDivide; i++)
        {
            var minutesIndex = minutesForOnePart * i;
            var pointCountIndex = ConstData.DisplayMaxPointsCount - i * pointCountForOnePart;
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