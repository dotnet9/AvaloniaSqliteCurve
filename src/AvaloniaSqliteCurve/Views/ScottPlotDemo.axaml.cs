using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
using System;
using ScottPlot;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotDemo : Window
{
    private static readonly string PlotFont = "Noto Sans TC"; //Segoe UI; Noto Sans; SimSun;Noto Mono;
    private static int _addCount = 0;

    static ScottPlotDemo()
    {
        PlotFont = ScottPlot.Fonts.Detect("实时曲线测试");
    }

    private bool _first = true;
    private Avalonia.Threading.DispatcherTimer? _timer;
    private const int LineCount = 20;
    private LiveLineModel[]? _lines;

    public ScottPlotDemo()
    {
        InitializeComponent();
        this.Loaded += MainView_Loaded;
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_first)
        {
            StartPlot();
        }

        _first = false;
    }

    private void StartPlot()
    {
        InitLines();
        if (_timer != null) return;
        _timer = new Avalonia.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += TimerElapsed;
        _timer.Start();
    }

    private void TimerElapsed(object? sender, EventArgs e)
    {
        _addCount++;
        var dt = DateTime.Now.AddMilliseconds(_addCount * 500);
        for (var i = 0; i < LineCount; i++)
        {
            _lines![i].Update(dt, Random.Shared.Next(-50000, 50000));
        }

        plot.Plot.Axes.AutoScale();
        plot.Refresh();
    }

    private void InitLines()
    {
        if (plot == null) return;

        plot.Plot.Axes.Title.Label.Text = "实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;
        plot.Plot.ShowLegend();
        plot.Plot.Legend.IsVisible = true;
        plot.Plot.Axes.DateTimeTicksBottom();
        plot.Plot.DataBackground.Color = ScottPlot.Colors.LightCyan;

        foreach (var linePattern in Enum.GetValues<LinePattern>())
        {
            ComboBoxGridLineType.Items.Add(linePattern);
        }

        _lines = new LiveLineModel[LineCount];
        var start = DateTime.Now;
        for (var i = 0; i < LineCount; i++)
        {
            _lines[i] = new LiveLineModel(plot.Plot, $"name{i}");
            AddLimit(Random.Shared.Next(-100, 100), Random.Shared.Next(300, 600), _lines[i].Scatter!.Color);
        }
    }

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

    private void ChangeBackgroundColor_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = BackgroundColorPicker.Color;
        plot.Plot.DataBackground.Color = selectedColor.ToScottPlotColor();
    }

    private void GridColorPicker_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = GridColorPicker.Color;
        plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = selectedColor.ToScottPlotColor();
    }

    private void ShowGird_OnClick(object? sender, RoutedEventArgs e)
    {
        plot.Plot.Grid.IsVisible = !plot.Plot.Grid.IsVisible;
    }

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
}