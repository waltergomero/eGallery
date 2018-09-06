using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.UnitOfWork.ViewModels;

namespace eGallery.UnitOfWork
{
    public interface IStatusUnitOfWork
    {
        Task<List<StatusViewModel>> StatusList();
        Task<List<StatusViewModel>> StatusListByType(int TypeId);
        Task<StatusViewModel[]> StatusListArray();
        Task<StatusViewModel> StatusById(int StatusId);
        Task SaveStatusData(string StatusName, int StatusId, int StatusType);
    }
}
