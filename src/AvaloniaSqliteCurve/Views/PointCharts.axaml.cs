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
        var plt = plotView.Plot; // ��ȡPlot����������ͼ��  
        plt.Title("ʵʱ����ͼ");
        plt.XLabel("ʱ�� (HH:mm:ss)");
        plt.YLabel("ֵ");

        // ��ʼ��һ���յ�����ϵ�У������Ҫ��  
        //plt.AddSignal(...); // ����ͨ����������ݣ���Ϊ�����Ժ�������  

        // ����X��Ϊʱ�䣨��ѡ�����ʽ����  
        //plt.XAxis.DateTimeFormat(true); // ��ʾ����ʱ�䣨�����Ҫ���ڣ�  
        //plt.XAxis.TickLabelRotation(45); // ��ת��ǩ�Ա����Ķ�����ѡ��  
        plt.Axes.DateTimeTicksBottom();
        // ��ʾͼ��  
        plotView.Refresh();
    }

    private void StartUpdatingData()
    {
        timer = new System.Timers.Timer(1000); // ÿ�����һ������  
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void TimerElapsed(Object source, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            // ����������һ����������ȡ�µ�PointValue  
            PointValue newDataPoint = GetNewDataPoint(); // ����Ҫʵ���������  

            // ����������ӵ������б���  
            dataPoints.Add(newDataPoint);

            // �������ݵ���������Է�ֹͼ���ù����Ӵ�  
            int maxPoints = 100;
            if (dataPoints.Count > maxPoints)
                dataPoints.RemoveAt(0);

            // ��ȡʱ���ֵ��ScottPlot��������������  
            double[] xs = dataPoints.Select(dp => (double)dp.UpdateTime).ToArray(); // ��DateTimeת��ΪOADate  
            double[] ys = dataPoints.Select(dp => dp.Value).ToArray();

            // ��ȡPlot�����Ը���ͼ��  
            var plt = plotView.Plot;

            // ����ɵ��źţ�����еĻ���  
            plt.Clear();

            // ����µ��źţ�����ͼ��  
            //plt.AddSignal(ys, sampleRate: 1); // sampleRate�ǲ����ʣ��������ÿ��һ����  
            //plt.XAxis.Ticks(xs, dateTimeFormat: "HH:mm:ss"); // ����X��̶�Ϊʱ�䣬��ָ��ʱ���ʽ  

            // ����X���Y�����ʾ��Χ����ѡ��  
            // plt.AxisAuto(); // �Զ����������ʺ����ݣ������Ҫ��  

            // ˢ��ͼ������ʾ����  
            plotView.Refresh();
        });
    }

    // ģ���ȡ�����ݵ�ķ���  
    private PointValue GetNewDataPoint()
    {
        // ����ֻ��ģ�����������ݣ�ʵ�����������ܴ�ĳ������Դ��ȡ����  
        return new PointValue
        {
            Id = 0, // ����ID����Ҫ��̶�  
            PointId = dataPoints.Count, // ʹ���б�������Ϊ��ID������Ϊʾ����  
            Value = Math.Sin(DateTime.Now.Second * 2 * Math.PI / 60), // ʾ����ÿ��仯�����Ҳ�  
            Status = 0, // ����״̬����Ҫ��̶�  
            UpdateTime = DateTime.Now.ToFileTimeUtc() // ��ǰʱ����Ϊ����ʱ��  
        };
    }
}