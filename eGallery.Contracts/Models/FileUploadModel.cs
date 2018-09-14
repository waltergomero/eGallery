using Microsoft.AspNetCore.Http;


namespace eGallery.Contracts.Models
{
    public class FileUploadModel
    {
        public int ImageId { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }
        public string UserEmail { get; set; }
        public string Format { get; set; }
        public string FolderName { get; set; }
    }
}
