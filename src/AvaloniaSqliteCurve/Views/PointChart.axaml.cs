using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using AvaloniaSqliteCurve.Entities;
using AvaloniaSqliteCurve.ViewModels;

namespace AvaloniaSqliteCurve.Views;

public partial class PointChart : UserControl
{
    public PointChart()
    {
        InitializeComponent();
        this.DataContext = new PointChartViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void Update(Dictionary<string, List<PointValue>> points)
    {
        (this.DataContext as PointChartViewModel)?.Update(points);
    }
}