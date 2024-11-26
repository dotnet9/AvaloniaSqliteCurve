using Avalonia.Controls;
using Avalonia.Interactivity;
using ScottPlot.Avalonia;
using System;

namespace AvaloniaSqliteCurve.Views;

public partial class DataStreamerWithNaN : Window
{
    public DataStreamerWithNaN()
    {
        InitializeComponent();
    }

    public void RefreshPlot_OnClick(object? sender, RoutedEventArgs e)
    {
        var myPlot = this.FindControl<AvaPlot>("MyPlot");
        var chkCreateNaN = this.FindControl<CheckBox>("ChkBoxWithNaN");

        var valueCount = Random.Shared.Next(10, 200);
        myPlot.Plot.Clear();
        var streamer = myPlot.Plot.Add.DataStreamer(valueCount);
        for (var i = 0; i < valueCount; i++)
        {
            double value;
            if (chkCreateNaN.IsChecked == true && i % 6 >= 2)
            {
                value = double.NaN;
            }
            else
            {
                value = Random.Shared.Next(0, 100);
            }

            streamer.Data.Add(value);
        }

        myPlot.Refresh();
    }
}