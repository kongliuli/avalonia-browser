using Microsoft.Data.Sqlite;
using Dapper;
using SimpleWebBrowserDemo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly string _connectionString;
    
    public UserSettingsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<string?> GetAsync(string key)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT value FROM user_settings WHERE key = @Key";
        return await connection.QueryFirstOrDefaultAsync<string?>(sql, new { Key = key });
    }
    
    public async Task SetAsync(string key, string value)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            INSERT OR REPLACE INTO user_settings (key, value, updated_at)
            VALUES (@Key, @Value, strftime('%s', 'now'))";
        await connection.ExecuteAsync(sql, new { Key = key, Value = value });
    }
    
    public async Task<IEnumerable<SimpleWebBrowserDemo.Models.UserSetting>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM user_settings";
        var result = await connection.QueryAsync<SimpleWebBrowserDemo.Models.UserSetting>(sql);
        return result ?? System.Linq.Enumerable.Empty<SimpleWebBrowserDemo.Models.UserSetting>();
    }
    
    public async Task DeleteAsync(string key)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM user_settings WHERE key = @Key";
        await connection.ExecuteAsync(sql, new { Key = key });
    }
}
