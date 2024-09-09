using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    public LiveCharts2Demo()
    {
        InitializeComponent();

        this.MySettingView.IsTimeRangeVisible = false;

        UpdateStyle();

        MySettingView_OnBackgroundColorChanged(MySettingView.BackgroundColorPicker.Color);
        MySettingView_OnGridLineColorChanged(MySettingView.GridColorPicker.Color);
        MySettingView_OnXDivideChanged(5);
        MySettingView_OnYDivideChanged(5);
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
        if (DataContext is not LiveCharts2DemoViewModel vm)
            return;

        var totalMilliseconds = (vm.EndDateTime - vm.StartDateTime).TotalMilliseconds;
        var divideMilliseconds = totalMilliseconds / divide;
        var ts = new TimeSpan(0, 0, 0, 0, (int)divideMilliseconds);
        vm.XAxes =
        [
            new DateTimeAxis(ts, date => $"{date:HH:mm}:00"),
        ];
    }

    private void MySettingView_OnYDivideChanged(int divide)
    {
        if (DataContext is not LiveCharts2DemoViewModel vm)
            return;

        const double valueRange = LiveCharts2DemoViewModel.MaxTop - LiveCharts2DemoViewModel.MinBottom;
        var divideValue = valueRange / divide;
        var separators = new List<double>();
        for (var i = 0; i <= divide; i++)
        {
            var currentValue = LiveCharts2DemoViewModel.MinBottom + i * divideValue;
            separators.Add((int)currentValue);
        }

        vm.YAxes[0].CustomSeparators = separators;
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