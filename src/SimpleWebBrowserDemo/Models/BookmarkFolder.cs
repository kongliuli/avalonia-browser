namespace SimpleWebBrowserDemo.Models;

public class BookmarkFolder
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public long? ParentId { get; set; }
    
    public long CreatedAt { get; set; }
    
    public int OrderIndex { get; set; } = 0;
}
