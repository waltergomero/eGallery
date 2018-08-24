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
    public class StatusUnitOfWork: IStatusUnitOfWork
    {

        private readonly IStatusService _statusService;

        public StatusUnitOfWork(IStatusService statusService)
        {
            _statusService = statusService;

        }
        public async Task<StatusViewModel[]> StatusList()
        {
            var status = await _statusService.StatusList();

            if (status != null)
            {
                var statusItems = status.Select(x => new StatusViewModel
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName,
                    StatusTypeId = x.StatusTypeId,
                    StatusConstant = x.StatusConstant                 
                }).ToArray();
                return statusItems.ToArray();
            }
            return null;
        }

    }
}
