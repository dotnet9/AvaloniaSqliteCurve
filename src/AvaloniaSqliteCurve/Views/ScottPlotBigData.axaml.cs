using System;
using Avalonia.Controls;
using ScottPlot.Plottables;
using System.Collections.Generic;
using System.Timers;
using Avalonia.Interactivity;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace AvaloniaSqliteCurve.Views;

public partial class ScottPlotBigData : Window
{
    private static readonly string PlotFont = "Noto Sans TC";
    private const int LineCount = 16;
    private const int PointsCount = 10000;
    private readonly Timer _addNewDataTimer = new(TimeSpan.FromMilliseconds(10));
    private readonly Timer _updateDataTimer = new(TimeSpan.FromMilliseconds(50));

    private const int MinutesInHour = 60;
    private const int MinutesInDay = 1440;

    private readonly List<DataStreamer> _streamers = new();

    static ScottPlotBigData()
    {
        PlotFont = ScottPlot.Fonts.Detect(" µ ±«˙œﬂ≤‚ ‘");
    }

    public ScottPlotBigData()
    {
        InitializeComponent();

        for (var i = 0; i < LineCount; i++)
        {
            var streamer = plot.Plot.Add.DataStreamer(PointsCount);
            streamer.ViewScrollLeft();
            _streamers.Add(streamer);
        }

        plot.Interaction.Disable();
        plot.Plot.Axes.SetLimitsY(bottom: -50, top: 50);
        plot.Plot.Axes.SetLimitsX(left: 1000, right: 0);

        _addNewDataTimer.Elapsed += AddNewDataHandler;
        _updateDataTimer.Elapsed += UpdateDataHandler;

        _addNewDataTimer.Start();
        _updateDataTimer.Start();
    }

    private void AddNewDataHandler(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < LineCount; i++)
        {
            _streamers[i].Add(Random.Shared.Next(-100, 300));
        }
    }

    private void UpdateDataHandler(object? sender, ElapsedEventArgs e)
    {
        if (_streamers.Count > 0 && _streamers[0].HasNewData)
        {
            plot.Refresh();
        }
    }


    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not ComboBoxItem selectedItem) return;

        var selectedTimeMinutes = double.Parse(selectedItem.Tag!.ToString()!);

        NumericManual ticks = new();
        ticks.AddMajor(10000, GetTimeStr(0) + " " + GetTimeUnit(selectedTimeMinutes));
        ticks.AddMajor(7500, GetTimeStr(selectedTimeMinutes / 4));
        ticks.AddMajor(5000, GetTimeStr(selectedTimeMinutes / 4 * 2));
        ticks.AddMajor(2500, GetTimeStr(selectedTimeMinutes / 4 * 3));
        ticks.AddMajor(0, GetTimeStr(selectedTimeMinutes));
        plot.Plot.Axes.Bottom.TickGenerator = ticks;
    }

    private string GetTimeStr(double minutes)
    {
        switch (minutes)
        {
            case < MinutesInHour:
            {
                var wholeMinutes = (int)minutes;
                var seconds = (int)((minutes - wholeMinutes) * 60);
                return $"{wholeMinutes}:{seconds:D2}";
            }
            case >= MinutesInHour and < MinutesInDay:
            {
                var hours = (int)(minutes / MinutesInHour);
                var remainingMinutes = (int)(minutes % MinutesInHour);
                return $"{hours}:{remainingMinutes:D2}";
            }
            default:
            {
                var days = (int)(minutes / MinutesInDay);
                var remainingMinutes = (int)(minutes % MinutesInDay);
                var hours = remainingMinutes / MinutesInHour;
                return $"{days}:{hours:D2}";
            }
        }
    }

    private string GetTimeUnit(double minutes)
    {
        return minutes switch
        {
            < MinutesInHour => "Min",
            < MinutesInDay => "Hour",
            _ => "Day"
        };
    }
}