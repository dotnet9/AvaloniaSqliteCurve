using AvaloniaSqliteCurve.Views;

namespace AvaloniaSqliteCurve.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public void ExecuteShowLiveCharts2DemoHandle()
    {
        new LiveCharts2Demo() { DataContext = new LiveCharts2DemoViewModel() }.Show();
    }

    public void ExecuteShowScottPlotDemoHandle()
    {
        new ScottPlotDemo().Show();
    }
}