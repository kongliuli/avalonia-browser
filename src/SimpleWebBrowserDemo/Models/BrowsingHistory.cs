namespace SimpleWebBrowserDemo.Models;

public class BrowsingHistory
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string? Title { get; set; }
    
    public long VisitTime { get; set; }
    
    public int VisitCount { get; set; } = 1;
    
    public string? FaviconUrl { get; set; }
    
    public string? Description { get; set; }
}
