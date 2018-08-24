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
    }
}
