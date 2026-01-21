using System;
using System.Threading.Tasks;
using SimpleWebBrowserDemo.Models;
using SimpleWebBrowserDemo.Repositories;

namespace SimpleWebBrowserDemo.Services;

public class UserSettingsService
{
    private readonly IUserSettingsRepository _repository;
    
    public UserSettingsService(IUserSettingsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<string?> GetAsync(string key)
    {
        return await _repository.GetAsync(key);
    }
    
    public async Task SetAsync(string key, string value)
    {
        await _repository.SetAsync(key, value);
        return;
    }
    
    public async Task<string?> GetHomePageAsync()
    {
        return await GetAsync("home_page");
    }
    
    public async Task SetHomePageAsync(string url)
    {
        await SetAsync("home_page", url);
        return;
    }
    
    public async Task<string?> GetSearchEngineAsync()
    {
        return await GetAsync("search_engine");
    }
    
    public async Task SetSearchEngineAsync(string url)
    {
        await SetAsync("search_engine", url);
        return;
    }
    
    public async Task<string?> GetThemeAsync()
    {
        return await GetAsync("theme");
    }
    
    public async Task SetThemeAsync(string theme)
    {
        await SetAsync("theme", theme);
        return;
    }
}
