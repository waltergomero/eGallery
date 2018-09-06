using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eGallery.UnitOfWork.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using eGallery.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
//using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace eGallery.Web.Razor.Pages.app.Gallery
{
    public class UploadModel : PageModel
    {
        private readonly IHostingEnvironment _env;
        public string Message { get; private set; } = "";

        private readonly ICategoryUnitOfWork _categoryUnitOfWork;
        private readonly ICommonUnitOfWork _commonUnitOfWork;

        public UploadModel(ICategoryUnitOfWork categoryUnitOfWork, ICommonUnitOfWork commonUnitOfWork, IHostingEnvironment env)
        {
            _categoryUnitOfWork = categoryUnitOfWork;
            _commonUnitOfWork = commonUnitOfWork;
            _env = env;
        }

        public List<SelectListItem> Categories { get; set; }
        public List<CategoryViewModel> categoryList { get; set; }

//[BindProperty]
       // public IFormFile Image { set; get; }

        public async Task<IActionResult> OnGetAsync()
        {
            categoryList = await _categoryUnitOfWork.CategoryList();
            Categories = _commonUnitOfWork.CategoryDropDownList(categoryList, "CategoryId", "CategoryName");
            return Page();
        }

        //Your existing code for properties goes here
        public async Task<IActionResult> OnPostAsync(ICollection<IFormFile> files)
        {
            string struserId = "208";
            string imageGallery = "Gallery";
            string originalGallery = "Original";
            string thumbGallery = "Thumb";

            var webRoot = _env.WebRootPath;
            var path = System.IO.Path.Combine(webRoot, imageGallery);

            string uploadResisedPath = path; //this is where resized images will be stored.
            string uploadOriginalPath = path + "\\" + originalGallery;
            string uploadThumbPath = path + "\\" + thumbGallery;

            if (!Directory.Exists(uploadResisedPath))
            {
                Directory.CreateDirectory(uploadResisedPath);
            }
            if (!Directory.Exists(uploadOriginalPath))
            {
                Directory.CreateDirectory(uploadOriginalPath);
            }
            if (!Directory.Exists(uploadThumbPath))
            {
                Directory.CreateDirectory(uploadThumbPath);
            }

            if (files == null || files.Count == 0)
            {
                Message = "files not selected";
                return Page();
            }

            foreach (var ImageFile in files)
            {
                if (ImageFile != null) // ImageFile = The IFormFile that was uploaded
                {
                    string format = "";

                    //var imageName = ImageFile.FileName;                   
                    string ext = System.IO.Path.GetExtension(ImageFile.FileName).ToLower();

                    string iName = ImageFile.FileName.Replace(ImageFile.FileName, struserId);
                    iName = iName + ext;

                    var newName = GetUniqueName(iName);
                    string orignalPath = Path.Combine(uploadOriginalPath, newName);
                    string resizedPath = Path.Combine(uploadResisedPath, newName);                   
                    string thumbPath = Path.Combine(uploadThumbPath, newName);
                
                    try
                    {
                        using (FileStream fs = new FileStream(orignalPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fs);
                        }
                        // var image = Image.FromStream(ImageFile.OpenReadStream());
                        Image<Rgba32> image = Image.Load(ImageFile.OpenReadStream());
                        int maxSize = 750;
                        int thumbSize = 200;
                        int newWidth = 0;
                        int newHeight = 0;
                        int newThumbWidth = 0;
                        int newThumbHeight = 0;
                        double resizeFactor = 0;
                        double resizeThumbFactor = 0;

                        if (image.Width > image.Height)
                        {
                            resizeFactor = image.Width / maxSize;
                            resizeThumbFactor = image.Width / thumbSize;
                            format = "l";
                        }
                        else
                        {
                            resizeFactor = image.Height / maxSize;
                            resizeThumbFactor = image.Height / thumbSize;
                            format = "p";
                        }

                        newWidth = (int)Math.Round(image.Width / resizeFactor);
                        newHeight = (int)Math.Round(image.Height / resizeFactor);

                        ResizeAndSaveImage(ImageFile.OpenReadStream(), resizedPath, newWidth, newHeight);

                        newThumbWidth = (int)Math.Round(image.Width / resizeThumbFactor);
                        newThumbHeight = (int)Math.Round(image.Height / resizeThumbFactor);

                        ResizeAndSaveImage(ImageFile.OpenReadStream(), resizedPath, newThumbWidth, newThumbHeight);
                        //Generate postal size images 
                        // var resizedImage = CropImage(image, 0, 0, image.Width, image.Height, newWidth, newHeight);
                        // SaveImage(image, resizedPath, resizedImage, newWidth, newHeight);

                        //Generate thumb size images 
                        // var resizedThumbImage = CropImage(image, 0, 0, image.Width, image.Height, newThumbWidth, newThumbHeight);
                        // SaveImage(image, thumbPath, resizedThumbImage, newThumbWidth, newThumbHeight);

                        //save information to the database
                        //await this._productService.ProductImageAdd(ProductId, imageName, format);
                    }
                    catch (Exception ex)
                    {
                       // ViewBag.Error = ex.Message;
                    }
                }
            }

           return RedirectToPage("Index");
        }

        private string GetUniqueName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString().Substring(0, 6) + Path.GetExtension(fileName);
        }

        public static void ResizeAndSaveImage(Stream stream, string filename, int newWidth, int newHeight)
        {
          
            using (Image<Rgba32> image = Image.Load(stream))
            {
                image.Mutate(x => x.Resize(newWidth, newHeight));
                image.Save(filename); // Automatic encoder selected based on extension.
            }
        }

        //private void SaveImage(Image image, string path, Image resizedImage, int newWidth, int newHeight)
        //{
        //    using (Graphics graphics = Graphics.FromImage(resizedImage))
        //    {
        //        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        //    }

        //    resizedImage.Save(path);
        //    resizedImage.Dispose();
        //}

        //private Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        //{
        //    Image destinationImage = new Bitmap(destinationWidth, destinationHeight);

        //    using (Graphics g = Graphics.FromImage(destinationImage))
        //        g.DrawImage(
        //          sourceImage,
        //          new Rectangle(0, 0, destinationWidth, destinationHeight),
        //          new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        //          GraphicsUnit.Pixel
        //        );

        //    return destinationImage;
        //}
    }
}