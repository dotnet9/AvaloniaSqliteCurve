using AvaloniaSqliteCurve.Views;

namespace AvaloniaSqliteCurve.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public void ExecuteShowLiveCharts2DemoHandler()
    {
        new LiveCharts2Demo() { DataContext = new LiveCharts2DemoViewModel() }.Show();
    }

    public void ExecuteShowScottPlotDemoHandler()
    {
        new ScottPlotDemo().Show();
    }

    public void ExecuteShowScottPlotBigDataHandler()
    {
        new ScottPlotBigData().Show();
    }
}