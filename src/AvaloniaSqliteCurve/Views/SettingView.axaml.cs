using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaSqliteCurve.Extensions;
using AvaloniaSqliteCurve.Models;
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

    public SettingView()
    {
        InitializeComponent();
        
        BackgroundColorPicker.Color = Avalonia.Media.Colors.Black;// ����ɫ
        GridColorPicker.Color = Avalonia.Media.Colors.White;// ����ɫ

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

        // ����X��Y�ȷ�
        for (var index = 1; index <= 7; index++)
        {
            ComboBoxXDivide.Items.Add(index);
            ComboBoxYDivide.Items.Add(index);
        }
        ComboBoxXDivide.SelectedItem = 5;
        ComboBoxYDivide.SelectedItem = 5;
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
        GridLineVisibleChanged?.Invoke(((ToggleSplitButton)sender!).IsChecked);
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
}