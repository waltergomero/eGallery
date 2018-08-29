using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork;
using eGallery.UnitOfWork.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace eGallery.Web.Razor.Pages.app.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryUnitOfWork _categoryUnitOfWork;

        public IndexModel(ICategoryUnitOfWork categoryUnitOfWork)
        {
            _categoryUnitOfWork = categoryUnitOfWork;
        }

        public string Message { get; private set; } = "";


        public List<CategoryViewModel> category { get; set; } = new List<CategoryViewModel>();

        public async Task<IActionResult> OnGet()
        {
            try
            {
                category = await _categoryUnitOfWork.CategoryList();
            }
            catch(Exception ex)
            {
                Message = ex.Message;
            }

            return Page();
        }
    }
}