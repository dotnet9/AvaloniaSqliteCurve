using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using LiveChartsCore;

namespace AvaloniaSqliteCurve.Views;

public partial class LiveCharts2Demo : Window
{
    public LiveCharts2Demo()
    {
        InitializeComponent(); 
    }

    private void SaveChartsToImage_OnClick(object? sender, RoutedEventArgs e)
    {
        var target = LvCharts;
        var imagePath = "a.png";
        var dpi = 96;
        var width = target.Bounds.Width;
        var height = target.Bounds.Height;
        var pixelSize = new PixelSize((int)width, (int)height);
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
}