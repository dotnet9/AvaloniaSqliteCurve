using AvaloniaSqliteCurve.Entities;
using AvaloniaSqliteCurve.Helpers;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services;

internal class DbService : IDbService
{
    private const string DefaultDbFolderName = "data";
    private static string _folder;

    static DbService()
    {
        _folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultDbFolderName);
        Directory.CreateDirectory(_folder);
    }

    public void ChangeDataFolder(string? folder)
    {
        if (string.IsNullOrWhiteSpace(folder)) return;

        _folder = folder;
        Directory.CreateDirectory(_folder);
    }


    public async Task<int> BulkInsertAsync(string pointName, List<PointValue> pointValues)
    {
        var connectionString = await CreateDbAndGetConnectionStringAsync(pointName);
        await using var connection = new SQLiteConnection(connectionString);
        await connection.OpenAsync();
        var transaction = connection.BeginTransaction();
        var sql =
            $"INSERT INTO PointValue(Value, Status, UpdateTime) VALUES(@Value, @Status, @UpdateTime)";
        var insertCount = await connection.ExecuteAsync(sql, pointValues, transaction: transaction);
        transaction.Commit();
        return insertCount;
    }

    public async Task<Dictionary<string, List<PointValue>?>> GetPointValues(List<string> names, DateTime startDateTime,
        DateTime endDateTime)
    {
        var allNameAndValues = new Dictionary<string, List<PointValue>?>();
        var tasks = Enumerable.Range(0, (endDateTime - startDateTime).Days + 1).Select(day =>
        {
            var currentDate = startDateTime.AddDays(day);
            var earlyStart = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            var latestEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59, 999);
            var currentDateStart = earlyStart > startDateTime ? earlyStart : startDateTime;
            var currentDateEnd = endDateTime < latestEnd ? endDateTime : latestEnd;
            var startTime = currentDateStart.ToTodayTimestamp();
            var endTime = currentDateEnd.ToTodayTimestamp();
            return OpenDbAndQueryAsync(currentDate, names, startTime, endTime);
        }).ToList();

        try
        {
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                foreach (var kvp in task.Result)
                {
                    var pointName = kvp.Key;
                    if (!allNameAndValues.ContainsKey(pointName))
                        allNameAndValues[pointName] = [];
                    if (kvp.Value?.Count > 0)
                    {
                        allNameAndValues[pointName]!.AddRange(kvp.Value);
                    }
                }
            }

            return allNameAndValues;
        }
        catch (FileNotFoundException ex)
        {
            return allNameAndValues;
        }
    }

    private async Task<Dictionary<string, List<PointValue>?>> OpenDbAndQueryAsync(DateTime dbCreateDate,
        List<string> names,
        int startTimestamp, int endTimestamp)
    {
        var nameAndValues = new Dictionary<string, List<PointValue>?>();

        try
        {
            foreach (var name in names)
            {
                try
                {
                    GetDbPathAndConnectionString(dbCreateDate, name, out var connectionString);
                    if (!await IsTableExistsAsync(connectionString, "PointValue"))
                    {
                        nameAndValues[name] = null;
                        continue;
                    }

                    await using var connection = new SQLiteConnection(connectionString);
                    var query =
                        $@"SELECT Id, Value, Status, UpdateTime FROM PointValue WHERE UpdateTime BETWEEN {startTimestamp} AND {endTimestamp}";
                    var dataValues = await connection.QueryAsync<PointValue>(query);
                    nameAndValues[name] = dataValues.ToList();
                }
                catch
                {
                    nameAndValues[name] = null;
                }
            }

            return nameAndValues;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    async Task<string> CreateDbAndGetConnectionStringAsync(string pointName)
    {
        GetDbPathAndConnectionString(DateTime.Now, pointName, out var connectionString);

        await using var connection = new SQLiteConnection(connectionString);
        await connection.ExecuteAsync($"CREATE TABLE IF NOT EXISTS PointValue(" +
                                      $"`Id` INTEGER," +
                                      $"`Value` DOUBLE," +
                                      $"`Status` TINYINT," +
                                      $"`UpdateTime` INTEGER," +
                                      $"CONSTRAINT Point_PK PRIMARY KEY (Id)" +
                                      $")");

        await connection.ExecuteAsync($"CREATE INDEX IF NOT EXISTS idx_UpdateTime ON PointValue (UpdateTime)");

        return connectionString;
    }

    async Task<bool> IsTableExistsAsync(string connectionString, string tableName)
    {
        await using var connection = new SQLiteConnection(connectionString);
        const string sqlQuery = $"SELECT count(1) FROM sqlite_master WHERE type='table' AND name=@tableName";
        var result = await connection.QuerySingleAsync<int>(sqlQuery, new { tableName });
        return result > 0;
    }

    bool GetDbPathAndConnectionString(DateTime date, string pointName, out string connectionString)
    {
        var dbDir = Path.Combine(_folder, $"{date:yyyy-MM-dd}");
        if (!Directory.Exists(dbDir))
        {
            Directory.CreateDirectory(dbDir);
        }


        var dbPath = Path.Combine(dbDir, pointName);
        connectionString = $"Data Source=\"{dbPath}\";Version=3;New=False;Compress=True;";
        return System.IO.File.Exists(dbPath);
    }
}