using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using eGallery.Contracts.Services;
using eGallery.Contracts.Models;
using eGallery.UnitOfWork.ViewModels;
using System.Threading.Tasks;

namespace eGallery.UnitOfWork
{
    public class CategoryUnitOfWork: ICategoryUnitOfWork
    {
        private readonly ICategoryService _categoryService;

        public CategoryUnitOfWork(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }

        public async Task<List<CategoryViewModel>> CategoryList()
        {
            var category = await _categoryService.CategoryList();

            if (category != null)
            {
                var categoryItems = category.Select(x => new CategoryViewModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    CategoryImage = x.CategoryImage,
                    ParentCategoryId = x.ParentCategoryId,
                    StatusId = x.StatusId
                }).ToArray();
                return categoryItems.ToList();
            }
            return null;
        }


        public async Task<CategoryViewModel[]> CategoryListArray()
        {
            var category = await _categoryService.CategoryList();

            if (category != null)
            {
                var categoryItems = category.Select(x => new CategoryViewModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    CategoryImage = x.CategoryImage,
                    ParentCategoryId = x.ParentCategoryId,
                    StatusId = x.StatusId
                }).ToArray();
                return categoryItems.ToArray();
            }
            return null;
        }

        public async Task<CategoryViewModel> CategoryById(int CategoryId)
        {
            var c = await _categoryService.CategoryById(CategoryId);

            CategoryViewModel cVM = new CategoryViewModel();
            cVM.CategoryId = c.CategoryId;
            cVM.CategoryName = c.CategoryName;
            cVM.Description = c.Description;
            cVM.ParentCategoryId = c.ParentCategoryId;
            cVM.CategoryImage = c.CategoryImage;
            cVM.StatusId = c.StatusId;

            return cVM;
        }

        public async Task SaveCategoryData(int CategoryId, string CategoryName, string Description, int ParentCategoryId, int StatusId)
        {
            await _categoryService.SaveCategoryData(CategoryId, CategoryName, Description, ParentCategoryId, StatusId);
        }

    }
}
