using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.Repositories;

public interface IBrowsingHistoryRepository
{
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> GetAllAsync();
    System.Threading.Tasks.Task<BrowsingHistory?> GetByIdAsync(long id);
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> GetRecentAsync(int limit = 20);
    System.Threading.Tasks.Task AddAsync(BrowsingHistory history);
    System.Threading.Tasks.Task UpdateAsync(BrowsingHistory history);
    System.Threading.Tasks.Task DeleteAsync(long id);
    System.Threading.Tasks.Task DeleteAllAsync();
    System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> SearchAsync(string query);
}
