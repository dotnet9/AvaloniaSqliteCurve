using AvaloniaSqliteCurve.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services
{
    internal interface IDbService
    {
        void ChangeDataFolder(string? folder);
        Task<IEnumerable<int>> GetPointIdsAsync();
        Task BulkInsertAsync(List<Point> points);
        Task BulkInsertAsync(List<PointValue> pointValues);
        Task<List<Point>> GetPointsAsync(List<int> pointIds);
    }
}