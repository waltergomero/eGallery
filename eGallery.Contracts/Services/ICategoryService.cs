using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.Contracts.Models;

namespace eGallery.Contracts.Services
{
    public interface ICategoryService
    {
        Task<CategoryModel[]> CategoryList();
        Task<CategoryModel> CategoryById(int CategoryId);
        Task SaveCategoryData(int CategoryId, string CategoryName, string Description, string CategoryImage, int ParentCategoryId, int StatusId);

    }
}
