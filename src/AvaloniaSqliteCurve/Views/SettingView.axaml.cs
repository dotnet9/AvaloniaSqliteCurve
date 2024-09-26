using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaSqliteCurve.Models;
using CodeWF.Tools.Extensions;
using ScottPlot;
using System;

namespace AvaloniaSqliteCurve.Views;

public partial class SettingView : UserControl
{
    public event Action<Avalonia.Media.Color>? BackgroundColorChanged;
    public event Action<Avalonia.Media.Color>? GridLineColorChanged;
    public event Action<bool>? GridLineVisibleChanged;
    public event Action<LinePattern>? GridLineLinePatternChanged;
    public event Action<int>? XDisplayTimeRangeChanged;
    public event Action<int>? XDivideChanged;
    public event Action<int>? YDivideChanged;
    public event Action<double, double>? YRangeChanged;

    public SettingView()
    {
        InitializeComponent();

        BackgroundColorPicker.Color = ConstData.Fill; // ����ɫ
        GridColorPicker.Color = ConstData.Stroke; // ����ɫ

        // ����������
        foreach (var linePattern in Enum.GetValues<LinePattern>())
        {
            ComboBoxGridLineType.Items.Add(linePattern);
        }

        ComboBoxGridLineType.SelectedItem = LinePattern.Solid;

        // X����ʾʱ�䷶Χ
        var kinds = Enum.GetValues<DisplayTimeRangeKind>();
        foreach (var kind in kinds)
        {
            ComboBoxDisplayTimeRange.Items.Add(new ComboBoxItem()
                { Content = kind.GetDescription(), Tag = (int)kind });
        }

        ComboBoxDisplayTimeRange.SelectedIndex = 1;

        // ���X��Y�ȷ�
        for (var index = 1; index <= 7; index++)
        {
            ComboBoxXDivide.Items.Add(index);
            ComboBoxYDivide.Items.Add(index);
        }

        ComboBoxXDivide.SelectedItem = 5;
        ComboBoxYDivide.SelectedItem = 5;
    }

    public bool IsTimeRangeVisible
    {
        get => ComboBoxDisplayTimeRange.IsVisible;
        set => ComboBoxDisplayTimeRange.IsVisible = TxtTimeRange.IsVisible = value;
    }

    /// <summary>
    /// �޸ı���ɫ
    /// </summary>
    private void ChangeBackgroundColor_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = BackgroundColorPicker.Color;
        BackgroundColorChanged?.Invoke(selectedColor);
    }

    /// <summary>
    /// �޸���ɫ
    /// </summary>
    private void GridColorPicker_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var selectedColor = GridColorPicker.Color;
        GridLineColorChanged?.Invoke(selectedColor);
    }

    /// <summary>
    /// �Ƿ���ʾ�߿�
    /// </summary>
    private void ShowGird_OnClick(object? sender, RoutedEventArgs e)
    {
        GridLineVisibleChanged?.Invoke(((ToggleButton)sender!).IsChecked!.Value);
    }

    /// <summary>
    /// �޸�������
    /// </summary>
    private void ComboBoxGridLineType_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ComboBoxGridLineType.SelectionBoxItem is LinePattern pattern)
        {
            GridLineLinePatternChanged?.Invoke(pattern);
        }
    }

    /// <summary>
    /// �޸�X����ʾʱ�䷶Χ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayTimeRange_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not ComboBoxItem selectedItem) return;

        XDisplayTimeRangeChanged?.Invoke(int.Parse(selectedItem.Tag!.ToString()!));
    }

    /// <summary>
    /// �޸�X�ȷ�
    /// </summary>
    private void ComboBoxXDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not int selectedItem) return;
        XDivideChanged?.Invoke(selectedItem);
    }

    /// <summary>
    /// �޸�Y�ȷ�
    /// </summary>
    private void ComboBoxYDivide_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (((ComboBox)sender!).SelectedItem is not int selectedItem) return;
        YDivideChanged?.Invoke(selectedItem);
    }

    private void MinY_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ChangeYRange();
    }

    private void MaxY_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ChangeYRange();
    }

    private void ChangeYRange()
    {
        double.TryParse(this.FindControl<TextBox>("MinY").Text, out var minValue);
        double.TryParse(this.FindControl<TextBox>("MaxY").Text, out var maxValue);
        YRangeChanged?.Invoke(minValue, maxValue);
    }
}