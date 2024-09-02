using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaSqliteCurve.Views;

public partial class ChartsViews : UserControl
{
    public ChartsViews()
    {
        InitializeComponent();
    }

    private ScottPlotDataStreamerView GetChartsView()
    {
        return new ScottPlotDataStreamerView();
    }

    private GridSplitter GetGridSplitter()
    {
        return new GridSplitter
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Colors.Gray)
        };
    }

    public void SwitchToSingleView()
    {
        MainDockPanel.Children.Clear();
        MainDockPanel.Children.Add(GetChartsView());
    }

    public void SwitchToQuadView()
    {
        MainDockPanel.Children.Clear();

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Pixel));
        grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Pixel));
        grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));

        var topLeft = GetChartsView();
        Grid.SetRow(topLeft, 0);
        Grid.SetColumn(topLeft, 0);
        grid.Children.Add(topLeft);

        var topright = GetChartsView();
        Grid.SetRow(topright, 0);
        Grid.SetColumn(topright, 2);
        grid.Children.Add(topright);

        var bottomLeft = GetChartsView();
        Grid.SetRow(bottomLeft, 2);
        Grid.SetColumn(bottomLeft, 0);
        grid.Children.Add(bottomLeft);

        var bottomRight = GetChartsView();
        Grid.SetRow(bottomRight, 2);
        Grid.SetColumn(bottomRight, 2);
        grid.Children.Add(bottomRight);

        var gridSplitterVertical = GetGridSplitter();
        Grid.SetRow(gridSplitterVertical, 0);
        Grid.SetColumn(gridSplitterVertical, 1);
        Grid.SetRowSpan(gridSplitterVertical, 3);
        grid.Children.Add(gridSplitterVertical);

        var gridSplitterHorizontal = GetGridSplitter();
        Grid.SetRow(gridSplitterHorizontal, 1);
        Grid.SetColumn(gridSplitterHorizontal, 0);
        Grid.SetColumnSpan(gridSplitterHorizontal, 3);
        grid.Children.Add(gridSplitterHorizontal);

        MainDockPanel.Children.Add(grid);
    }
}