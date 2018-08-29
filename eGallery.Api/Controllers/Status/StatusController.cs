using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eGallery.Contracts.Services;

namespace eGallery.Api.Controllers.Status
{
    [Produces("application/json")]
    [Route("api/Status")]
    public class StatusController: Controller
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;

        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> StatusList()
        {
            var apiResult = await this._statusService.StatusList();
            return this.Ok(apiResult);
        }
    }
}
