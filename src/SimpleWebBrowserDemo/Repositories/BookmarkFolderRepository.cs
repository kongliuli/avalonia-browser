using Microsoft.Data.Sqlite;
using Dapper;
using SimpleWebBrowserDemo.Models;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Repositories;

public class BookmarkFolderRepository : IBookmarkFolderRepository
{
    private readonly string _connectionString;
    
    public BookmarkFolderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BookmarkFolder>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmark_folders ORDER BY order_index";
        return await connection.QueryAsync<BookmarkFolder>(sql);
    }
    
    public async System.Threading.Tasks.Task<BookmarkFolder?> GetByIdAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmark_folders WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<BookmarkFolder>(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task<BookmarkFolder?> GetRootFolderAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmark_folders WHERE parent_id IS NULL ORDER BY order_index";
        return await connection.QueryFirstOrDefaultAsync<BookmarkFolder>(sql);
    }
    
    public async System.Threading.Tasks.Task AddAsync(BookmarkFolder folder)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            INSERT INTO bookmark_folders (name, parent_id, created_at, order_index)
            VALUES (@Name, @ParentId, @CreatedAt, @OrderIndex)";
        await connection.ExecuteAsync(sql, folder);
    }
    
    public async System.Threading.Tasks.Task UpdateAsync(BookmarkFolder folder)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = @"
            UPDATE bookmark_folders 
            SET name = @Name, parent_id = @ParentId
            WHERE id = @Id";
        await connection.ExecuteAsync(sql, folder);
    }
    
    public async System.Threading.Tasks.Task DeleteAsync(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "DELETE FROM bookmark_folders WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }
    
    public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BookmarkFolder>> GetChildrenAsync(long parentId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        const string sql = "SELECT * FROM bookmark_folders WHERE parent_id = @ParentId ORDER BY order_index";
        return await connection.QueryAsync<BookmarkFolder>(sql, new { ParentId = parentId });
    }
}
