using ReadLater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLater.Services
{
    public interface ICateogoriesPerUser
    {
        CategoriesPerUser CreateCategoriesPerUser(CategoriesPerUser categoriesPerUser);
        List<CategoriesPerUser> GetCategoriesPerUser(string userId);
        void UpdateCategoriesPerUser(CategoriesPerUser categoriesPerUser);
        void DeleteCategoriesPerUser(CategoriesPerUser categoriesPerUser);
    }
}
