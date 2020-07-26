using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadLater.Entities;
using ReadLater.Repository;

namespace ReadLater.Services
{
    public class CategoryService : ICategoryService
    {
        protected IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Category CreateCategory(Category category)
        {
            _unitOfWork.Repository<Category>().Insert(category);
            _unitOfWork.Save();
            return category;
        }

        public void UpdateCategory(Category category)
        {
            _unitOfWork.Repository<Category>().Update(category);
            _unitOfWork.Save();
        }

        public List<Category> GetCategories()
        {
            return _unitOfWork.Repository<Category>().Query().Get().ToList();
        }

        public List<Category> GetCategoriesByUserId(string Id)
        {
            List<CategoriesPerUser> categoriesPerUsers = _unitOfWork.Repository<CategoriesPerUser>().Query()
                                                    .Filter(c => c.UserID == Id)
                                                    .Get()
                                                    .ToList();
            List<CategoriesPerUser> distinctCPU = categoriesPerUsers.GroupBy(c => c.CategoryID).Select(c => c.FirstOrDefault()).ToList();

            List<Category> categories = GetCategories();
            List<Category> resultList = new List<Category>();

            foreach(Category category in categories)
            { 
                foreach(CategoriesPerUser cpu in distinctCPU)
                {
                    if(category.ID == cpu.ID)
                    {
                        resultList.Add(category);
                    }
                }
            }
            return resultList;
        }

        public Category GetCategory(int Id)
        {
            return _unitOfWork.Repository<Category>().Query()
                                                    .Filter(c => c.ID == Id)
                                                    .Get()
                                                    .FirstOrDefault();
        }

        public Category GetCategory(string Name)
        {
            return _unitOfWork.Repository<Category>().Query()
                                        .Filter(c => c.Name == Name)
                                        .Get()
                                        .FirstOrDefault();
        }

        public void DeleteCategory(Category category)
        {
            _unitOfWork.Repository<Category>().Delete(category);
            _unitOfWork.Save();
        }
    }
}
