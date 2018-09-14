using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eGallery.UnitOfWork
{
    public class UploadUnitOfWork : IUploadUnitOfWork
    {
        private readonly IUploadService _uploadService;
        public UploadUnitOfWork(IUploadService uploadService)
        {
            _uploadService = uploadService;

        }
        public async Task SaveUploadImages(int CategoryId, string ImageName, string UserEmail)
        {
            await _uploadService.SaveCategoryData(CategoryId, ImageName, UserEmail);
        }
    }
}
