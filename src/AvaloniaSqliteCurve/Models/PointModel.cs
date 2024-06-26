using System;
using AvaloniaSqliteCurve.ViewModels;
using ReactiveUI;

namespace AvaloniaSqliteCurve.Models
{
    internal class PointModel : ViewModelBase
    {
        public string Name { get; set; } = null!;
        private double? _value;

        public double? Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private DateTime? _updateTime;

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => this.RaiseAndSetIfChanged(ref _updateTime, value);
        }
    }
}