using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSqliteCurve.Controls;

public partial class Chart : UserControl
{
    public Chart()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}