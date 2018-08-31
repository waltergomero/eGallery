using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using eGallery.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eGallery.Web.Razor.Pages.app.Category
{
    public class AddEditCategoryModel : PageModel
    {
        private readonly ICategoryUnitOfWork _categoryUnitOfWork;
        private readonly IStatusUnitOfWork _statusUnitOfWork;

        public AddEditCategoryModel(ICategoryUnitOfWork categoryUnitOfWork, IStatusUnitOfWork statusUnitOfWork)
        {
            _categoryUnitOfWork = categoryUnitOfWork;
            _statusUnitOfWork = statusUnitOfWork;
        }

        public string Message { get; private set; } = "";
        public string Title { get; private set; } = "";

        [BindProperty]
        public CategoryViewModel Category { get; set; }
        public List<SelectListItem> statusIdSelected { get; set; }
        public List<SelectListItem> parentCategoryIdSelected { get; set; }
        public List<StatusViewModel> statusList { get; set; }
        public List<CategoryViewModel> categoryList { get; set; }


        public async Task<IActionResult> OnGetAsync(int Id = 0)
        {
            int selParentCategoryId = 0;

            if (Id > 0)
            {
                Title = "Edit";
                Category = await _categoryUnitOfWork.CategoryById(Id);
                statusList = await _statusUnitOfWork.StatusList();

                selParentCategoryId = Category.ParentCategoryId;

                StatusDropDownList(statusList, "StatusId", "StatusName", Category.StatusId);

                if (Category == null)
                {
                    Message = "No records found.";
                }
            }
            else
            {
                Title = "Add";

            }
            categoryList = await _categoryUnitOfWork.CategoryList();
            CategoryDropDownList(categoryList, "CategoryId", "CategoryName", selParentCategoryId);

            return Page();
        }

        public void StatusDropDownList(List<StatusViewModel> statusList, string statusId, string statusName, int selStatusId)
        {
            var selectList = new SelectList(statusList, statusId, statusName, selStatusId);
            statusIdSelected = selectList.ToList();
        }

        public void CategoryDropDownList(List<CategoryViewModel> categoryList, string categoryId, string categoryName, int SelParentCategoryId)
        {
            var selectList = new SelectList(categoryList, categoryId, categoryName, SelParentCategoryId);
            parentCategoryIdSelected = selectList.ToList();
        }

        //Method to save the data back to database
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                    int CategoryId = Category.CategoryId;
                    string CategoryName = Category.CategoryName;
                    string Description = Category.Description;
                    int ParentCategoryId = Category.ParentCategoryId;
                    int StatusId = Category.StatusId;

                    await this._categoryUnitOfWork.SaveCategoryData(CategoryId, CategoryName, Description, ParentCategoryId, StatusId);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Page();
            }           
        }

    }
}