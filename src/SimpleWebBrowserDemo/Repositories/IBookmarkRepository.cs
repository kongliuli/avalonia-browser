using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.Repositories;

public interface IBookmarkRepository
{
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Bookmark>> GetAllAsync();
    System.Threading.Tasks.Task<Bookmark?> GetByIdAsync(long id);
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Bookmark>> GetByFolderAsync(long folderId);
    System.Threading.Tasks.Task AddAsync(Bookmark bookmark);
    System.Threading.Tasks.Task UpdateAsync(Bookmark bookmark);
    System.Threading.Tasks.Task DeleteAsync(long id);
    System.Threading.Tasks.Task DeleteAllAsync();
    System.Threading.Tasks.Task MoveToFolderAsync(long bookmarkId, long folderId);
}
