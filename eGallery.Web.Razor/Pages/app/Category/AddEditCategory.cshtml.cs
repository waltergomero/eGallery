using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using eGallery.UnitOfWork;

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

        [BindProperty]
        public CategoryViewModel Category { get; set; }
        public IList<StatusViewModel> statusList { get; set; }
        public async Task<IActionResult> OnGetAsync(int Id = 0)
        {
            if (Id == 0)
            {
                return Page();
            }
            else
            {
                statusList = await _statusUnitOfWork.StatusList();

                Category = await _categoryUnitOfWork.CategoryById(Id);
                if (Category == null)
                {
                    Message = "No records found.";
                }
                return Page();

            }
        }
    }
}