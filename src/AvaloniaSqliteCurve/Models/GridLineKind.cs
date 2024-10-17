using System.ComponentModel;
using ScottPlot;

namespace AvaloniaSqliteCurve.Models;

public enum GridLineKind
{
    [Description("实线")] Solid,
    [Description("虚线")] Dashed,
    [Description("密集虚线")] DenselyDashed,
    [Description("点线")] Dotted
}

public static class GridLineKindExt
{
    public static LinePattern ToLinePattern(this GridLineKind kind)
    {
        return kind switch
        {
            GridLineKind.Solid => LinePattern.Solid,
            GridLineKind.Dashed => LinePattern.Dashed,
            GridLineKind.DenselyDashed => LinePattern.DenselyDashed,
            _ => LinePattern.Dotted
        };
    }
}