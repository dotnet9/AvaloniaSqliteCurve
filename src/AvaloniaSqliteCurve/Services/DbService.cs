using Avalonia;
using AvaloniaSqliteCurve.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services;

internal class DbService : IDbService
{
    private const string DefaultDbFolderName = "data";
    private const string DefaultBaseDbName = "BaseInfo.db";
    private const string DefaultDataDbPrefixName = "Data_";
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

    public async Task<IEnumerable<int>> GetPointIdsAsync()
    {
        var connectionString = await CreateDbAndGetConnectionStringAsync(DataTypeKind.BaseDb);
        await using var connection = new SQLiteConnection(connectionString);
        const string sql = "SELECT Id FROM POINT";
        return await connection.QueryAsync<int>(sql);
    }


    public async Task BulkInsertAsync(List<Entities.Point> points)
    {
        var connectionString = await CreateDbAndGetConnectionStringAsync(DataTypeKind.BaseDb);
        await using var connection = new SQLiteConnection(connectionString);
        await connection.OpenAsync();
        var transaction = connection.BeginTransaction();
        const string sql = "INSERT INTO POINT(Name, Type) VALUES(@Name, @Type)";
        await connection.ExecuteAsync(sql, points, transaction: transaction);
        transaction.Commit();
    }

    public async Task BulkInsertAsync(List<PointValue> pointValues)
    {
        var connectionString = await CreateDbAndGetConnectionStringAsync(DataTypeKind.DataDb);
        await using var connection = new SQLiteConnection(connectionString);
        await connection.OpenAsync();
        var transaction = connection.BeginTransaction();
        const string sql =
            "INSERT INTO POINTVALUE(PointId, Value, Status, UpdateTime) VALUES(@PointId, @Value, @Status, @UpdateTime)";
        await connection.ExecuteAsync(sql, pointValues, transaction: transaction);
        transaction.Commit();
    }

    async Task<string> CreateDbAndGetConnectionStringAsync(DataTypeKind kind = DataTypeKind.DataDb)
    {
        string connectionString;
        if (kind == DataTypeKind.BaseDb)
        {
            var dbPath = Path.Combine(_folder, DefaultBaseDbName);
            connectionString = $"data source={dbPath}";
            if (File.Exists(dbPath)) return connectionString;

            await using var connection = new SQLiteConnection(connectionString);
            await connection.ExecuteAsync($"CREATE TABLE Point(" +
                                          $"`Id` INTEGER," +
                                          $"`Name` VARCHAR(50)," +
                                          $"`Type` INTEGER," +
                                          $"CONSTRAINT Point_PK PRIMARY KEY (Id)" +
                                          $")");
        }
        else
        {
            var dbPath = Path.Combine(_folder, $"{DefaultDataDbPrefixName}{DateTime.Now:yyyyMMdd}.db");
            connectionString = $"data source={dbPath}";
            if (File.Exists(dbPath)) return connectionString;

            await using var connection = new SQLiteConnection(connectionString);
            await connection.ExecuteAsync($"CREATE TABLE PointValue(" +
                                          $"`Id` INTEGER," +
                                          $"`PointId` INTEGER," +
                                          $"`Value` DOUBLE," +
                                          $"`Status` INTEGER," +
                                          $"`UpdateTime` INTEGER," +
                                          $"CONSTRAINT Point_PK PRIMARY KEY (Id)" +
                                          $")");
        }

        return connectionString;
    }
}

public enum DataTypeKind
{
    BaseDb,
    DataDb
}