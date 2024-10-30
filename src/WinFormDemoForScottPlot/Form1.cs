using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;
using ScottPlot;
using System.Timers;

namespace WinFormDemoForScottPlot
{
    public partial class Form1 : Form
    {
        private static readonly string PlotFont = "Noto Sans TC";
        private readonly System.Windows.Forms.Timer _addNewDataTimer = new(){Interval = ConstData.AddDataInterval , Enabled = true};
        private readonly System.Windows.Forms.Timer _updateDataTimer = new() { Interval = ConstData.UpdateDataInterval, Enabled = true };

        private int _displayMinuteRange = 5;
        private int _xDivide = 5;
        private int _yDivide = 5;

        private VerticalLine? _vLine;
        private Dictionary<int, ScottPlot.Plottables.Text> _streamerTexts = new();

        private readonly Dictionary<int, DataStreamer> _streamers = new();
        private readonly Dictionary<int, RightAxis> _rightAxes = new();

        static Form1()
        {
            PlotFont = ScottPlot.Fonts.Detect("ʵʱ���߲���");
        }

        public Form1()
        {
            InitializeComponent();

            plot.Plot.Axes.Title.Label.Text = "ʵʱ����";
            plot.Plot.Axes.Title.Label.FontName = PlotFont;
            plot.Plot.Axes.Title.IsVisible = true;
            plot.Plot.Axes.AntiAlias(false);
            plot.Plot.Axes.ContinuouslyAutoscale = false;
            plot.UserInputProcessor.Disable();


            notUpdate = false;

            // ��������
            plot.MouseClick += Plot_PointerPressed;

            CreateCharts();

            _addNewDataTimer.Tick += AddNewDataHandler;
            _updateDataTimer.Tick += UpdateDataHandler;

            _addNewDataTimer.Start();
            _updateDataTimer.Start();

            //SettingView_OnBackgroundColorChanged(MySettingView.BackgroundColorPicker.Color);
            //SettingView_OnGridLineColorChanged(MySettingView.GridColorPicker.Color);
            SettingView_OnXDivideChanged(_xDivide);
            SettingView_OnYDivideChanged(_yDivide);
        }

        private void Plot_PointerPressed(object? sender, MouseEventArgs e)
        {
            //var mousePos = e.GetPosition(plot);
            //var dataArea = plot.Plot.LastRender.DataRect;
            //var width = dataArea.Width;
            //var x = mousePos.X - dataArea.Left;
            //var ratio = x / width;
            //var pointCountIndex = (int)(ConstData.DisplayMaxPointsCount * ratio);
            //if (pointCountIndex < 0)
            //{
            //    pointCountIndex = 0;
            //}

            //if (pointCountIndex >= ConstData.DisplayMaxPointsCount)
            //{
            //    pointCountIndex = ConstData.DisplayMaxPointsCount - 1;
            //}

            //if (_vLine == null)
            //{
            //    _vLine = plot.Plot.Add.VerticalLine(pointCountIndex, pattern: LinePattern.Solid);
            //    _vLine.IsVisible = true;

            //    for (var i = 0; i < _streamers.Count; i++)
            //    {
            //        var value = _streamers[i].Data.Data[pointCountIndex];
            //        _streamerTexts[i] = CreateText(pointCountIndex, value);
            //        _streamerTexts[i].IsVisible = value != double.NaN;
            //    }
            //}
            //else
            //{
            //    _vLine.X = pointCountIndex;

            //    for (var i = 0; i < _streamers.Count; i++)
            //    {
            //        var value = _streamers[i].Data.Data[ConstData.DisplayMaxPointsCount - pointCountIndex];
            //        _streamerTexts[i].IsVisible = value != double.NaN;
            //        _streamerTexts[i].LabelText = value.ToString();
            //        _streamerTexts[i].Location = new Coordinates(pointCountIndex, value);
            //    }
            //}

            //MySettingView.UpdateMoreText($"DataRect=({dataArea}),x={x},index={pointCountIndex}");

            //plot.Refresh();
        }

        private ScottPlot.Plottables.Text CreateText(double x, double y)
        {
            var text = y.ToString();
            var txtSample = plot.Plot.Add.Text(text, x, y);

            txtSample.LabelFontSize = 14;
            txtSample.LabelFontName = Fonts.Detect(text); // this works
            txtSample.LabelStyle.SetBestFont(); // this also works
            txtSample.LabelFontColor = ScottPlot.Colors.DarkRed;
            return txtSample;
        }

        private void AddNewDataHandler(object? sender, EventArgs e)
        {
            for (var i = 0; i < ConstData.LineCount; i++)
            {
                var value = DateTime.Now.Ticks * (DateTime.Now.Ticks % 5 == 1 ? -1.0 : 1.0) % 300;
                _streamers[i].Add(value);
            }
        }

        private void UpdateDataHandler(object? sender, EventArgs e)
        {
            if (_streamers.Count > 0 && _streamers[0].HasNewData)
            {
                plot.Refresh();
            }
        }

        private bool notUpdate = true;

        private void Update()
        {
            if (notUpdate)
            {
                return;
            }

            CreateCharts();
        }

