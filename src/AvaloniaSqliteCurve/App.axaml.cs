using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSqliteCurve.ViewModels;
using AvaloniaSqliteCurve.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace AvaloniaSqliteCurve
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LiveCharts.Configure(config =>
                config
                    .HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('บบ'))
                    .HasMap<City>((city, index) => new(index, city.Population))
            );
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public record City(string Name, double Population);
    }
}