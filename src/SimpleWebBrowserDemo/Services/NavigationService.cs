using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleWebBrowserDemo.Models;

namespace SimpleWebBrowserDemo.Services;

public class NavigationService
{
    private readonly BrowserState _state;

    public NavigationService(BrowserState state)
    {
        _state = state;
    }

    public async Task<string> NavigateToAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var normalizedUrl = NormalizeUrl(url);

        if (_state.CurrentUrl != normalizedUrl && !string.IsNullOrEmpty(_state.CurrentUrl))
        {
            _state.BackHistory.Push(_state.CurrentUrl);
        }

        _state.CurrentUrl = normalizedUrl;
        _state.ForwardHistory.Clear();

        return normalizedUrl;
    }

    public async Task<string?> GoBackAsync()
    {
        if (!_state.CanGoBack)
        {
            return null;
        }

        var currentUrl = _state.CurrentUrl;
        _state.ForwardHistory.Push(currentUrl);

        var previousUrl = _state.BackHistory.Pop();
        _state.CurrentUrl = previousUrl;

        return previousUrl;
    }

    public async Task<string?> GoForwardAsync()
    {
        if (!_state.CanGoForward)
        {
            return null;
        }

        var currentUrl = _state.CurrentUrl;
        _state.BackHistory.Push(currentUrl);

        var nextUrl = _state.ForwardHistory.Pop();
        _state.CurrentUrl = nextUrl;

        return nextUrl;
    }

    public async Task<string> RefreshAsync()
    {
        return _state.CurrentUrl;
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

        return $"https://www.bing.com/search?q={Uri.EscapeDataString(url)}";
    }

    public void SetLoadingState(bool isLoading)
    {
        _state.IsLoading = isLoading;
    }

    public void SetPageTitle(string title)
    {
        _state.PageTitle = title;
    }
}
