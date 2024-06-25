using AvaloniaSqliteCurve.Entities;
using System.Collections.ObjectModel;
using AvaloniaSqliteCurve.Models;

namespace AvaloniaSqliteCurve.ViewModels
{
    internal class PointChartViewModel : ViewModelBase
    {
        public ObservableCollection<PointModel> Points { get; } = new();
    }
}