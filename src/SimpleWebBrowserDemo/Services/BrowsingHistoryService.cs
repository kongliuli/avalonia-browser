using System;
using System.Threading.Tasks;
using SimpleWebBrowserDemo.Models;
using SimpleWebBrowserDemo.Repositories;

namespace SimpleWebBrowserDemo.Services;

public class BrowsingHistoryService
{
    private readonly IBrowsingHistoryRepository _repository;
    
    public BrowsingHistoryService(IBrowsingHistoryRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddVisitAsync(string url, string? title)
    {
        var history = new BrowsingHistory
        {
            Url = url,
            Title = title,
            VisitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VisitCount = 1
        };
        
        await _repository.AddAsync(history);
        return;
    }
    
    public async Task<System.Collections.Generic.IEnumerable<BrowsingHistory>> GetRecentAsync(int limit = 20)
    {
        return await _repository.GetRecentAsync(limit);
    }
    
    public async Task ClearAsync()
    {
        await _repository.DeleteAllAsync();
        return;
    }
}
