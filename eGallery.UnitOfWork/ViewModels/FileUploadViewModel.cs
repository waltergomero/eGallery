using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace eGallery.UnitOfWork.ViewModels
{
    class FileUploadViewModel
    {
     public IFormFile fileUpload { get; set; }
     public int ImageId { get; set; }
     public int CategoryId { get; set; }
     public string ImageName { get; set; }
     public string UserEmail { get; set; }
     public string Format { get; set; }
     public string FolderName { get; set; }
    }
}
