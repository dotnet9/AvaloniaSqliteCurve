using AvaloniaSqliteCurve.Entities;
using CodeWF.EventBus;
using System.Collections.Generic;

namespace AvaloniaSqliteCurve.Commands
{
    internal class ChangePlotChartsDataCommand : Command
    {
        public List<Point> Points { get; set; }
    }
}