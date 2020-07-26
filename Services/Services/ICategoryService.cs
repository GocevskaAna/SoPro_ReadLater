using System;
using System.Collections.Generic;
using ReadLater.Entities;
namespace ReadLater.Services
{
    public interface ICategoryService
    {
        Category CreateCategory(Category category);
        List<Category> GetCategories();
        List<Category> GetCategoriesByUserId(string Id);
        Category GetCategory(int Id);
        Category GetCategory(string Name);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
