using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using AvaloniaSqliteCurve.ViewModels;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace AvaloniaSqliteCurve.Views;

public partial class LiveCharts2Demo : Window
{
    public LiveCharts2Demo()
    {
        InitializeComponent();

        LvCharts.DrawMarginFrame = new DrawMarginFrame()
        {
            Fill = new SolidColorPaint(_fill.ToSKColor())
        };
    }

    private Avalonia.Media.Color _fill = Avalonia.Media.Colors.Black;
    private Avalonia.Media.Color _stroke = Avalonia.Media.Colors.White;
    private float _lineWidth = 1;

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

    private void ChangeBackgroundColor_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        _fill = BackgroundColorPicker.Color;
        UpdateStyle();
    }

    private void GridColorPicker_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        _stroke = GridColorPicker.Color;
        UpdateStyle();
    }

    private void ShowGird_OnClick(object? sender, RoutedEventArgs e)
    {
        _lineWidth = _lineWidth > 0.01f ? 0.01f : 1f;
        UpdateStyle();
    }

    private void UpdateStyle()
    {
        var newFrame = new DrawMarginFrame();

        newFrame.Fill = new SolidColorPaint(_fill.ToSKColor());
        (this.DataContext as LiveCharts2DemoViewModel).XAxes[0].SeparatorsPaint =
            (this.DataContext as LiveCharts2DemoViewModel).YAxes[0].SeparatorsPaint =
            new SolidColorPaint(_stroke.ToSKColor(), _lineWidth);

        LvCharts.DrawMarginFrame = newFrame;
    }
}