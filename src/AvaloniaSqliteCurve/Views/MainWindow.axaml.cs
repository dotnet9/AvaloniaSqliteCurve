using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaSqliteCurve.Services;
using AvaloniaSqliteCurve.ViewModels;

namespace AvaloniaSqliteCurve.Views
{
    public partial class MainWindow : Window
    {
        private IFileChooserService? fileChooserService;
        private INotificationService? notificationService;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (fileChooserService != null)
            {
                return;
            }

            var level = GetTopLevel(this);
            if (level == null)
            {
                return;
            }

            fileChooserService = new FileChooserService();
            notificationService = new NotificationService();
            fileChooserService.SetHostWindow(level);
            notificationService.SetHostWindow(level);
            ((MainWindowViewModel?)DataContext)?.SetTool(fileChooserService, notificationService);
        }
    }
}