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
        Task<StatusViewModel[]> StatusListArray();
    }
}
