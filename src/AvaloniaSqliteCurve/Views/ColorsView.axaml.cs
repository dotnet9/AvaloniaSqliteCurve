using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSqliteCurve.Views;

public partial class ColorsView : Window
{
    public ColorsView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}