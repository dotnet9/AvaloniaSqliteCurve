using Avalonia.Controls;
using Avalonia.Interactivity;
using CfPlot.Avalonia.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace CfPlot.Avalonia.Demo.Desktop
{
    public partial class MainWindow : Window
    {
        private const int DefaultLineCount = 1;
        private const int DefaultPointCount = 1000;
        private const int DefaultAddInterval = 50;
        private const int DefaultRefreshInterval = 10;

        private int _lineCount = 8;
        private int _pointCount = 3000;
        private int _addInterval = 50;
        private int _refreshInterval = 10;

        private readonly List<DataStreamer> _streamers = new();

        private Timer? _addTimer;
        private Timer? _refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                ReadParameters();
                CreateCharts();
                RefreshCharts();
                //TestRefresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Start Error£º{ex.Message}");
            }
        }

        private void Stop_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                _addTimer?.Stop();
                _refreshTimer?.Stop();

                _addTimer = null;
                _refreshTimer = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Top Error£º{ex.Message}");
            }
        }

        private void ReadParameters()
        {
            _lineCount = ReadParam(TxtLineCount, DefaultLineCount);
            _pointCount = ReadParam(TxtPointCount, DefaultPointCount);
            _addInterval = ReadParam(TxtAddInterval, DefaultAddInterval);
            _refreshInterval = ReadParam(TxtRefreshInterval, DefaultRefreshInterval);
        }

        private void CreateCharts()
        {
            for (var i = 0; i < _lineCount; i++)
            {
                var streamer = Plot.AddDataStreamer(_pointCount);
                _streamers.Add(streamer);
            }
        }

        private void RefreshCharts()
        {
            _addTimer = new() { Interval = _addInterval };
            var screenHeight = Screens.Primary?.Bounds.Height ?? 300;
            _addTimer.Elapsed += (s, e) =>
            {
                for (var i = 0; i < _lineCount; i++)
                {
                    var value = DateTime.Now.Ticks * (DateTime.Now.Ticks % 5 == 1 ? -1.0 : 1.0) % screenHeight;
                    _streamers[i].Add(value);
                }
            };

            _refreshTimer = new() { Interval = _refreshInterval };
            _refreshTimer.Elapsed += (s, e) =>
            {
                if (_streamers.Any())
                {
                    Plot.Refresh();
                }
            };

            _addTimer.Start();
            _refreshTimer.Start();
        }

        private void TestRefresh()
        {
            for (var i = 0; i < _lineCount; i++)
            {
                for (var j = 0; j < 1000; j++)
                {
                    var value = DateTime.Now.Ticks * (DateTime.Now.Ticks % 5 == 1 ? -1.0 : 1.0) % 300;
                    _streamers[i].Add(value);
                }
            }
            if (_streamers.Any())
            {
                Plot.Refresh();
            }
        }

        private int ReadParam(TextBox txt, int defaultValue)
        {
            int.TryParse(txt.Text, out var value);
            return value <= 0 ? defaultValue : value;
        }
    }
}