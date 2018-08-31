using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.UnitOfWork.ViewModels;

namespace eGallery.UnitOfWork
{
   public interface ICategoryUnitOfWork
    {
        Task<List<CategoryViewModel>> CategoryList();
        Task<CategoryViewModel[]> CategoryListArray();
        Task<CategoryViewModel> CategoryById(int StatusId);
        Task SaveCategoryData(int CategoryId, string CategoryName, string Description, int ParentCategoryId, int StatusId);
    }
}
