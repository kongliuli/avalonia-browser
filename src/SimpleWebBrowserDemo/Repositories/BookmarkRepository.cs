using Microsoft.Data.Sqlite;
using Dapper;
using SimpleWebBrowserDemo.Models;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly string _connectionString;
    
    public BookmarkRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Bookmark>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmarks ORDER BY order_index";
        return await connection.QueryAsync<Bookmark>(sql);
    }
    
    public async System.Threading.Tasks.Task<Bookmark?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmarks WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Bookmark>(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Bookmark>> GetByFolderAsync(long folderId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmarks WHERE folder_id = @FolderId ORDER BY order_index";
        return await connection.QueryAsync<Bookmark>(sql, new { FolderId = folderId });
    }
    
    public async System.Threading.Tasks.Task AddAsync(Bookmark bookmark)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            INSERT INTO bookmarks (url, title, folder_id, created_at, order_index)
            VALUES (@Url, @Title, @FolderId, @CreatedAt, @OrderIndex)";
        await connection.ExecuteAsync(sql, bookmark);
    }
    
    public async System.Threading.Tasks.Task UpdateAsync(Bookmark bookmark)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            UPDATE bookmarks 
            SET url = @Url, title = @Title, folder_id = @FolderId, description = @Description
            WHERE id = @Id";
        await connection.ExecuteAsync(sql, bookmark);
    }
    
    public async System.Threading.Tasks.Task DeleteAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM bookmarks WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task DeleteAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM bookmarks";
        await connection.ExecuteAsync(sql);
    }
    
    public async System.Threading.Tasks.Task MoveToFolderAsync(long bookmarkId, long folderId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "UPDATE bookmarks SET folder_id = @FolderId WHERE id = @BookmarkId";
        await connection.ExecuteAsync(sql, new { BookmarkId = bookmarkId, FolderId = folderId });
    }
}
