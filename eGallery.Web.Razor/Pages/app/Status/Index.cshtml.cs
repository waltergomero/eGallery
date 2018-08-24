using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork;
using eGallery.UnitOfWork.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace eGallery.Web.Razor.Pages.app.Status
{
    public class IndexModel : PageModel
    {
        private readonly IStatusUnitOfWork _statusUnitOfWork;

        public IndexModel(IStatusUnitOfWork statusUnitOfWork)
        {
            _statusUnitOfWork = statusUnitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            var query = await _statusUnitOfWork.StatusList();
            if (query != null)
            {
                return this.Ok(query); ;
            }

            return this.NotFound();
        }
    }
}