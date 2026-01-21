namespace SimpleWebBrowserDemo.Models;

public class DownloadItem
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string FileName { get; set; } = string.Empty;
    
    public string FilePath { get; set; } = string.Empty;
    
    public long? FileSize { get; set; }
    
    public string? MimeType { get; set; }
    
    public long CreatedAt { get; set; }
    
    public bool Completed { get; set; }
}
