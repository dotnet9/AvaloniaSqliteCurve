using System.Collections.Generic;
using AvaloniaSqliteCurve.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaSqliteCurve.Models;

namespace AvaloniaSqliteCurve.ViewModels
{
    internal class PointChartViewModel : ViewModelBase
    {
        public ObservableCollection<PointModel> Points { get; } = new();

        public void Update(Dictionary<string, List<PointValue>> dataLst)
        {
            foreach (var (pointName, value) in dataLst)
            {
                var newData = value.OrderByDescending(item => item.UpdateTime).FirstOrDefault();
                var oldData = Points.FirstOrDefault(item => item.Name == pointName);
                if (oldData == null)
                {
                    Points.Add(new PointModel() { Name = pointName, Value = newData?.Value });
                }
                else
                {
                    oldData.Value = newData?.Value;
                    //oldData.UpdateTime = newData?.UpdateTime;
                }
            }
        }
    }
}