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
    private List<int>? _allPointIds;
    private readonly List<int> _plotPointIds = [];
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

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.IsRunning).Subscribe(newValue => { RunningContent = newValue ? "停止生成数据" : "开始生成数据"; });
        this.WhenAnyValue(x => x.DataDir).Subscribe(_dbService.ChangeDataFolder);
        DataDir = System.IO.Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "Data");
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

    public async Task ExecuteRunningHandle()
    {
        IsRunning = !IsRunning;
        try
        {
            if (IsRunning)
            {
                _allPointIds = await CreateBasePointAsync();
                if (_allPointIds?.Count > 0)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    var token = _cancellationTokenSource.Token;
                    _longRunningTask = Task.Run(() => UpdateData(_allPointIds, token), token);
                }
                else
                {
                    IsRunning = false;
                }

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

    public async Task ExecuteChangeRealtimePointHandler()
    {
        if (_allPointIds == null)
        {
            _allPointIds =(await _dbService.GetPointIdsAsync()).ToList();
        }

        if (_allPointIds == null || _allPointIds.Count == 0)
        {
            _notificationService!.Show(message: $"没有数据，请先点击生成");
            return;
        }
        _plotPointIds.Clear();
        var selectedIndexes = new HashSet<int>();
        while (selectedIndexes.Count < PlotPointCount)
        {
            var index = Random.Shared.Next(_allPointIds!.Count);
            if (selectedIndexes.Add(index))
            {
                _plotPointIds.Add(_allPointIds[index]);
            }
        }

        var plotPoints = await _dbService.GetPointsAsync(_plotPointIds);
    }

    private async Task<List<int>?> CreateBasePointAsync()
    {
        if (PointCount <= 0)
        {
            _notificationService!.Show(message: $"请输入生成点数量！");
            return null;
        }

        var pointIDs = (await _dbService.GetPointIdsAsync()).ToList();
        if (pointIDs.Count > 0)
        {
            var pointCount = pointIDs.Count;
            _notificationService!.Show(message: pointCount != (PointCount * 2)
                ? $"已经生成了{pointCount}个点数据，如需重新生成，请手动删除DB再生成！"
                : $"已经生成了{pointCount}个点数据！");
            return pointIDs;
        }

        var pointCountStrLen = $"{PointCount}".Length;
        var intPoints = Enumerable.Range(0, PointCount).Select(index =>
            new Point($"IPoint{index.ToString($"D{pointCountStrLen}")}", (int)PointType.Integer)).ToList();
        var doublePoints = Enumerable.Range(0, PointCount).Select(index =>
            new Point($"DPoint{index.ToString($"D{pointCountStrLen}")}", (int)PointType.Double)).ToList();

        await _dbService.BulkInsertAsync(intPoints);
        await _dbService.BulkInsertAsync(doublePoints);

        _notificationService!.Show(message: $"生成{PointCount * 2}个点数据！");
        pointIDs = (await _dbService.GetPointIdsAsync()).ToList();

        return pointIDs;
    }

    private async Task UpdateData(List<int> pointIds, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var stopwatch = Stopwatch.StartNew();
            var randomValues = Enumerable.Range(0, 5).Select(index => Random.Shared.Next(0, 100)).ToList();
            var pointValues = pointIds.Select(id =>
            {
                var value = randomValues[id % randomValues.Count];
                return new PointValue()
                {
                    PointId = id, Value = value, Status = (byte)Random.Shared.Next(0, 7),
                    UpdateTime = DateTime.Now.ToTodayUtcTimestamp()
                };
            }).ToList();
            stopwatch.Stop();
            Console.WriteLine($"生成数据耗时：{stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Restart();
            await _dbService.BulkInsertAsync(pointValues);
            stopwatch.Stop();
            Console.WriteLine($"插入数据耗时：{stopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(TimeSpan.FromMilliseconds(UpdateMilliseconds), cancellationToken);
        }
    }
}