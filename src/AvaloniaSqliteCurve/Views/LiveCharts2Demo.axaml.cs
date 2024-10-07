using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using AvaloniaSqliteCurve.Models;
using AvaloniaSqliteCurve.ViewModels;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ScottPlot;
using System;
using System.Collections.Generic;
using Color = Avalonia.Media.Color;

namespace AvaloniaSqliteCurve.Views;

public partial class LiveCharts2Demo : Window
{
    private Avalonia.Media.Color _fill = ConstData.Fill;
    private Avalonia.Media.Color _stroke = ConstData.Stroke;
    private float _lineWidth = 1;
    private LinePattern _linePattern = LinePattern.Solid;
    private int _xDivide = 5;
    private int _yDivide = 5;

    public LiveCharts2Demo()
    {
        InitializeComponent();

        this.MySettingView.IsTimeRangeVisible = false;

        UpdateStyle();

        MySettingView_OnBackgroundColorChanged(MySettingView.BackgroundColorPicker.Color);
        MySettingView_OnGridLineColorChanged(MySettingView.GridColorPicker.Color);
        MySettingView_OnXDivideChanged(_xDivide);
        MySettingView_OnYDivideChanged(_yDivide);
    }


    private void SaveChartsToImage_OnClick(object? sender, RoutedEventArgs e)
    {
        var target = LvCharts;
        var imagePath = "a.png";
        var dpi = 96;
        var width = target.Bounds.Width;
        var height = target.Bounds.Height;
        var pixelSize = new Avalonia.PixelSize((int)width, (int)height);
        var size = new Size(width, height);
        var dpiVector = new Vector(dpi, dpi);

        using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, dpiVector))
        {
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(imagePath);
        }
    }

    private void MySettingView_OnBackgroundColorChanged(Color color)
    {
        _fill = color;
        UpdateStyle();
    }

    private void MySettingView_OnGridLineColorChanged(Color color)
    {
        _stroke = color;
        UpdateStyle();
    }

    private void MySettingView_OnGridLineVisibleChanged(bool visible)
    {
        _lineWidth = visible ? 1f : 0.01f;
        UpdateStyle();
    }

    private void MySettingView_OnGridLineLinePatternChanged(LinePattern pattern)
    {
        _linePattern = pattern;
        UpdateStyle();
    }

    private void MySettingView_OnXDivideChanged(int divide)
    {
        _xDivide = divide;
        if (DataContext is not LiveCharts2DemoViewModel vm)
            return;

        var startTicks = vm.StartDateTime.Ticks;
        var valueRange = (vm.EndDateTime - vm.StartDateTime).Ticks;
        var divideValue = valueRange / _xDivide;
        var separators = new List<double>();
        for (var i = 0; i <= _xDivide; i++)
        {
            var currentValue = startTicks + i * divideValue;
            separators.Add(currentValue);
        }

        vm.XAxes[0].CustomSeparators = separators;
    }

    private void MySettingView_OnYDivideChanged(int divide)
    {
        _yDivide = divide;
        UpdateYDivide();
    }

    private void MySettingView_OnYRangeChanged(double min, double max)
    {
        UpdateYDivide();
    }

    private void UpdateYDivide()
    {
        if (DataContext is not LiveCharts2DemoViewModel vm)
            return;

        var valueRange = ConstData.MaxTop - ConstData.MinBottom;
        var divideValue = valueRange / _yDivide;
        var separators = new List<double>();
        for (var i = 0; i <= _yDivide; i++)
        {
            var currentValue = ConstData.MinBottom + i * divideValue;
            separators.Add((int)currentValue);
        }

        vm.YAxes[0].MinLimit = ConstData.MinBottom;
        vm.YAxes[0].MaxLimit = ConstData.MaxTop;
        vm.YAxes[0].CustomSeparators = separators;
        AddLimit(-100, 100, ScottPlot.Colors.Red);
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


    private void UpdateStyle()
    {
        LvCharts.DrawMarginFrame = new DrawMarginFrame
        {
            Fill = new SolidColorPaint(_fill.ToSKColor()),
            Stroke = new SolidColorPaint(_stroke.ToSKColor())
        };
        if (DataContext is not LiveCharts2DemoViewModel vm) return;

        var effect = _linePattern switch
        {
            LinePattern.Solid => null,
            LinePattern.Dashed => new DashEffect([10f, 10f]),
            LinePattern.DenselyDashed => new DashEffect([6f, 6f]),
            _ => new DashEffect([3f, 5f])
        };

        vm.XAxes[0].SeparatorsPaint =
            vm.YAxes[0].SeparatorsPaint = new SolidColorPaint
            {
                Color = _stroke.ToSKColor(),
                StrokeThickness = _lineWidth,
                PathEffect = effect
            };
    }
}