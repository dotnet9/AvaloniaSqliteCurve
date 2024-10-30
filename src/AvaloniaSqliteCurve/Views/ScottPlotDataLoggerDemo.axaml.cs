using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
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

    private readonly List<DataLogger> _loggers = new();

    static ScottPlotDataLoggerDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("ʵʱ���߲���");
    }

    public ScottPlotDataLoggerDemo()
    {
        InitializeComponent();

        plot.Plot.Axes.Title.Label.Text = "ʵʱ����";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;
        plot.Plot.Axes.AntiAlias(false);
        plot.Plot.Axes.ContinuouslyAutoscale = false;
        plot.UserInputProcessor.Disable();

        // ��������
        plot.Plot.Axes.SetLimits(0, ConstData.DisplayMaxPointsCount, ConstData.MinBottom, ConstData.MaxTop);
        for (var i = 0; i < ConstData.LineCount; i++)
        {
            var logger = plot.Plot.Add.DataLogger();
            logger.ManageAxisLimits = false;
            logger.ViewSlide();
            _loggers.Add(logger);
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
    private void SettingView_OnGridLineLinePatternChanged(GridLineKind pattern)
    {
        plot.Plot.Grid.XAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.XAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.YAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        plot.Plot.Grid.YAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
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
        ChangeYRange();
    }

    private void ChangeYRange()
    {
        // 1��ֻ��ʾ�Ҳ�Y��
        DivideOneRight();

        // ÿ����һ��Y��
        //EveryLineY();
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
            ticks.AddMajor(pointCountIndex,
                XYLableExtensions.GetTimeStr(minutesIndex, _displayMinuteRange));
        }

        plot.Plot.Axes.Bottom.TickGenerator = ticks;
    }
}