using AvaloniaSqliteCurve.Extensions;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.ViewModels
{
    internal class LiveCharts2DemoViewModel : ViewModelBase
    {
        public const int EverLinePointCount = 1000;
        public const int IntervalMilliseconds = 100;
        public const double MinBottom = -300.0;
        public const double MaxTop = 300.0;
        private const int LineCount = 16;
        private readonly Dictionary<int, RangeObservableCollection<DateTimePoint?>> _values = new();

        public ObservableCollection<ISeries> Series { get; } = new();

        private Axis[] _axis =
        [
            new DateTimeAxis(TimeSpan.FromMinutes(1), date => $"00"),
        ];

        public Axis[] XAxes
        {
            get => _axis;
            set => this.RaiseAndSetIfChanged(ref _axis, value);
        }

        public Axis[] YAxes { get; } =
        [
            new Axis
            {
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
                IsVisible = false,
                Labeler = Labelers.Default,
                LabelsPaint = new SolidColorPaint
                {
                    Color = SKColors.Blue,
                    FontFamily = "Times New Roman",
                    SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Italic)
                },
                MinLimit = MinBottom,
                MaxLimit = MaxTop
            }
        ];

        private DateTime _startDate = new(2024, 08, 27);

        public DateTime StartDate
        {
            get => _startDate;
            set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }

        private TimeSpan _startTime = new(8, 0, 0);

        public TimeSpan StartTime
        {
            get => _startTime;
            set => this.RaiseAndSetIfChanged(ref _startTime, value);
        }

        private DateTime _endDate = new(2024, 08, 27);

        public DateTime EndDate
        {
            get => _endDate;
            set => this.RaiseAndSetIfChanged(ref _endDate, value);
        }

        private TimeSpan _endTime = new(12, 0, 0);

        public TimeSpan EndTime
        {
            get => _endTime;
            set => this.RaiseAndSetIfChanged(ref _endTime, value);
        }

        public DateTime StartDateTime =>
            new(StartDate.Year, StartDate.Month, StartDate.Day,
                StartTime.Hours, StartTime.Minutes, StartTime.Seconds);

        public DateTime EndDateTime =>
            new(EndDate.Year, EndDate.Month, EndDate.Day,
                EndTime.Hours, EndTime.Minutes, EndTime.Seconds);

        public void RaiseChangeDataCommand()
        {
            Task.Factory.StartNew(UpdateData);
        }

        private void UpdateData()
        {
            var st = Stopwatch.StartNew();

            // 1、生成曲线
            if (Series.Count <= 0)
            {
                for (var i = 0; i < LineCount; i++)
                {
                    var seriesValues = new RangeObservableCollection<DateTimePoint?>();
                    _values[i] = seriesValues;
                    Series.Add(new LineSeries<DateTimePoint?>
                    {
                        Values = seriesValues,
                        Fill = null,
                        GeometryFill = null,
                        GeometryStroke = null,
                        LineSmoothness = 0,
                        Stroke = new SolidColorPaint(new SKColor(255, 0, 0)) { StrokeThickness = 1 }
                    });
                }
            }

            st.Stop();
            Debug.WriteLine($"生成曲线耗时：{st.ElapsedMilliseconds}ms");

            // 2、清空曲线数据
            st.Restart();
            foreach (var value in _values)
            {
                value.Value.Clear();
            }

            st.Stop();
            Debug.WriteLine($"清空曲线耗时：{st.ElapsedMilliseconds}ms");

            // 3、生成数据
            st.Restart();
            Dictionary<int, List<DateTimePoint?>> dataList = new();
            for (var i = 0; i < LineCount; i++)
            {
                dataList[i] = new List<DateTimePoint?>();
            }

            for (var i = 0; i < LineCount; i++)
            {
                dataList[i].AddRange(GeneratePoints(StartDateTime, EndDateTime));
            }

            st.Stop();
            Debug.WriteLine($"生成数据耗时：{st.ElapsedMilliseconds}ms");

            // 4、更新UI
            st.Restart();
            foreach (var data in dataList)
            {
                _values[data.Key].Add(data.Value);
            }

            st.Stop();
            Debug.WriteLine($"绑定曲线耗时：{st.ElapsedMilliseconds}ms");
        }

        private static List<DateTimePoint?> GeneratePoints(DateTime startDate, DateTime endDate)
        {
            List<DateTimePoint?> datas = [];

            for (var currentTime = startDate;
                 currentTime < endDate;
                 currentTime = currentTime.AddMilliseconds(IntervalMilliseconds))
            {
                if (currentTime.Millisecond % 17 == 0)
                {
                    datas.Add(null);
                }
                else if (currentTime.Millisecond % 17 == 1)
                {
                    datas.Add(new DateTimePoint(currentTime, Random.Shared.Next(-1000, 1000)));
                }
                else
                {
                    datas.Add(new DateTimePoint(currentTime, Random.Shared.Next(-300, 300)));
                }
            }

            return datas.Count <= EverLinePointCount ? datas : SamplePoints(datas, EverLinePointCount);
        }

        private static List<DateTimePoint?> SamplePoints(List<DateTimePoint?> allPoints, int sampleSize)
        {
            var samplePoints = new List<DateTimePoint?>();
            var step = allPoints.Count / sampleSize;
            for (var i = 0; i < allPoints.Count; i += step)
            {
                samplePoints.Add(allPoints[i]);
            }

            return samplePoints;
        }
    }
}