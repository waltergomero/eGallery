using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eGallery.UnitOfWork
{
    public interface IUploadUnitOfWork
    {
        Task SaveUploadImages(int CategoryId, string ImageName, string UserEmail);
    }
}
