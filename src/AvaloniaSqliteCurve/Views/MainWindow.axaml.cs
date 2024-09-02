using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSqliteCurve.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var ctl = sender as ComboBox;
        var chartsView = this.FindControl<ChartsViews>("MyChartsView");
        if (ctl?.SelectedIndex == 0)
        {
            chartsView?.SwitchToSingleView();
        }
        else
        {
            chartsView?.SwitchToQuadView();
        }
    }
}