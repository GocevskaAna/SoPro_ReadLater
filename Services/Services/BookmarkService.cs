using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadLater.Entities;
using ReadLater.Repository;

namespace ReadLater.Services
{
    public class BookmarkService : IBookmarkService
    {
        protected IUnitOfWork _unitOfWork;
        protected CategoryService _categoryService;
        protected CategoriesPerUserService _categoriesPerUserService;

        public BookmarkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Bookmark CreateBookmark(Bookmark bookmark)
        {

            bookmark.CreateDate = DateTime.Now;
            _unitOfWork.Repository<Bookmark>().Insert(bookmark);
            _unitOfWork.Save();
            return bookmark;
        }

        public Bookmark GetBookmarkById(int Id)
        {
            return _unitOfWork.Repository<Bookmark>().Query()
                                                   .Filter(b=> b.ID == Id)
                                                   .Get()
                                                   .FirstOrDefault();
        }

        public Dictionary<string, List<Bookmark>> GetBookmarks(string userId)
        {
            Dictionary<string, List<Bookmark>> resultDictionary = new Dictionary<string, List<Bookmark>>();
            List<CategoriesPerUser> categoriesPerUser = new List<CategoriesPerUser>();
            var categoriesPerUserDistinct = new List<CategoriesPerUser>();

            if (string.IsNullOrEmpty(userId))
            {
                categoriesPerUser = _unitOfWork.Repository<CategoriesPerUser>().Query()
                                                        .OrderBy(l => l.OrderByDescending(b => b.UserID))
                                                        .Get()
                                                        .ToList();
                categoriesPerUserDistinct = categoriesPerUser.GroupBy(c => c.CategoryID).Select(c => c.FirstOrDefault()).ToList();
            }
            else
            {
                categoriesPerUser = _unitOfWork.Repository<CategoriesPerUser>().Query()
                                                            .Filter(b => b.UserID != null && b.UserID == userId)
                                                            .Get()
                                                            .Distinct()
                                                            .ToList();

                categoriesPerUserDistinct = categoriesPerUser.GroupBy(c =>new { c.CategoryID, c.UserID }).Select(c => c.FirstOrDefault()).ToList();

                foreach(CategoriesPerUser item in categoriesPerUserDistinct)
                {
                    List<Bookmark> bookmarks = GetBookmarksByCategory(item.CategoryID);
                    Category category = _unitOfWork.Repository<Category>().Query().Filter(c => c.ID == item.CategoryID).Get().FirstOrDefault();
                    if(bookmarks.Count!=0)
                    {
                        resultDictionary.Add(category.Name, bookmarks);
                    }
                }
            }
           return resultDictionary;
        }

        public List<Bookmark> GetBookmarksByCategory(int? categoryId)
        {
            if (categoryId==0 || categoryId==null)
            {
                return _unitOfWork.Repository<Bookmark>().Query()
                                                        .OrderBy(l => l.OrderByDescending(b => b.Category))
                                                        .Get()
                                                        .ToList();
            }
            else
            {
                return  _unitOfWork.Repository<Bookmark>().Query()
                                                            .Filter(b => b.Category != null && b.Category.ID == categoryId)
                                                            .Get()
                                                            .ToList();
            }
        }

        //update bookmark
        public void UpdateBookmark(Bookmark bookmark)
        {
            _unitOfWork.Repository<Bookmark>().Update(bookmark);
            _unitOfWork.Save();
        }

        //detele bookmark
        public void DeleteBookmark(Bookmark bookmark)
        {
            _unitOfWork.Repository<Bookmark>().Delete(bookmark);
            _unitOfWork.Save();
        }
    }
}
