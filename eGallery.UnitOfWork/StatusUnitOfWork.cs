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
        public async Task<List<StatusViewModel>> StatusList()
        {
            var status = await _statusService.StatusList();

            if (status != null)
            {
                var statusItems = status.Select(x => new StatusViewModel
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName,
                    StatusTypeId = x.StatusTypeId
                }).ToArray();
                return statusItems.ToList();
            }
            return null;
        }

        public async Task<List<StatusViewModel>> StatusListByType(int TypeId)
        {
            var status = await _statusService.StatusListByType(TypeId);

            if (status != null)
            {
                var statusItems = status.Select(x => new StatusViewModel
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName
                }).ToArray();
                return statusItems.ToList();
            }
            return null;
        }

        public async Task<StatusViewModel[]> StatusListArray()
        {
            var status = await _statusService.StatusList();

            if (status != null)
            {
                var statusItems = status.Select(x => new StatusViewModel
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName,
                    StatusTypeId = x.StatusTypeId         
                }).ToArray();
                return statusItems.ToArray();
            }
            return null;
        }

        public async Task<StatusViewModel> StatusById(int StatusId)
        {
            var s = await _statusService.StatusById(StatusId);

            StatusViewModel sVM = new StatusViewModel();
            sVM.StatusId = s.StatusId;
            sVM.StatusName = s.StatusName;
            sVM.StatusTypeId = s.StatusTypeId;

            return sVM;


        }

        public async Task SaveStatusData(string StatusName, int StatusId, int StatusType)
        {
            await _statusService.SaveStatusData(StatusName, StatusId, StatusType);
        }

    }
}
