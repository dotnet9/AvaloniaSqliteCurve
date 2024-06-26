using System;
using AvaloniaSqliteCurve.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services
{
    internal interface IDbService
    {
        void ChangeDataFolder(string? folder);
        Task<int> BulkInsertAsync(string pointName, List<PointValue> pointValues);
        /// <summary>
        /// 查询点值
        /// 1. 存在跨多天情况，可能某天不存在Db文件（Db以存储数据当天时间格式化全名为【yyyy-MM-dd.db】
        /// 2. 查询时间范围存储跨天
        /// </summary>
        /// <param name="names">需要查询的点列表，每个库中数据表表名即是点名，库中可能不存在点对应的表</param>
        /// <param name="startDateTime">查询开始时间</param>
        /// <param name="endDateTime">查询结束时间</param>
        /// <returns>返回的查询结果，字典键为点名，如果有点没有数据，则字典值为NULL即可；字典值为点的值列表</returns>
        Task<Dictionary<string, List<PointValue>?>> GetPointValues(List<string> names, DateTime startDateTime, DateTime endDateTime);
    }
}