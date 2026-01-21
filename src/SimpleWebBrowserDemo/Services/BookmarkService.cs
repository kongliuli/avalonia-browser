using System;
using System.Threading.Tasks;
using SimpleWebBrowserDemo.Models;
using SimpleWebBrowserDemo.Repositories;

namespace SimpleWebBrowserDemo.Services;

public class BookmarkService
{
    private readonly IBookmarkRepository _bookmarkRepository;
    private readonly IBookmarkFolderRepository _folderRepository;
    
    public BookmarkService(
        IBookmarkRepository bookmarkRepository,
        IBookmarkFolderRepository folderRepository)
    {
        _bookmarkRepository = bookmarkRepository;
        _folderRepository = folderRepository;
    }
    
    public async Task<System.Collections.Generic.IEnumerable<Bookmark>> GetAllAsync()
    {
        return await _bookmarkRepository.GetAllAsync();
    }
    
    public async Task<Bookmark?> GetByIdAsync(long id)
    {
        return await _bookmarkRepository.GetByIdAsync(id);
    }
    
    public async Task<System.Collections.Generic.IEnumerable<Bookmark>> GetByFolderAsync(long folderId)
    {
        return await _bookmarkRepository.GetByFolderAsync(folderId);
    }
    
    public async Task AddAsync(Bookmark bookmark)
    {
        await _bookmarkRepository.AddAsync(bookmark);
        return;
    }
    
    public async Task UpdateAsync(Bookmark bookmark)
    {
        await _bookmarkRepository.UpdateAsync(bookmark);
        return;
    }
    
    public async Task DeleteAsync(long id)
    {
        await _bookmarkRepository.DeleteAsync(id);
        return;
    }
    
    public async Task DeleteAllAsync()
    {
        await _bookmarkRepository.DeleteAllAsync();
        return;
    }
    
    public async Task MoveToFolderAsync(long bookmarkId, long folderId)
    {
        await _bookmarkRepository.MoveToFolderAsync(bookmarkId, folderId);
        return;
    }
}
