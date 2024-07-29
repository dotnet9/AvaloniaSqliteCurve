using Avalonia.Controls;
using ScottPlot;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AvaloniaSqliteCurve;

public partial class AvaPlotWindow : Window
{
    private static string PlotFont = "Noto Sans TC"; //Segoe UI; Noto Sans; SimSun;Noto Mono;

    static AvaPlotWindow()
    {
        PlotFont = ScottPlot.Fonts.Detect("测试");
    }

    private class LineModel
    {
        public const int MaxPointCount = 100;
        private static Regex LabelReg = new Regex(@"(?<=:)\d+(\.\d+)?", RegexOptions.Compiled);

        public string? Label;

        //Windows Debug 100点之后 SignalXY:25FPS,Scatter:26FPS
        public ScottPlot.Plottables.Scatter? sca;
        public double[]? ys;
        public double[]? xs;

        public void Init(int i, Plot Plot, DateTime start)
        {
            Plot.Axes.Title.Label.FontName = PlotFont;
            xs = Enumerable.Repeat(start.ToOADate(), LineModel.MaxPointCount).ToArray();
            ys = new double[MaxPointCount];
            sca = Plot.Add.Scatter(xs, ys);
            Label = sca.Label = $"Speed速度{i}:0 RPM";
            Plot.Legend.IsVisible = true;
            Plot.Legend.Font.Name = PlotFont;
            Plot.Axes.DateTimeTicks(Edge.Bottom);
        }

        public void UpdateData(DateTime ts, double latestValue)
        {
            if (xs == null || ys == null || sca == null || Label == null) return;

            Array.Copy(xs, 1, xs, 0, xs.Length - 1);
            xs[LineModel.MaxPointCount - 1] = ts.ToOADate();
            // 使用正则表达式替换冒号后的数字
            string result = LabelReg.Replace(Label, latestValue.ToString("F1"));
            sca.Label = result;
            Array.Copy(ys, 1, ys, 0, ys.Length - 1);
            ys[LineModel.MaxPointCount - 1] = latestValue;
        }
    }

    private bool first = true;
    private Avalonia.Threading.DispatcherTimer? timer;
    private int LineCount = 20;
    private LineModel[]? Lines;

    public AvaPlotWindow()
    {
        InitializeComponent();
        this.Loaded += MainView_Loaded;
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (first)
        {
            StartPlot();
        }

        first = false;
    }

    private void StartPlot()
    {
        InitLines();
        if (timer == null)
        {
            timer = new Avalonia.Threading.DispatcherTimer(); // 每隔1秒触发一次
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerElapsed;
            timer.Start();
        }
    }

    private void InitLines()
    {
        if (plot == null) return;
        // PlotFont = Fonts.Detect("实时数据");
        plot.Plot.Axes.Title.Label.Text = "RealTime Data:实时数据";
        plot.Plot.Axes.Title.Label.FontName = PlotFont;
        plot.Plot.Axes.Title.IsVisible = true;
        plot.Plot.Title("RealTime Data:实时数据");
        plot.Plot.Clear();
        Lines = new LineModel[LineCount];
        DateTime start = DateTime.Now;
        for (int i = 0; i < LineCount; i++)
        {
            Lines[i] = new LineModel();
            Lines[i].Init(i + 1, plot.Plot, start);
        }

        plot.Plot.Benchmark.IsVisible = true;
    }

    public void UpdateData(DateTime ts, double[] data)
    {
        int Min = Math.Min(data.Length, Lines.Length);
        if (Min <= 0) return;

        for (int i = 0; i < Min; i++)
        {
            UpdateData(i, ts, data[i]);
        }

        Render();
    }

    private void UpdateData(int i, DateTime ts, double latestValue)
    {
        var tmp = Lines[i];
        tmp.UpdateData(ts, latestValue);
    }

    public void Render()
    {
        plot.Plot.Axes.AutoScale();
        plot.Refresh();
    }

    private void TimerElapsed(object? sender, EventArgs e)
    {
        //InvokeAsync(() =>
        double[] data = new double[LineCount];
        for (int i = 0; i < LineCount; i++)
        {
            data[i] = Generate.RandomData.RandomNumber(100);
        }

        UpdateData(DateTime.Now, data);
    }

    public void Dispose()
    {
        timer?.Stop();
    }
}