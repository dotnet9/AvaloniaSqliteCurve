using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace AvaloniaSqliteCurve;

public partial class LiveCharts2Demo : Window
{
    public LiveCharts2Demo()
    {
        InitializeComponent();
        var myChart = this.FindControl<CartesianChart>("MyChart");
    }
}