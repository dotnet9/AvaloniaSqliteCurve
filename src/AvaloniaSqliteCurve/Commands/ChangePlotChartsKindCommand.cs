using CodeWF.EventBus;

namespace AvaloniaSqliteCurve.Commands
{
    internal class ChangePlotChartsKindCommand : Command
    {
        public PlotChartsKind Kind { get; set; }
    }

    internal enum PlotChartsKind
    {
        History,
        SingleRealtime,
        FourRealtime
    }
}