        private void CreateCharts()
        {
            plot.Plot.Clear();
            _streamers.Clear();
            for (var i = 0; i < ConstData.LineCount; i++)
            {
                //var point = PointListView.ViewModel!.Points[i];
                var streamer = plot.Plot.Add.DataStreamer(ConstData.DisplayMaxPointsCount);
                //streamer.Color = point.LineColor.Value.ToScottPlotColor();
                //streamer.LineWidth = point.LineWidth;
                streamer.ManageAxisLimits = false;
                streamer.ViewScrollLeft();
                _streamers[i] = streamer;
            }

            //plot.Plot.Axes.Left.IsVisible = false;
            plot.Plot.Axes.Right.IsVisible = true;
        }

        /// <summary>
        /// ���������ޱ�ǩ
        /// </summary>
        private void AddLimit(double min, double max, ScottPlot.Color color)
        {
            //var textColor = new SolidColorBrush(new Avalonia.Media.Color(color.A, color.R, color.G, color.B));
            //MinItems.Items.Add(new TextBlock()
            //{
            //    Text = $"{min}",
            //    Foreground = textColor
            //});
            //MaxItems.Items.Add(new TextBlock()
            //{
            //    Text = $"{max}",
            //    Foreground = textColor
            //});
        }

        //// �޸ı���ɫ
        //private void SettingView_OnBackgroundColorChanged(Color color)
        //{
        //    plot.Plot.FigureBackground.Color = Color.FromRgb(230, 232, 234).ToScottPlotColor();
        //    plot.Plot.DataBackground.Color = color.ToScottPlotColor();
        //}

        //// �޸ı�������ɫ
        //private void SettingView_OnGridLineColorChanged(Color color)
        //{
        //    plot.Plot.Grid.MajorLineColor = plot.Plot.Grid.MinorLineColor = color.ToScottPlotColor();
        //}

        // �޸ı����߿ɼ���
        private void SettingView_OnGridLineVisibleChanged(bool visible)
        {
            plot.Plot.Grid.IsVisible = visible;
        }

        //// �޸ı���������
        //private void SettingView_OnGridLineLinePatternChanged(GridLineKind pattern)
        //{
        //    plot.Plot.Grid.XAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        //    plot.Plot.Grid.XAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
        //    plot.Plot.Grid.YAxisStyle.MajorLineStyle.Pattern = pattern.ToLinePattern();
        //    plot.Plot.Grid.YAxisStyle.MinorLineStyle.Pattern = pattern.ToLinePattern();
        //}

        // �޸�X�ȷ�
        private void SettingView_OnXDivideChanged(int divide)
        {
            _xDivide = divide;
            ChangeXDivide();
        }

        // �޸�Y�ȷ�
        private void SettingView_OnYDivideChanged(int divide)
        {
            _yDivide = divide;
            ChangeYRange();
        }

        /// <summary>
        /// �޸�Y��������
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MySettingView_OnYRangeChanged(double min, double max)
        {
            ChangeYRange();
        }

        private void ChangeYRange()
        {
            // 1��ֻ��ʾ�Ҳ�Y��
            DivideOneRight();

            // ÿ����һ��Y��
            //EveryLineY();
        }

        private NumericManual? _yTicks;

        private void DivideOneRight()
        {
            if (_yTicks == null)
            {
                plot.Plot.Axes.Left.Min = plot.Plot.Axes.Right.Min = ConstData.MinBottom;
                plot.Plot.Axes.Left.Max = plot.Plot.Axes.Right.Max = ConstData.MaxTop;
                plot.Plot.Axes.Left.TickLabelStyle.IsVisible = plot.Plot.Axes.Right.TickLabelStyle.IsVisible = false;
                plot.Plot.Axes.Left.MajorTickStyle.Length = plot.Plot.Axes.Right.MajorTickStyle.Length = 0;
            }

            var range = ConstData.MaxTop - ConstData.MinBottom;
            var valueRangeOfOnePart = range / _yDivide;

            _yTicks = new NumericManual();
            for (var i = 0; i <= _yDivide; i++)
            {
                var position = ConstData.MinBottom + valueRangeOfOnePart * i;
                _yTicks.AddMajor(position, string.Empty);
            }

            plot.Plot.Axes.Left.TickGenerator =
                plot.Plot.Axes.Right.TickGenerator = _yTicks;

            AddLimit(ConstData.MinBottom, ConstData.MaxTop, ScottPlot.Colors.Red);
        }

        // �޸�X����ʾʱ�䷶Χ
        private void SettingView_OnXDisplayTimeRangeChanged(int displayMinuteRange)
        {
            _displayMinuteRange = displayMinuteRange;
            ChangeXDivide();
        }

        private void ChangeXDivide()
        {
            NumericManual ticks = new();
            var pointCountForOnePart = ConstData.DisplayMaxPointsCount * 1.0 / _xDivide;
            var minutesForOnePart = _displayMinuteRange * 1.0 / _xDivide;
            plot.Plot.Axes.Bottom.Min = 0;
            plot.Plot.Axes.Bottom.Max = ConstData.DisplayMaxPointsCount;
            for (var i = 0; i <= _xDivide; i++)
            {
                var minutesIndex = minutesForOnePart * i;
                var pointCountIndex = ConstData.DisplayMaxPointsCount - i * pointCountForOnePart;
                ticks.AddMajor(pointCountIndex,
                    i.ToString());
            }

            plot.Plot.Axes.Bottom.TickGenerator = ticks;
        }
    }
}