using ReadLater.Entities;
using ReadLater.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace ReadLater.Services
{
    public class CategoriesPerUserService : ICateogoriesPerUser
    {
        protected IUnitOfWork _unitOfWork;

        public CategoriesPerUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CategoriesPerUser CreateCategoriesPerUser(CategoriesPerUser categoriesPerUser)
        {
            _unitOfWork.Repository<CategoriesPerUser>().Insert(categoriesPerUser);
            _unitOfWork.Save();
            return categoriesPerUser;
        } 

        public void DeleteCategoriesPerUser(CategoriesPerUser categoriesPerUser)
        {
            _unitOfWork.Repository<CategoriesPerUser>().Delete(categoriesPerUser);
            _unitOfWork.Save();
        }

        public List<CategoriesPerUser> GetCategoriesPerUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.Repository<CategoriesPerUser>().Query()
                                                        .Get()
                                                        .ToList();
            }
            else
            {
                return _unitOfWork.Repository<CategoriesPerUser>().Query()
                                                            .Filter(b => b.UserID != null && b.UserID == userId)
                                                            .Get()
                                                            .ToList();
            }
        }

        public void UpdateCategoriesPerUser(CategoriesPerUser categoriesPerUser)
        {
            _unitOfWork.Repository<CategoriesPerUser>().Update(categoriesPerUser);
            _unitOfWork.Save();
        }
    }
}
