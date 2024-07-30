using Avalonia.Controls;
using ScottPlot.Avalonia;
using ScottPlot.AxisPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ScottPlot.DataGenerators;

namespace AvaloniaSqliteCurve;

public partial class ScottPlotDemo : Window
{
    public string Title => "Data Logger";
    public string Description => "Plots live streaming data as a growing line plot.";

    readonly Timer AddNewDataTimer = new() { Interval = 10, Enabled = true };
    readonly Timer UpdatePlotTimer = new() { Interval = 50, Enabled = true };

    private readonly List<ScottPlot.Plottables.DataLogger> _loggers = new();
    private readonly List<RandomWalker> _walkers = new();

    public ScottPlotDemo()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        double[] dataX = { 1, 2, 3, 4, 5 };
        double[] dataY = { 1, 4, 9, 16, 25 };

        AvaPlot avaPlot1 = this.Find<AvaPlot>("AvaPlot1");
        avaPlot1.Plot.Add.Scatter(dataX, dataY);
        avaPlot1.Refresh();

        // disable interactivity by default
        avaPlot1.Interaction.Disable();

        // create two loggers and add them to the plot

        RightAxis axis1 = (RightAxis)avaPlot1.Plot.Axes.Right;
        var lineCount = Random.Shared.Next(5, 16);
        for (var i = 0; i < lineCount; i++)
        {
            var logger = avaPlot1.Plot.Add.DataLogger();
            logger.Axes.YAxis = axis1;
            logger.ViewSlide();
            _loggers.Add(logger);

            _walkers.Add(new(Random.Shared.Next(1, 1000), multiplier: Random.Shared.Next(20, 1000)));
        }

        axis1.Color(_loggers.First().Color);

        AddNewDataTimer.Elapsed += (s, e) =>
        {
            for (var i = 0; i < lineCount; i++)
            {
                _loggers[i].Add(_walkers[i].Next(5));
            }
        };

        UpdatePlotTimer.Elapsed += (s, e) =>
        {
            if (_loggers.Any(logger => logger.HasNewData))
                avaPlot1.Refresh();
        };
    }
}