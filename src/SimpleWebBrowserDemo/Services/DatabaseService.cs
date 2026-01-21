using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Services;

public class DatabaseService : IDisposable
{
    private readonly string _dbPath;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public DatabaseService()
    {
        var appDataFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(appDataFolder, "browser.db");
    }
    
    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            
            var initScriptPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "init.sql");
            
            if (File.Exists(initScriptPath))
            {
                var initScript = await File.ReadAllTextAsync(initScriptPath);
                var command = connection.CreateCommand();
                command.CommandText = initScript;
                await command.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
        return;
    }
    
    public async Task<SqliteConnection> GetConnectionAsync()
    {
        await _semaphore.WaitAsync();
        var connection = new SqliteConnection($"Data Source={_dbPath}");
        await connection.OpenAsync();
        return connection;
    }
    
    public void ReleaseConnection()
    {
        _semaphore.Release();
    }
    
    public string GetDatabasePath()
    {
        return _dbPath;
    }
    
    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}
