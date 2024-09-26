using Avalonia.Controls;
using AvaloniaSqliteCurve.ViewModels;

namespace AvaloniaSqliteCurve.Views;

public partial class LinePointDataView : UserControl
{
    public LinePointDataViewModel? ViewModel
    {
        get => DataContext as LinePointDataViewModel;
        set=> DataContext = value;
    }
    public LinePointDataView()
    {
        ViewModel = new LinePointDataViewModel();
        InitializeComponent();
    }
}