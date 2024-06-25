using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaSqliteCurve.Entities;

namespace AvaloniaSqliteCurve.Views;

public partial class PointChart : UserControl
{
    public PointChart()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Update(List<Point> points)
    {

    }
}