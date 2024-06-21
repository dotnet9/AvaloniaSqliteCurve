using AvaloniaSqliteCurve.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services
{
    internal interface IDbService
    {
        void ChangeDataFolder(string? folder);
        Task BulkInsertAsync(List<Point> points);
    }
}