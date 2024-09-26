using System;
using Avalonia.Media;
using AvaloniaSqliteCurve.ViewModels;
using ReactiveUI;

namespace AvaloniaSqliteCurve.Models;

public class LinePoint : ViewModelBase
{
    public int Id { get; set; }
    private DateTime? _date;

    public DateTime? Date
    {
        get => _date;
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }

    private double _value;

    public double Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    private double _cursorValue;

    public double CursorValue
    {
        get => _cursorValue;
        set => this.RaiseAndSetIfChanged(ref _cursorValue, value);
    }


    private bool _visible;

    public bool Visible
    {
        get => _visible;
        set => this.RaiseAndSetIfChanged(ref _visible, value);
    }

    private Color? _lineColor;

    public Color? LineColor
    {
        get => _lineColor;
        set => this.RaiseAndSetIfChanged(ref _lineColor, value);
    }

    private int _lineWidth;

    public int LineWidth
    {
        get => _lineWidth;
        set => this.RaiseAndSetIfChanged(ref _lineWidth, value);
    }

    private double _min;

    public double Min
    {
        get => _min;
        set => this.RaiseAndSetIfChanged(ref _min, value);
    }

    private double _max;

    public double Max
    {
        get => _max;
        set => this.RaiseAndSetIfChanged(ref _max, value);
    }

    private int _windowIndex;

    public int WindowIndex
    {
        get => _windowIndex;
        set => this.RaiseAndSetIfChanged(ref _windowIndex, value);
    }
}