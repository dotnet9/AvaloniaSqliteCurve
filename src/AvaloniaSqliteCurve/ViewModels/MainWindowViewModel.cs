using AvaloniaSqliteCurve.Entities;
using AvaloniaSqliteCurve.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaSqliteCurve.Commands;
using AvaloniaSqliteCurve.Helpers;
using CodeWF.EventBus;

namespace AvaloniaSqliteCurve.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IFileChooserService? _fileChooserService;
    private INotificationService? _notificationService;
    private readonly IDbService _dbService = new DbService();
    private const string PointNamePrefix = "Point";
    private readonly List<string> _plotPointNames = [];
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _longRunningTask;
    private string? _dataDir;

    public string? DataDir
    {
        get => _dataDir;
        set => this.RaiseAndSetIfChanged(ref _dataDir, value);
    }

    private int _pointCount = 200000;

    public int PointCount
    {
        get => _pointCount;
        set => this.RaiseAndSetIfChanged(ref _pointCount, value);
    }

    private int _plotPointCount = 16;

    public int PlotPointCount
    {
        get => _plotPointCount;
        set => this.RaiseAndSetIfChanged(ref _plotPointCount, value);
    }

    private int _updateMilliseconds = 500;

    public int UpdateMilliseconds
    {
        get => _updateMilliseconds;
        set => this.RaiseAndSetIfChanged(ref _updateMilliseconds, value);
    }

    private bool _isShowRealtime = true;

    public bool IsShowRealtime
    {
        get => _isShowRealtime;
        set => this.RaiseAndSetIfChanged(ref _isShowRealtime, value);
    }

    private bool _isFourPlotCharts;

    public bool IsFourPlotCharts
    {
        get => _isFourPlotCharts;
        set => this.RaiseAndSetIfChanged(ref _isFourPlotCharts, value);
    }

    private bool _isRunning;

    public bool IsRunning
    {
        get => _isRunning;
        set => this.RaiseAndSetIfChanged(ref _isRunning, value);
    }

    private string? _runningContent = "开始生成数据";

    public string? RunningContent
    {
        get => _runningContent;
        set => this.RaiseAndSetIfChanged(ref _runningContent, value);
    }

    private string _startDate;

    public string StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
    }

    private string _endDate;

    public string EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }

    private string? _displayChartPoints;

    public string? DisplayChartPoints
    {
        get => _displayChartPoints;
        set => this.RaiseAndSetIfChanged(ref _displayChartPoints, value);
    }

    private string? _displayMsg;

    public string? DisplayMsg
    {
        get => _displayMsg;
        set => this.RaiseAndSetIfChanged(ref _displayMsg, value);
    }

    private static readonly char[] separator = new char[] { ',', '，' };

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.IsRunning).Subscribe(newValue => { RunningContent = newValue ? "停止生成数据" : "开始生成数据"; });
        this.WhenAnyValue(x => x.DataDir).Subscribe(_dbService.ChangeDataFolder);
        DataDir = System.IO.Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "Data");
        StartDate = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
        EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void SetTool(IFileChooserService fileChooserService, INotificationService notificationService)
    {
        _fileChooserService = fileChooserService;
        _notificationService = notificationService;
    }

    public async Task ExecuteChoiceFolderHandle()
    {
        var folders = await _fileChooserService!.OpenFolderPickerAsync("修改数据目录", false);
        if (folders?.Count > 0)
        {
            DataDir = folders[0];
        }
    }

    public async Task ExecuteOpenFolderHandle()
    {
        if (string.IsNullOrWhiteSpace(DataDir) ||
            !System.IO.Directory.Exists(DataDir))
        {
            _notificationService?.Show(message: "请选择正确的数据目录");
            return;
        }

        await _fileChooserService!.OpenFolderAsync(DataDir);
    }

    public void ExecuteShowLiveCharts2DemoHandle()
    {
        new LiveCharts2Demo().Show();
    }

    public void ExecuteShowScottPlotDemoHandle()
    {
        new ScottPlotDemo().Show();
    }

    public async Task ExecuteRunningHandle()
    {
        IsRunning = !IsRunning;
        try
        {
            if (IsRunning)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                _longRunningTask = Task.Run(() => UpdateData(token), token);

                return;
            }

            await _cancellationTokenSource!.CancelAsync();
            try
            {
                await _longRunningTask!;
            }
            catch (OperationCanceledException ex)
            {
                _notificationService!.Show(message: $"更新点任务取消");
            }

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _longRunningTask = null;
        }
        catch (Exception ex)
        {
            _notificationService!.Show(message: $"生成失败：{ex.Message}");
        }
    }

    public async Task ExecuteChangePlotChartKindHandler()
    {
        var kind = PlotChartsKind.History;
        if (IsShowRealtime)
        {
            kind = IsFourPlotCharts ? PlotChartsKind.FourRealtime : PlotChartsKind.SingleRealtime;
        }

        await EventBus.Default.PublishAsync(new ChangePlotChartsKindCommand()
            { Kind = kind });
    }

    public async Task ExecuteChangePlotChartPointHandler()
    {
        _plotPointNames.Clear();
        var selectedIndexes = new HashSet<int>();
        while (selectedIndexes.Count < PlotPointCount)
        {
            var index = Random.Shared.Next(PointCount);
            if (selectedIndexes.Add(index))
            {
                _plotPointNames.Add($"{PointNamePrefix}{index}");
            }
        }

        DisplayChartPoints = string.Join(",", _plotPointNames);
    }

    public async Task ExecuteSearchPointsHandler()
    {
        if (_plotPointNames?.Count > 0 == false)
        {
            if (!string.IsNullOrWhiteSpace(DisplayChartPoints))
            {
                _plotPointNames.AddRange(DisplayChartPoints.Split(separator));
            }
        }
        if (_plotPointNames?.Count > 0 == false)
        {
            _notificationService?.Show("请切换查询点！");
            return;
        }


        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            var startTime = DateTime.Parse(StartDate);
            var endTime = DateTime.Parse(EndDate);
            if (startTime >= endTime)
            {
                _notificationService?.Show("请正确设置查询起止时间！");
                return;
            }

            var pointValues = await _dbService.GetPointValues(_plotPointNames, startTime, endTime);
            var queryPointValueCount = pointValues?.Values.Sum(list => list?.Count);
            stopwatch.Stop();
            await EventBus.Default.PublishAsync(new ChangePlotChartsDataCommand { PointValues = pointValues });
            DisplayMsg = $"查询结果：{queryPointValueCount}条，耗时{stopwatch.ElapsedMilliseconds}ms";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            DisplayMsg = $"查询结果：耗时{stopwatch.ElapsedMilliseconds}ms，出错信息【{ex.Message}】";
        }
    }

    private async Task UpdateData(CancellationToken cancellationToken)
    {
        try
        {
            var randomValues = Enumerable.Range(0, 5).Select(index => Random.Shared.Next(0, 100)).ToList();
            const int bulkInsertCount = 5000;
            var onePointWriteCountInOneDay = (60 * 1000 / UpdateMilliseconds) * 60 * 24;


            for (var j = 0; j < onePointWriteCountInOneDay; j++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var insertCount = (j + bulkInsertCount <= onePointWriteCountInOneDay)
                    ? bulkInsertCount
                    : (onePointWriteCountInOneDay - j);

                var dataList = Enumerable.Range(j, insertCount).Select(index => new PointValue()
                {
                    Value = randomValues[index % randomValues.Count],
                    Status = (byte)Random.Shared.Next(0, 7),
                    UpdateTime = DateTime.Now.ToTodayTimestamp()
                }).ToList();
                await Task.WhenAll(Enumerable.Range(0, PointCount)
                    .Select(i => InsetData(i, dataList, cancellationToken)));

                j += insertCount;
            }
        }
        catch (Exception ex)
        {
            _notificationService!.Show(message: $"更新失败：{ex.Message}");
        }
    }

    private async Task InsetData(int pointIndex, List<PointValue> dataList, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _dbService.BulkInsertAsync($"{PointNamePrefix}{pointIndex}", dataList);
    }
}