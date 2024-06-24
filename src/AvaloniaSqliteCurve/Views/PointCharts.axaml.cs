using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaSqliteCurve.Entities;
using AvaloniaSqliteCurve.Helpers;
using ScottPlot;
using ScottPlot.Avalonia;

namespace AvaloniaSqliteCurve.Views;

public partial class PointCharts : UserControl
{
    private AvaPlot plotView;
    private List<PointValue> dataPoints = new List<PointValue>();
    private System.Timers.Timer timer;

    public PointCharts()
    {
        InitializeComponent();

        this.plotView = this.FindControl<AvaPlot>("PlotView");
        SetUpChart();
        StartUpdatingData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void SetUpChart()
    {
        var plt = plotView.Plot; // 获取Plot对象以配置图表  
        plt.Title("实时折线图");
        plt.XLabel("时间 (HH:mm:ss)");
        plt.YLabel("值");

        // 初始化一个空的数据系列（如果需要）  
        //plt.AddSignal(...); // 这里通常不添加数据，因为我们稍后会更新它  

        // 配置X轴为时间（可选的轴格式化）  
        //plt.XAxis.DateTimeFormat(true); // 显示日期时间（如果需要日期）  
        //plt.XAxis.TickLabelRotation(45); // 旋转标签以便于阅读（可选）  
        plt.Axes.DateTimeTicksBottom();
        // 显示图表  
        plotView.Refresh();
    }

    private void StartUpdatingData()
    {
        timer = new System.Timers.Timer(1000); // 每秒更新一次数据  
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void TimerElapsed(Object source, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            // 假设我们有一个方法来获取新的PointValue  
            PointValue newDataPoint = GetNewDataPoint(); // 你需要实现这个方法  

            // 将新数据添加到数据列表中  
            dataPoints.Add(newDataPoint);

            // 限制数据点的数量，以防止图表变得过于庞大  
            int maxPoints = 100;
            if (dataPoints.Count > maxPoints)
                dataPoints.RemoveAt(0);

            // 提取时间和值到ScottPlot可以理解的数组中  
            double[] xs = dataPoints.Select(dp => (double)dp.UpdateTime).ToArray(); // 将DateTime转换为OADate  
            double[] ys = dataPoints.Select(dp => dp.Value).ToArray();

            // 获取Plot对象以更新图表  
            var plt = plotView.Plot;

            // 清除旧的信号（如果有的话）  
            plt.Clear();

            // 添加新的信号（折线图）  
            //plt.AddSignal(ys, sampleRate: 1); // sampleRate是采样率，这里假设每秒一个点  
            //plt.XAxis.Ticks(xs, dateTimeFormat: "HH:mm:ss"); // 设置X轴刻度为时间，并指定时间格式  

            // 配置X轴和Y轴的显示范围（可选）  
            // plt.AxisAuto(); // 自动缩放轴以适合数据（如果需要）  

            // 刷新图表以显示更新  
            plotView.Refresh();
        });
    }

    // 模拟获取新数据点的方法  
    private PointValue GetNewDataPoint()
    {
        // 这里只是模拟生成新数据，实际情况下你可能从某个数据源获取数据  
        return new PointValue
        {
            Id = 0, // 假设ID不重要或固定  
            PointId = dataPoints.Count, // 使用列表索引作为点ID（仅作为示例）  
            Value = Math.Sin(DateTime.Now.Second * 2 * Math.PI / 60), // 示例：每秒变化的正弦波  
            Status = 0, // 假设状态不重要或固定  
            UpdateTime = DateTime.Now.ToFileTimeUtc() // 当前时间作为更新时间  
        };
    }
}