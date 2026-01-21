using System.Collections.Generic;

namespace SimpleWebBrowserDemo.Models;

public class BrowserState
{
    public string CurrentUrl { get; set; } = string.Empty;

    public string PageTitle { get; set; } = string.Empty;

    public bool IsLoading { get; set; }

    public Stack<string> BackHistory { get; set; } = new();

    public Stack<string> ForwardHistory { get; set; } = new();

    public bool CanGoBack => BackHistory.Count > 0;

    public bool CanGoForward => ForwardHistory.Count > 0;
}
