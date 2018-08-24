using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.Contracts.Models;

namespace eGallery.Contracts.Services
{
   public interface IStatusService
    {
        Task<StatusModel[]> StatusList();
    }
}
