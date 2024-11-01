using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaSqliteCurve.Models;
using CodeWF.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaSqliteCurve.ViewModels;

public class LinePointDataViewModel : ViewModelBase
{
    public LinePointDataViewModel()
    {
        Points = Enumerable.Range(0, ConstData.LineCount).Select(index => new LinePoint()
        {
            Id = index + 1,
            Visible = true,
            LineColor = GetBrushFromResource(index),
            LineWidth = ConstData.DefaultLineWidth,
            Min = -10 * (index + 1),
            Max = 10 * (index + 1),
            WindowIndex = (int)DisplayWindows.First
        }).ToList();
    }

    public List<LinePoint> Points { get; }

    public List<int> LineWidths { get; } = Enumerable.Range(1, 7).ToList();

    public List<string> DisplayWindowNames { get; } =
        Enum.GetValues<DisplayWindows>().Select(e => e.GetDescription()).ToList();

    private Color GetBrushFromResource(int index)
    {
        var res = Application.Current?.FindResource($"Color{index}");
        if (res is Color color)
        {
            return color;
        }

        return Colors.Red;
    }
}