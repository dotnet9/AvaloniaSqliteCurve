using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaSqliteCurve.Commands;
using CodeWF.EventBus;

namespace AvaloniaSqliteCurve.Views;

public partial class PointCharts : UserControl
{
    public PointCharts()
    {
        InitializeComponent();
        EventBus.Default.Subscribe<ChangePlotChartsKindCommand>(ReceiveChangePlotChartsKindHandler);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async Task ReceiveChangePlotChartsKindHandler(ChangePlotChartsKindCommand command)
    {
        var grid = this.Find<Grid>("GridSingle");
        grid!.IsVisible = command.Kind != PlotChartsKind.FourRealtime;
    }
}