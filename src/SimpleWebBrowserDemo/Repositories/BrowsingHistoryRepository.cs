using Microsoft.Data.Sqlite;
using Dapper;
using SimpleWebBrowserDemo.Models;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Repositories;

public class BrowsingHistoryRepository : IBrowsingHistoryRepository
{
    private readonly string _connectionString;
    
    public BrowsingHistoryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM browsing_history ORDER BY visit_time DESC";
        return await connection.QueryAsync<BrowsingHistory>(sql);
    }
    
    public async System.Threading.Tasks.Task<BrowsingHistory?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM browsing_history WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<BrowsingHistory>(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> GetRecentAsync(int limit = 20)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM browsing_history ORDER BY visit_time DESC LIMIT @Limit";
        return await connection.QueryAsync<BrowsingHistory>(sql, new { Limit = limit });
    }
    
    public async System.Threading.Tasks.Task AddAsync(BrowsingHistory history)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            INSERT INTO browsing_history (url, title, visit_time, visit_count)
            VALUES (@Url, @Title, @VisitTime, @VisitCount)";
        await connection.ExecuteAsync(sql, history);
    }
    
    public async System.Threading.Tasks.Task UpdateAsync(BrowsingHistory history)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            UPDATE browsing_history 
            SET url = @Url, title = @Title, visit_count = @VisitCount
            WHERE id = @Id";
        await connection.ExecuteAsync(sql, history);
    }
    
    public async System.Threading.Tasks.Task DeleteAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM browsing_history WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task DeleteAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM browsing_history";
        await connection.ExecuteAsync(sql);
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> SearchAsync(string query)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            SELECT * FROM browsing_history 
            WHERE url LIKE @Query OR title LIKE @Query
            ORDER BY visit_time DESC
            LIMIT 50";
        return await connection.QueryAsync<BrowsingHistory>(sql, new { Query = $"%{query}%" });
    }
}
