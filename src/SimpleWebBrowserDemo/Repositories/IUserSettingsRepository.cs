using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.Repositories;

public interface IUserSettingsRepository
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task<IEnumerable<UserSetting>> GetAllAsync();
    Task DeleteAsync(string key);
}
