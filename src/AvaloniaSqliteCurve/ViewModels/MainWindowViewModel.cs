using AvaloniaSqliteCurve.Entities;
using AvaloniaSqliteCurve.Services;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IFileChooserService? _fileChooserService;
    private INotificationService? _notificationService;
    private readonly IDbService _dbService = new DbService();
    private string? _dataDir;

    public string? DataDir
    {
        get => _dataDir;
        set => this.RaiseAndSetIfChanged(ref _dataDir, value);
    }

    private int _pointCount = 5000;

    public int PointCount
    {
        get => _pointCount;
        set => this.RaiseAndSetIfChanged(ref _pointCount, value);
    }

    private int _updateMilliseconds = 500;

    public int UpdateMilliseconds
    {
        get => _updateMilliseconds;
        set => this.RaiseAndSetIfChanged(ref _updateMilliseconds, value);
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
        this.WhenAnyValue(x => x.IsRunning).Subscribe(newValue =>
        {
            if (newValue)
            {
                RunningContent = "停止生成数据";
            }
            else
            {
                RunningContent = "开始生成数据";
            }
        });
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
                await CreateBasePointAsync();
            }
        }
        catch (Exception ex)
        {
            _notificationService!.Show(message: $"生成失败：{ex.Message}");
        }
    }

    private async Task CreateBasePointAsync()
    {
        if (PointCount <= 0)
        {
            return;
        }

        var pointCountStrLen = $"{PointCount}".Length;
        var intPoints = Enumerable.Range(0, PointCount).Select(index =>
            new Point($"IPoint{index.ToString($"D{pointCountStrLen}")}", (int)PointType.Integer)).ToList();
        var doublePoints = Enumerable.Range(0, PointCount).Select(index =>
            new Point($"DPoint{index.ToString($"D{pointCountStrLen}")}", (int)PointType.Double)).ToList();
        await _dbService.BulkInsertAsync(intPoints);
        await _dbService.BulkInsertAsync(doublePoints);
    }

    private async Task UpdateData(CancellationToken cancellationToken)
    {
        await Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(UpdateMilliseconds), cancellationToken);
            }
        }, cancellationToken);
    }
}