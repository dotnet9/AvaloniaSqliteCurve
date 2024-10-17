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
using Color = Avalonia.Media.Color;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotSignalDemo : Window
{
    private static readonly string PlotFont = "Noto Sans TC";
    private int _nextValueIndex = ConstData.DisplayMaxPointsCount - 1;
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(ConstData.AddDataInterval));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(ConstData.UpdateDataInterval));

    private int _displayMinuteRange = 5;
    private int _xDivide = 5;
    private int _yDivide = 5;

    private readonly List<double[]> _signals = new();

    static ScottPlotSignalDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("实时曲线测试");
    }

    public ScottPlotSignalDemo()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;

        // 生成曲线
        plot.Interaction.Disable();
        plot.Plot.Axes.ContinuouslyAutoscale = false;
        plot.Plot.Axes.SetLimits(0, ConstData.DisplayMaxPointsCount, ConstData.MinBottom, ConstData.MaxTop);
        for (var i = 0; i < ConstData.LineCount; i++)
        {
            var data = new double[ConstData.DisplayMaxPointsCount];
            _signals.Add(data);
            var logger = plot.Plot.Add.Signal(data);
            //logger.ManageAxisLimits = false;
            //logger.ViewSlide();
            //_signals.Add(logger);
        }

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
            double newValue;
            if (DateTime.Now.Millisecond % 5 == 1)
            {
                newValue = Random.Shared.Next(-1000, 1000);
            }
            else
            {
                newValue = Random.Shared.Next(-50, 200);
            }

            Buffer.BlockCopy(_signals[i], 1, _signals[i], 0, ConstData.DisplayMaxPointsCount - 1);
            if (_nextValueIndex < 0)
            {
                _nextValueIndex = ConstData.DisplayMaxPointsCount - 1;
            }
            _signals[i][_nextValueIndex] = newValue;
            _nextValueIndex--;
        }
    }

    private void UpdateDataHandler(object? sender, ElapsedEventArgs e)
    {
        plot.Refresh();
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

        const double range = ConstData.MaxTop - ConstData.MinBottom;
        var valueRangeOfOnePart = range / _yDivide;

        NumericManual ticks = new();
        for (var i = 0; i <= _yDivide; i++)
        {
            var position = ConstData.MinBottom + valueRangeOfOnePart * i;
            var label = string.Empty; //$"{position:F2}";
            ticks.AddMajor(position, label);
        }

        plot.Plot.Axes.Left.TickGenerator = ticks;
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