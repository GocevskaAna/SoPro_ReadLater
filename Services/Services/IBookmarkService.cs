using System.Collections.Generic;
using ReadLater.Entities;

namespace ReadLater.Services
{
    public interface IBookmarkService
    {
        Bookmark CreateBookmark(Bookmark bookmark);
        Bookmark GetBookmarkById(int Id);
        Dictionary<string, List<Bookmark>> GetBookmarks(string userId);
        List<Bookmark> GetBookmarksByCategory(int? categoryId);
        void UpdateBookmark(Bookmark bookmark);
        void DeleteBookmark(Bookmark bookmark);

    }
}