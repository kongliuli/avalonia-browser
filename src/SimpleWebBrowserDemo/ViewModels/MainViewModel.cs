using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleWebBrowserDemo.Services;
using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly BrowsingHistoryService _historyService;
    private readonly BookmarkService _bookmarkService;
    private readonly UserSettingsService _settingsService;
    private readonly WebPageService _webPageService;
    
    [ObservableProperty]
    private string _currentUrl = "https://www.bing.com";

    [ObservableProperty]
    private string _pageTitle = "Simple Web Browser";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _canGoBack;

    [ObservableProperty]
    private bool _canGoForward;
    
    [ObservableProperty]
    private ObservableCollection<BrowsingHistory> _browsingHistory = new();
    
    [ObservableProperty]
    private ObservableCollection<Bookmark> _bookmarks = new();
    
    [ObservableProperty]
    private ObservableCollection<BookmarkFolder> _bookmarkFolders = new();
    
    [ObservableProperty]
    private bool _showHistoryPanel;
    
    [ObservableProperty]
    private bool _showBookmarksPanel;
    
    [ObservableProperty]
    private bool _showSettingsPanel;
    
    [ObservableProperty]
    private string _homePage = "https://www.bing.com";
    
    [ObservableProperty]
    private string _searchEngine = "https://www.bing.com/search?q=";
    
    [ObservableProperty]
    private string _theme = "dark";
    
    [ObservableProperty]
    private BrowsingHistory? _selectedHistoryItem;
    
    [ObservableProperty]
    private Bookmark? _selectedBookmarkItem;
    
    [ObservableProperty]
    private bool _isLightTheme;
    
    [ObservableProperty]
    private bool _isDarkTheme;
    
    public MainViewModel(
        BrowsingHistoryService historyService,
        BookmarkService bookmarkService,
        UserSettingsService settingsService)
    {
        _historyService = historyService;
        _bookmarkService = bookmarkService;
        _settingsService = settingsService;
        _webPageService = new WebPageService();
        
        LoadSettingsAsync();
    }
    
    private async Task LoadSettingsAsync()
    {
        var homePage = await _settingsService.GetHomePageAsync();
        if (!string.IsNullOrEmpty(homePage))
        {
            HomePage = homePage;
        }
        
        var searchEngine = await _settingsService.GetSearchEngineAsync();
        if (!string.IsNullOrEmpty(searchEngine))
        {
            SearchEngine = searchEngine;
        }
        
        var theme = await _settingsService.GetThemeAsync();
        if (!string.IsNullOrEmpty(theme))
        {
            Theme = theme;
            IsLightTheme = theme == "light";
            IsDarkTheme = theme == "dark";
        }
    }

    [RelayCommand]
    private void Go()
    {
        if (string.IsNullOrWhiteSpace(CurrentUrl))
        {
            return;
        }

        var normalizedUrl = NormalizeUrl(CurrentUrl);
        CurrentUrl = normalizedUrl;
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Back()
    {
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private void Forward()
    {
    }

    [RelayCommand]
    private void Refresh()
    {
    }

    [RelayCommand]
    private void Home()
    {
        CurrentUrl = HomePage;
    }
    
    [RelayCommand]
    private void ShowHistory()
    {
        ShowHistoryPanel = true;
        ShowBookmarksPanel = false;
        ShowSettingsPanel = false;
    }
    
    [RelayCommand]
    private void ShowBookmarks()
    {
        ShowHistoryPanel = false;
        ShowBookmarksPanel = true;
        ShowSettingsPanel = false;
    }
    
    [RelayCommand]
    private void ShowSettings()
    {
        ShowHistoryPanel = false;
        ShowBookmarksPanel = false;
        ShowSettingsPanel = true;
    }
    
    [RelayCommand]
    private async Task AddBookmark()
    {
        var bookmark = new Bookmark
        {
            Url = CurrentUrl,
            Title = PageTitle,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            FolderId = 1
        };
        
        await _bookmarkService.AddAsync(bookmark);
        await LoadBookmarksAsync();
    }
    
    [RelayCommand]
    private async Task ClearHistory()
    {
        await _historyService.ClearAsync();
        await LoadHistoryAsync();
    }
    
    [RelayCommand]
    private async Task SaveHomePage()
    {
        await _settingsService.SetHomePageAsync(HomePage);
    }
    
    [RelayCommand]
    private async Task SaveSearchEngine()
    {
        await _settingsService.SetSearchEngineAsync(SearchEngine);
    }
    
    [RelayCommand]
    private async Task SetLightTheme()
    {
        IsLightTheme = true;
        IsDarkTheme = false;
        await _settingsService.SetThemeAsync("light");
    }
    
    [RelayCommand]
    private async Task SetDarkTheme()
    {
        IsLightTheme = false;
        IsDarkTheme = true;
        await _settingsService.SetThemeAsync("dark");
    }
    
    public async Task<string?> LoadPageContentAsync(string url)
    {
        try
        {
            var content = await _webPageService.GetPageContentAsync(url);
            if (string.IsNullOrEmpty(content))
            {
                return "页面加载失败，请检查网址是否正确。";
            }
            
            return FormatHtmlContent(content);
        }
        catch (Exception ex)
        {
            return $"加载页面时发生错误：{ex.Message}";
        }
    }
    
    public async Task<string?> GetPageTitleAsync(string url)
    {
        try
        {
            return await _webPageService.GetPageTitleAsync(url);
        }
        catch (Exception ex)
        {
            return $"获取页面标题失败：{ex.Message}";
        }
    }
    
    private string FormatHtmlContent(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        var plainText = html
            .Replace("<br>", "\n")
            .Replace("<br/>", "\n")
            .Replace("<p>", "\n\n")
            .Replace("</p>", "\n")
            .Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&amp;", "&");

        var lines = plainText.Split('\n');
        var formattedLines = new System.Collections.Generic.List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.Length > 0 && !trimmedLine.StartsWith("<") && !trimmedLine.EndsWith(">"))
            {
                formattedLines.Add(trimmedLine);
            }
        }

        return string.Join("\n", formattedLines);
    }
    
    private async Task LoadHistoryAsync()
    {
        var history = await _historyService.GetRecentAsync(50);
        BrowsingHistory.Clear();
        foreach (var item in history)
        {
            BrowsingHistory.Add(item);
        }
    }
    
    private async Task LoadBookmarksAsync()
    {
        var bookmarks = await _bookmarkService.GetAllAsync();
        Bookmarks.Clear();
        foreach (var bookmark in bookmarks)
        {
            Bookmarks.Add(bookmark);
        }
    }
    
    private string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return "about:blank";
        }

        url = url.Trim();

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("about:", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        if (url.Contains('.') && !url.Contains(' '))
        {
            return $"https://{url}";
        }

        return $"{SearchEngine}{Uri.EscapeDataString(url)}";
    }
}
