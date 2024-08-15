using AvaloniaSqliteCurve.Extensions;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Platform;
using LiveChartsCore.Defaults;

namespace AvaloniaSqliteCurve.ViewModels
{
    internal class LiveCharts2DemoViewModel : ViewModelBase
    {
        private const int EverLinePointCount = 100;
        private const int LineCount = 16;
        private readonly Dictionary<int, RangeObservableCollectionT<DateTimePoint?>> _values = new();

        public ObservableCollection<ISeries> Series { get; } = new();

        public Axis[] XAxes { get; } =
        [
            new DateTimeAxis(TimeSpan.FromMinutes(1), date => $"{date:HH:mm}:00")
        ];

        public Axis[] YAxes { get; } =
        [
            new Axis
            {
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
                Labeler = Labelers.Default,
                LabelsPaint = new SolidColorPaint
                {
                    Color = SKColors.Blue,
                    FontFamily = "Times New Roman",
                    SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Italic)
                },
            }
        ];

        public void RaiseChangeDataCommand()
        {
            Task.Factory.StartNew(UpdateData);
        }

        private void UpdateData()
        {
            var st = Stopwatch.StartNew();

            if (Series.Count <= 0)
            {
                for (var i = 0; i < LineCount; i++)
                {
                    var seriesValues = new RangeObservableCollectionT<DateTimePoint?>();
                    _values[i] = seriesValues;
                    Series.Add(new LineSeries<DateTimePoint?>
                    {
                        Values = seriesValues,
                        Fill = null,
                        GeometryFill = null,
                        GeometryStroke = null,
                        LineSmoothness = 0,
                        Stroke = new SolidColorPaint(new SKColor(255, 0, 0)){StrokeThickness = 1}
                    });
                }
            }

            st.Stop();
            Debug.WriteLine($"生成曲线耗时：{st.ElapsedMilliseconds}ms");

            st.Restart();
            foreach (var value in _values)
            {
                value.Value.Clear();
            }

            st.Stop();
            Debug.WriteLine($"清空曲线耗时：{st.ElapsedMilliseconds}ms");

            st.Restart();
            Dictionary<int, List<DateTimePoint?>> dataList = new();
            for (var i = 0; i < LineCount; i++)
            {
                dataList[i] = new List<DateTimePoint?>();
            }

            var time = DateTime.Now;
            for (var j = 0; j < EverLinePointCount; j++)
            {
                var currentTime = time.AddMilliseconds(j * 500*60);
                for (var i = 0; i < LineCount; i++)
                {
                    dataList[i].Add(Random.Shared.Next(1, 1000) % 9 == 1
                        ? default
                        : new DateTimePoint(currentTime, Random.Shared.Next(-300, 600)));
                }
            }

            st.Stop();
            Debug.WriteLine($"生成数据耗时：{st.ElapsedMilliseconds}ms");

            st.Restart();
            foreach (var data in dataList)
            {
                _values[data.Key].Add(data.Value);
            }

            st.Stop();
            Debug.WriteLine($"绑定曲线耗时：{st.ElapsedMilliseconds}ms");
        }
    }
}