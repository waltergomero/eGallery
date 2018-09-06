using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.Contracts.Models;

namespace eGallery.Contracts.Repositories
{
    public interface IStatusRepository
    {
        Task<StatusModel[]> StatusList();
        Task<StatusModel[]> StatusListByType(int TypeId);
        Task<StatusModel> StatusById(int StatusId);
        Task SaveStatusData(string StatusName, int StatusId, int StatusType);
    }
}
