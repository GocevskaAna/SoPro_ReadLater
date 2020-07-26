using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using MVC.Models;
using ReadLater.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using ReadLater.Entities;

namespace MVC.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        IBookmarkService _bookmarkService;
        ICategoryService _categoryService;
        ICateogoriesPerUser _categoriesPerUserService;

        public BookmarksController(IBookmarkService bookmarkService, ICategoryService categoryService, ICateogoriesPerUser cateogoriesPerUserService)
        {
            _bookmarkService = bookmarkService;
            _categoryService = categoryService;
            _categoriesPerUserService = cateogoriesPerUserService;
        }

        // GET: Bookmarks
        public ActionResult NewBookmark()
        {
            var userId = Session["LoggedInUserId"].ToString();

            List<Category> AllCategories = _categoryService.GetCategories();
            List<Category> CategoryPerUserList = new List<Category>();

            List<CategoriesPerUser> CPUList = _categoriesPerUserService.GetCategoriesPerUser(userId);
            List<int?> CategoryIds = CPUList.GroupBy(c => new { c.CategoryID, c.UserID }).Select(c => c.Key.CategoryID).ToList();


            foreach (Category category in AllCategories)
            {
                foreach (int Id in CategoryIds)
                {
                    if (category.ID == Id)
                    {
                        CategoryPerUserList.Add(category);
                    }
                }
            }

            List<Category> categories = _categoryService.GetCategories();
            ViewData["Categories"] = categories;

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(BookmarkFromCategory bookmarkFromCategory)
        {
            //I had a problem here with inserting multiple records in Category table.
            //With every new insert for Bookmark, CategoryPerUser and Category new row with new ID was created in the DB
            //although I sent the ID of the newly created Category

            //I think that there are some issues with the Entity Framework with saving records

            Category category = _categoryService.GetCategory(bookmarkFromCategory.CategoryId);
            if(category!=null)
            {
                List<CategoriesPerUser> CPUList = _categoriesPerUserService.GetCategoriesPerUser(Session["LoggedInUserID"].ToString());
                List<int?> CategoryIds = CPUList.GroupBy(c => new { c.CategoryID, c.UserID }).Select(c => c.Key.CategoryID).ToList();
                if (!CategoryIds.Contains(bookmarkFromCategory.CategoryId))
                {
                    CategoriesPerUser categoriesPerUserNew = new CategoriesPerUser();
                    categoriesPerUserNew.CategoryID = bookmarkFromCategory.CategoryId;
                    categoriesPerUserNew.UserID = bookmarkFromCategory.UserId;
                    _categoriesPerUserService.CreateCategoriesPerUser(categoriesPerUserNew);
                }

                Bookmark bookmark = new Bookmark();
                bookmark.URL = bookmarkFromCategory.URL;
                bookmark.ShortDescription = bookmarkFromCategory.ShortDescription;
                bookmark.CategoryId = category.ID;
                bookmark.CreateDate = DateTime.Now;
                _bookmarkService.CreateBookmark(bookmark);
            }
            else
            {
                //create new category
                Category newCategory = new Category();
                newCategory.Name = bookmarkFromCategory.CategoryName;
                newCategory = _categoryService.CreateCategory(newCategory);


                List<CategoriesPerUser> CPUList = _categoriesPerUserService.GetCategoriesPerUser(Session["LoggedInUserID"].ToString());
                List<int?> CategoryIds = CPUList.GroupBy(c => new { c.CategoryID, c.UserID }).Select(c => c.Key.CategoryID).ToList();
                if (!CategoryIds.Contains(newCategory.ID))
                {
                    CategoriesPerUser categoriesPerUser = new CategoriesPerUser();
                    categoriesPerUser.Category = newCategory;
                    categoriesPerUser.CategoryID = newCategory.ID;
                    categoriesPerUser.UserID = bookmarkFromCategory.UserId;
                    _categoriesPerUserService.CreateCategoriesPerUser(categoriesPerUser);
                }

                Bookmark bookmark = new Bookmark();
                bookmark.URL = bookmarkFromCategory.URL;
                bookmark.ShortDescription = bookmarkFromCategory.ShortDescription;
                bookmark.Category = newCategory;
                bookmark.CategoryId = newCategory.ID;
                bookmark.CreateDate = DateTime.Now;
                _bookmarkService.CreateBookmark(bookmark);
            }

            return Json(new { message = "ok" });
            //return RedirectToAction("List", "Bookmarks")
        }

        public ActionResult List()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            Session["LoggedInUserId"] = userId;

            Dictionary<string, List<Bookmark>> BookmarksPerCategory = _bookmarkService.GetBookmarks(userId);
            ViewData["BookmarksPerCategory"] = BookmarksPerCategory;
            return View();
        }

        public ActionResult EditBookmark(int bookmarkID)
        {
            List<Category> AllCategories = _categoryService.GetCategories();
            List<Category> CategoryPerUserList = new List<Category>();
            
            List<CategoriesPerUser> CPUList = _categoriesPerUserService.GetCategoriesPerUser(Session["LoggedInUserID"].ToString());
            List<int?> CategoryIds = CPUList.GroupBy(c => new { c.CategoryID, c.UserID }).Select(c => c.Key.CategoryID).ToList();

            
            foreach(Category category in AllCategories)
            {
                foreach (int Id in CategoryIds)
                {
                    if(category.ID == Id)
                    {
                        CategoryPerUserList.Add(category);
                    }
                }
            }

            ViewData["Categories"] = CategoryPerUserList;
            Bookmark bookmark = _bookmarkService.GetBookmarkById(bookmarkID);
            return View(bookmark);
        }

        [HttpPost]
        public ActionResult EditBookmark([Bind(Include = "ID,URL,ShortDescription,CreateDate,CategoryId,Category")] Bookmark bookmark)
        {
            Category category = _categoryService.GetCategory((int)bookmark.CategoryId);
            Bookmark newBookmark = bookmark;
            newBookmark.Category = category;
            newBookmark.CreateDate = DateTime.Now;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            List<CategoriesPerUser> categoriesPerUser = _categoriesPerUserService.GetCategoriesPerUser(userId);
            List<int?> distinctCPU = categoriesPerUser.GroupBy(c => new { c.CategoryID, c.UserID }).Select(c => c.Key.CategoryID).ToList();
            
            if(!distinctCPU.Contains(newBookmark.CategoryId))
            {
                CategoriesPerUser newCPU = new CategoriesPerUser();
                newCPU.UserID = userId;
                newCPU.CategoryID = newBookmark.CategoryId;

                _categoriesPerUserService.CreateCategoriesPerUser(newCPU);
            }
           
            if (ModelState.IsValid)
            {
                _bookmarkService.UpdateBookmark(newBookmark);
                return RedirectToAction("List");
            }
            return View(bookmark);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Bookmark bookmark = _bookmarkService.GetBookmarkById(id);
            _bookmarkService.DeleteBookmark(bookmark);
            return Json(new { message = "ok" });
        }
    }
}