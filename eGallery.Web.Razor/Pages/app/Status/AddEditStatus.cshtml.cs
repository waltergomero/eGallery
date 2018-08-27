using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using eGallery.UnitOfWork;


namespace eGallery.Web.Razor.Pages.app.Status
{
    public class AddEditStatusModel : PageModel
    {
        private readonly IStatusUnitOfWork _statusUnitOfWork;

        public AddEditStatusModel(IStatusUnitOfWork statusUnitOfWork)
        {
            _statusUnitOfWork = statusUnitOfWork;
        }

        public string Message { get; private set; } = "";

        [BindProperty]
        public StatusViewModel Status { get; set; }

        public async Task<IActionResult> OnGetAsync(int Id = 0)
        {
            // StatusViewModel _model = new StatusViewModel();
            //var myViewData = new ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
            //                        new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());
            //return new PartialViewResult() { ViewName = "AddEditStatus", ViewData = this.ViewData };
            if (Id == 0)
            {
                return Page();
            }
            else
            {
                Status = await _statusUnitOfWork.StatusById(Id);
                return Page();

            }
        }

        //Method to save the data back to database
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                int StatusId = Status.StatusId;
                string StatusName = Status.StatusName;
                int StatusTypeId = Status.StatusTypeId;

                await this._statusUnitOfWork.SaveStatusData(StatusName, StatusId, StatusTypeId);

            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            //Redirecting back to Index page after successfull save
            return RedirectToPage("./Index");

        }
    }
}