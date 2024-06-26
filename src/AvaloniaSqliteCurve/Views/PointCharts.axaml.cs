using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaSqliteCurve.Commands;
using AvaloniaSqliteCurve.Entities;
using CodeWF.EventBus;

namespace AvaloniaSqliteCurve.Views;

public partial class PointCharts : UserControl
{
    public PointCharts()
    {
        InitializeComponent();
        EventBus.Default.Subscribe(this);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    [EventHandler]
    private async Task ReceiveKindHandler(ChangePlotChartsKindCommand command)
    {
        var grid = this.Find<Grid>("GridSingle");
        grid!.IsVisible = command.Kind != PlotChartsKind.FourRealtime;
    }

    [EventHandler]
    private async Task ReceiveDataHandler(ChangePlotChartsDataCommand command)
    {
        var grid = this.Find<Grid>("GridSingle");
        if (grid!.IsVisible)
        {
            var view = this.Find<PointChart>("ChartsSingle");
            if (command.PointValues != null)
            {
                view?.Update(command.PointValues!);
            }
        }
        else
        {
            const int groupCount = 4;
            var groups = new Dictionary<string, List<PointValue>?>[4];
            var i = 0;
            foreach (var kvp in command.PointValues)
            {
                groups[i % groupCount][kvp.Key] = kvp.Value;
                i++;
            }

            var view1 = this.Find<PointChart>("Charts1");
            if (groups[0] != null)
            {
                view1.Update(groups[0]!);
            }

            var view2 = this.Find<PointChart>("Charts2");
            if (groups[1] != null)
            {
                view1.Update(groups[1]!);
            }

            var view3 = this.Find<PointChart>("Charts3");
            if (groups[2] != null)
            {
                view1.Update(groups[2]!);
            }

            var view4 = this.Find<PointChart>("Charts4");
            if (groups[3] != null)
            {
                view1.Update(groups[3]!);
            }
        }
    }
}