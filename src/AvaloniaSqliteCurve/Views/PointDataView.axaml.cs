using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using CodeWF.LogViewer.Avalonia;

namespace AvaloniaSqliteCurve.Views;

public partial class PointDataView : Window
{
    private ObservableCollection<DataRowViewModel> Rows { get; } = [];
    private List<string>? _pointNames;

    public PointDataView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        InitializeDataGrid();
        ReceiveData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeDataGrid()
    {
        // 假设启动时从某处获取点名  
        _pointNames = ["点名1", "点名2", "点名3", "点名4", "点名5", "点名6"];

        var dataGrid = this.FindControl<DataGrid>("PointDataGrid")!;
        dataGrid.Columns.Add(new DataGridTextColumn()
        {
            Header = "序号", Binding = new CompiledBindingExtension(new CompiledBindingPathBuilder()
                .Property(new ClrPropertyInfo(
                        nameof(DataRowViewModel.Index),
                        obj => ((DataRowViewModel)obj).Index,
                        (obj, val) => { },
                        typeof(int)),
                    PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
                )
                .Build())
        });
        dataGrid.Columns.Add(new DataGridTextColumn()
        {
            Header = "时间", Binding = new CompiledBindingExtension(new CompiledBindingPathBuilder()
                .Property(new ClrPropertyInfo(
                        nameof(DataRowViewModel.Timestamp),
                        obj => ((DataRowViewModel)obj).Timestamp,
                        (obj, val) => { },
                        typeof(string)),
                    PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
                )
                .Build())
        });
        foreach (var column in _pointNames.Select(name => new DataGridTextColumn
                     { Header = name, Binding = CreateBinding(name) }))
        {
            dataGrid?.Columns.Add(column);
        }

        if (dataGrid != null) dataGrid.ItemsSource = Rows;
    }

    private CompiledBindingExtension CreateBinding(string key)
    {
        return new CompiledBindingExtension(new CompiledBindingPathBuilder()
            .Property(new ClrPropertyInfo(
                    key,
                    obj =>
                    {
                        ((DataRowViewModel)obj).Values.TryGetValue(key, out var value);
                        return value;
                    },
                    (obj, val) => { },
                    typeof(double)),
                PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
            )
            .Build());
    }

    private CompiledBindingExtension CreateNormalBinding(string key)
    {
        return new CompiledBindingExtension(new CompiledBindingPathBuilder()
            .Property(new ClrPropertyInfo(
                    key,
                    obj => ((DataRowViewModel)obj).Index,
                    (obj, val) => { },
                    typeof(Index)),
                PropertyInfoAccessorFactory.CreateInpcPropertyAccessor
            )
            .Build());
    }

    private void ReceiveData()
    {
        var observable = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ =>
        {
            // 模拟接收数据  
            var points = _pointNames?.Select(name => new Point { Name = name, Value = DateTime.Now.Millisecond }).ToList();
            return new DataRowViewModel
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Values = points?.ToDictionary(p => p.Name, p => p.Value)
            };
        });

        observable.Subscribe(row =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Logger.Info($"添加数据: {row}");
                Rows.Insert(0, row);
                for (var i = 0; i < Rows.Count; i++)
                {
                    Rows[i].Index = i;
                }
            });
        });
    }
}

public class DataRowViewModel : INotifyPropertyChanged
{
    private int _index;
    private string _timestamp;
    private Dictionary<string, double> _values;

    public int Index
    {
        get { return _index; }
        set
        {
            _index = value;
            OnPropertyChanged();
        }
    }

    public string Timestamp
    {
        get { return _timestamp; }
        set
        {
            _timestamp = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<string, double> Values
    {
        get { return _values; }
        set
        {
            _values = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        var value = new StringBuilder();
        foreach (var v in Values)
        {
            value.Append($"{v.Key}: {v.Value}");
        }
        return $"Index: {Index}, Tiemstamp: {Timestamp}, Values: {value}";
    }
}

public class Point
{
    public string Name { get; set; } = null!;
    public double Value { get; set; }
}