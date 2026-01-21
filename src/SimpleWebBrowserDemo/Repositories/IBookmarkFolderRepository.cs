using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.Repositories;

public interface IBookmarkFolderRepository
{
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BookmarkFolder>> GetAllAsync();
    System.Threading.Tasks.Task<BookmarkFolder?> GetByIdAsync(long id);
    System.Threading.Tasks.Task<BookmarkFolder?> GetRootFolderAsync();
    System.Threading.Tasks.Task AddAsync(BookmarkFolder folder);
    System.Threading.Tasks.Task UpdateAsync(BookmarkFolder folder);
    System.Threading.Tasks.Task DeleteAsync(long id);
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BookmarkFolder>> GetChildrenAsync(long parentId);
}
