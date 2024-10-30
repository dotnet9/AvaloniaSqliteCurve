using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
using ReactiveUI;
using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using System;
using System.Collections.Generic;
using System.Timers;
using Color = Avalonia.Media.Color;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotDataLoggerDemo : Window
{
    private static readonly string PlotFont = "Noto Sans TC";
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(ConstData.AddDataInterval));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(ConstData.UpdateDataInterval));

    private int _displayMinuteRange = 5;
    private int _xDivide = 5;
    private int _yDivide = 5;

    private VerticalLine? _vLine;
    private Dictionary<int, ScottPlot.Plottables.Text> _streamerTexts = new();

    private readonly Dictionary<int, DataLogger> _loggers = new();
    private readonly Dictionary<int, RightAxis> _rightAxes = new();

    static ScottPlotDataLoggerDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("实时曲线测试");
    }

    public ScottPlotDataLoggerDemo()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;
        plot.Plot.Axes.AntiAlias(false);
        plot.Plot.Axes.ContinuouslyAutoscale = false;
        plot.UserInputProcessor.Disable();

        //foreach (var point in PointListView.ViewModel.Points)
        //{
        //    point.WhenAnyValue(p => p.Visible).Subscribe(_ => Update());
        //    point.WhenAnyValue(p => p.LineColor).Subscribe(_ => Update());
        //    point.WhenAnyValue(p => p.LineWidth).Subscribe(_ => Update());
        //    point.WhenAnyValue(p => p.Min).Subscribe(_ => Update());
        //    point.WhenAnyValue(p => p.Max).Subscribe(_ => Update());
        //    point.WhenAnyValue(p => p.WindowIndex).Subscribe(_ => Update());
        //}

        _notUpdate = false;

        // 生成曲线
        plot.PointerPressed += Plot_PointerPressed;

        // 生成曲线
        CreateCharts();

        _addNewDataTimer.Elapsed += AddNewDataHandler;
        _updateDataTimer.Elapsed += UpdateDataHandler;

        _addNewDataTimer.Start();
        _updateDataTimer.Start();

        SettingView_OnBackgroundColorChanged(MySettingView.BackgroundColorPicker.Color);
        SettingView_OnGridLineColorChanged(MySettingView.GridColorPicker.Color);
        SettingView_OnXDivideChanged(_xDivide);
        SettingView_OnYDivideChanged(_yDivide);
    }
    private void Plot_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var mousePos = e.GetPosition(plot);
        var dataArea = plot.Plot.LastRender.DataRect;
        var width = dataArea.Width;
        var x = mousePos.X - dataArea.Left;
        var ratio = x / width;
        var pointCountIndex = (int)(ConstData.DisplayMaxPointsCount * ratio);
        if (pointCountIndex < 0)
        {
            pointCountIndex = 0;
        }

        if (pointCountIndex >= ConstData.DisplayMaxPointsCount)
        {
            pointCountIndex = ConstData.DisplayMaxPointsCount - 1;
        }

        //if (_vLine == null)
        //{
        //    _vLine = plot.Plot.Add.VerticalLine(pointCountIndex, pattern: LinePattern.Solid);
        //    _vLine.IsVisible = true;

        //    for (var i = 0; i < _loggers.Count; i++)
        //    {
        //        var value = _loggers[i].Data.Data[pointCountIndex];
        //        _streamerTexts[i] = CreateText(pointCountIndex, value);
        //        _streamerTexts[i].IsVisible = value != double.NaN;
        //    }
        //}
        //else
        //{
        //    _vLine.X = pointCountIndex;

        //    for (var i = 0; i < _streamers.Count; i++)
        //    {
        //        var value = _streamers[i].Data.Data[ConstData.DisplayMaxPointsCount - pointCountIndex];
        //        _streamerTexts[i].IsVisible = value != double.NaN;
        //        _streamerTexts[i].LabelText = value.ToString();
        //        _streamerTexts[i].Location = new Coordinates(pointCountIndex, value);
        //    }
        //}

        MySettingView.UpdateMoreText($"DataRect=({dataArea}),x={x},index={pointCountIndex}");

        plot.Refresh();
    }

    private ScottPlot.Plottables.Text CreateText(double x, double y)
    {
        var text = y.ToString();
        var txtSample = plot.Plot.Add.Text(text, x, y);

        txtSample.LabelFontSize = 14;
        txtSample.LabelFontName = Fonts.Detect(text); // this works
        txtSample.LabelStyle.SetBestFont(); // this also works
        txtSample.LabelFontColor = ScottPlot.Colors.DarkRed;
        return txtSample;
    }

    private void AddNewDataHandler(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < ConstData.LineCount; i++)
        {
            var value = DateTime.Now.Ticks * (DateTime.Now.Ticks % 5 == 1 ? -1.0 : 1.0) % 300;
            _loggers[i].Add(value);
        }
    }

    private void UpdateDataHandler(object? sender, ElapsedEventArgs e)
    {
        if (_loggers.Count > 0 && _loggers[0].HasNewData)
        {
            plot.Refresh();
        }
    }

    private readonly bool _notUpdate;

    private void Update()
    {
        if (_notUpdate)
        {
            return;
        }

        CreateCharts();
    }

    private void CreateCharts()
    {
        plot.Plot.Clear();
        _loggers.Clear();
        for (var i = 0; i < ConstData.LineCount; i++)
        {
            //var point = PointListView.ViewModel!.Points[i];
            var logger = plot.Plot.Add.DataLogger();
            //logger.Color = point.LineColor.Value.ToScottPlotColor();
            //logger.LineWidth = point.LineWidth;
            logger.ManageAxisLimits = false;
            logger.ViewSlide();
            _loggers[i] = logger;
        }

        plot.Plot.Axes.Right.IsVisible = true;
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

    // 修改背景色
    private void SettingView_OnBackgroundColorChanged(Color color)
    {
        plot.Plot.FigureBackground.Color = Color.FromRgb(230, 232, 234).ToScottPlotColor();
        plot.Plot.DataBackground.Color = color.ToScottPlotColor();
    }

    // 修改表格线颜色
    private void SettingView_OnGridLineColorChanged(Color color)
    {
        plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = color.ToScottPlotColor();
    }

    // 修改表格线可见性
    private void SettingView_OnGridLineVisibleChanged(bool visible)
    {
        plot.Plot.Grid.IsVisible = visible;
    }

    // 修改表格线类型
    private void SettingView_OnGridLineLinePatternChanged(GridLineKind pattern)
    {
        plot.Plot.Grid.XAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.XAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.YAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.YAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
    }

    // 修改X等分
    private void SettingView_OnXDivideChanged(int divide)
    {
        _xDivide = divide;
        ChangeXDivide();
    }

    // 修改Y等分
    private void SettingView_OnYDivideChanged(int divide)
    {
        _yDivide = divide;
        ChangeYRange();
    }

    /// <summary>
    /// 修改Y轴上下限
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MySettingView_OnYRangeChanged(double min, double max)
    {
        ChangeYRange();
    }

    private void ChangeYRange()
    {
        try
        {
            // 1、只显示右侧Y轴
            DivideOneRight();

            // 每条线一个Y轴
            //EveryLineY();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Change Y exception: {ex.Message}");
        }
    }

    private NumericManual? _yTicks;

    private void DivideOneRight()
    {
        if (_yTicks == null)
        {
            plot.Plot.Axes.Left.Min = plot.Plot.Axes.Right.Min = ConstData.MinBottom;
            plot.Plot.Axes.Left.Max = plot.Plot.Axes.Right.Max = ConstData.MaxTop;
            plot.Plot.Axes.Left.TickLabelStyle.IsVisible = plot.Plot.Axes.Right.TickLabelStyle.IsVisible = false;
            plot.Plot.Axes.Left.MajorTickStyle.Length = plot.Plot.Axes.Right.MajorTickStyle.Length = 0;
        }

        var range = ConstData.MaxTop - ConstData.MinBottom;
        var valueRangeOfOnePart = range / _yDivide;

        _yTicks = new NumericManual();
        for (var i = 0; i <= _yDivide; i++)
        {
            var position = ConstData.MinBottom + valueRangeOfOnePart * i;
            _yTicks.AddMajor(position, string.Empty);
        }

        plot.Plot.Axes.Left.TickGenerator =
            plot.Plot.Axes.Right.TickGenerator = _yTicks;

        AddLimit(ConstData.MinBottom, ConstData.MaxTop, ScottPlot.Colors.Red);
    }

    // 修改X轴显示时间范围
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
            ticks.AddMajor(pointCountIndex,
                XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange));
        }

        plot.Plot.Axes.Bottom.TickGenerator = ticks;
    }
}