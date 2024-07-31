using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.ViewModels
{
    internal class PointChartViewModel : ViewModelBase
    {
        private readonly List<ObservableCollection<DateTimePoint?>> _values = new();
        private readonly DateTimeAxis _customAxis;

        public PointChartViewModel()
        {
            var lineCount = Random.Shared.Next(5, 36);
            Series = [];
            for (var i = 0; i < lineCount; i++)
            {
                var seriesValues = new ObservableCollection<DateTimePoint?>();
                _values.Add(seriesValues);
                Series.Add(new LineSeries<DateTimePoint?>
                {
                    Values = seriesValues,
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0
                });
            }

            _customAxis = new DateTimeAxis(TimeSpan.FromMinutes(1), date => $"{date:HH:mm:00}")
            {
                Name = "时间",
                AnimationsSpeed = TimeSpan.FromMilliseconds(100),
                SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
            };

            XAxes = [_customAxis];


            YAxes =
            [
                new Axis
                {
                    Name = "点值",
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

            _ = ReadData();
        }

        public ObservableCollection<ISeries> Series { get; set; }
        public Axis[] XAxes { get; set; }

        public Axis[] YAxes { get; set; }

        public object Sync { get; } = new object();

        public bool IsReading { get; set; } = true;

        private async Task ReadData()
        {
            while (IsReading)
            {
                await Task.Delay(100);

                lock (Sync)
                {
                    var time = DateTime.Now;
                    _values.ForEach(list =>
                    {
                        if (DateTime.Now.Microsecond % 3 == 1)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            list.Add(new DateTimePoint(time, Random.Shared.Next(-300, 600)));
                        }
                        if (list.Count > 30000)
                        {
                            list.RemoveAt(0);
                        }
                    });
                }
            }
        }
    }
}