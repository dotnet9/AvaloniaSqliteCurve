using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Linq;

namespace AvaloniaSqliteCurve.Models;

public class LiveLineModel
{
    public const int MaxPointCount = 100;
    private string _name;
    private Scatter _scatter;
    private double[]? _xs;
    private double[]? _ys;

    public Scatter? Scatter => _scatter;

    public LiveLineModel(Plot plot, string name)
    {
        _name = name;
        _xs = Enumerable.Repeat(DateTime.Now.ToOADate(), MaxPointCount).ToArray();
        _ys = Enumerable.Repeat(default(double), MaxPointCount).ToArray();
        _scatter = plot.Add.Scatter(_xs, _ys);
    }

    public void Update(DateTime updateTime, double value)
    {
        _scatter!.LegendText = $"{_name}: {value}";
        Array.Copy(_xs!, 1, _xs!, 0, _xs!.Length - 1);
        _xs[MaxPointCount - 1] = updateTime.ToOADate();
        Array.Copy(_ys!, 1, _ys!, 0, _ys!.Length - 1);
        _ys[MaxPointCount - 1] = value;
    }
}