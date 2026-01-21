namespace SimpleWebBrowserDemo.Models;

public class Bookmark
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public long? FolderId { get; set; }
    
    public long CreatedAt { get; set; }
    
    public string? FaviconUrl { get; set; }
    
    public string? Description { get; set; }
    
    public int OrderIndex { get; set; } = 0;
}
