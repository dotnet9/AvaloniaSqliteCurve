using AvaloniaSqliteCurve.Views;

namespace AvaloniaSqliteCurve.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public void ExecuteShowLiveCharts2DemoHandler()
    {
        new LiveCharts2Demo() { DataContext = new LiveCharts2DemoViewModel() }.Show();
    }

    public void ExecuteShowScottPlotDataLoggerHandler()
    {
        new ScottPlotDataLoggerDemo().Show();
    }

    public void ExecuteShowScottPlotDataStreamerHandler()
    {
        new ScottPlotDataStreamerDemo().Show();
    }

    public void ExecuteShowScottPlotSignalHandler()
    {
        new ScottPlotSignalDemo().Show();
    }

    public void ExecuteShowCustomPlotHandler()
    {
        new CustomCursorView().Show();
    }

    public void ShowDataViewHandler()
    {
        new PointDataView().Show();
    }
}