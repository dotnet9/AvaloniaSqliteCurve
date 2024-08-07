using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotBigData : Window
{
    private static readonly string PlotFont = "Noto Sans TC"; //Segoe UI; Noto Sans; SimSun;Noto Mono;
    private const int LineCount = 16;
    private const int MaxDataCount = 600;

    static ScottPlotBigData()
    {
        PlotFont = ScottPlot.Fonts.Detect("历史曲线测试");
    }

    public ScottPlotBigData()
    {
        InitializeComponent();
        InitLines();
    }

    private bool isDrawing = false;
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (isDrawing)
        {
            return;
        }
        isDrawing = true;
        plot.Plot.Clear();

        var allXS = new Dictionary<int, double[]>();
        var allYS = new Dictionary<int, double[]>();
        for (var i = 0; i < LineCount; i++)
        {
            allXS[i] = new double[MaxDataCount];
            allYS[i] = new double[MaxDataCount];
            plot.Plot.Add.Scatter(allXS[i], allYS[i]);
        }

        var dtNow = DateTime.Now;
        for (var i = 0; i < MaxDataCount; i++)
        {
            var pointTime = dtNow.AddMilliseconds(i * 500).ToOADate();
            for (var j = 0; j < LineCount; j++)
            {
                allXS[j][i] = pointTime;
                allYS[j][i] = Random.Shared.Next(-500, 500);
            }
        }

        plot.Plot.Axes.AutoScale();
        plot.Refresh();
        isDrawing = false;
    }

    private void InitLines()
    {
        if (plot == null) return;

        plot.Plot.Title("实时数据");
        plot.Plot.YLabel("实时值");
        plot.Plot.XLabel("时间");
        plot.Plot.Axes.Title.Label.Text = "实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;
        plot.Plot.ShowLegend();
        plot.Plot.Legend.IsVisible = true;
        plot.Plot.Axes.DateTimeTicksBottom();
    }
}