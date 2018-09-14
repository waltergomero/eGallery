using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eGallery.Contracts.Services;

namespace eGallery.UnitOfWork
{
    public class UploadUnitOfWork : IUploadUnitOfWork
    {
        private readonly IUploadService _uploadService;
        public UploadUnitOfWork(IUploadService uploadService)
        {
            _uploadService = uploadService;

        }
        public async Task SaveUploadedImages(int ImageId, int CategoryId, string ImageName, string UserEmail, string Format, string FolderName)
        {
            await _uploadService.SaveUploadedImages(ImageId, CategoryId, ImageName, UserEmail, Format, FolderName);
        }

        public string GetUserFolderName(string UserEmail)
        {
            return _uploadService.GetUserFolderName(UserEmail);
        }

        
    }
}
