using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork;
using eGallery.UnitOfWork.ViewModels;


namespace eGallery.Web.Razor.Pages.app.Status
{
    public class IndexModel : PageModel
    {
        private readonly IStatusUnitOfWork _statusUnitOfWork;

        public IndexModel(IStatusUnitOfWork statusUnitOfWork)
        {
            _statusUnitOfWork = statusUnitOfWork;
        }

        public string Message { get; private set; } = "";

        
        public List<StatusViewModel> status { get; set; } = new List<StatusViewModel>();
       
        public async Task<IActionResult> OnGet()
        {
            // Message += $" Server time is { DateTime.Now }";
            try
            {
                status = await _statusUnitOfWork.StatusList();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Page();
        }


        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var query = await _statusUnitOfWork.StatusList();
        //    return new JsonResult(query);

        //}

    }
}