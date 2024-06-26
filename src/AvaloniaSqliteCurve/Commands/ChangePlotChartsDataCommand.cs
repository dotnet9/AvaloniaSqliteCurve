using CodeWF.EventBus;
using System.Collections.Generic;
using AvaloniaSqliteCurve.Entities;

namespace AvaloniaSqliteCurve.Commands
{
    internal class ChangePlotChartsDataCommand : Command
    {
        public Dictionary<string, List<PointValue>?>? PointValues { get; set; }
    }
}