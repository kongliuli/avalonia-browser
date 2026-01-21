using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleWebBrowserDemo.Services;

public class WebPageService
{
    private readonly HttpClient _httpClient;
    
    public WebPageService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
    
    public async Task<string?> GetPageContentAsync(string url)
    {
        try
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"加载页面失败: {ex.Message}", ex);
        }
        
        return null;
    }
    
    public async Task<string?> GetPageTitleAsync(string url)
    {
        try
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                
                var titleStart = content.IndexOf("<title>");
                var titleEnd = content.IndexOf("</title>");
                if (titleStart >= 0 && titleEnd > titleStart)
                {
                    return content.Substring(titleStart + 7, titleEnd - titleStart - 7);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"获取页面标题失败: {ex.Message}", ex);
        }
        
        return null;
    }
}
