﻿using System;
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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace eGallery.Web.Razor.Pages.app.Gallery
{
    [Authorize]
    public class UploadModel : PageModel
    {
        private readonly IHostingEnvironment _env;
        public string Message { get; private set; } = "";

        private readonly ICategoryUnitOfWork _categoryUnitOfWork;
        private readonly ICommonUnitOfWork _commonUnitOfWork;
        private readonly IUploadUnitOfWork _uploadUnitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public UploadModel(ICategoryUnitOfWork categoryUnitOfWork, ICommonUnitOfWork commonUnitOfWork, IUploadUnitOfWork uploadUnitOfWork, IHostingEnvironment env, UserManager<IdentityUser> userManager)
        {
            _categoryUnitOfWork = categoryUnitOfWork;
            _commonUnitOfWork = commonUnitOfWork;
            _uploadUnitOfWork = uploadUnitOfWork;
            _env = env;
            _userManager = userManager;
        }

        public List<SelectListItem> SortedCategoryList { get; set; }
        public List<CategoryViewModel> categoryList { get; set; }
        public int CategoryId { get; set; }
        public string FolderName { get; set; }


        //[BindProperty]
        //public FileUploadViewModel fileUpload { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            categoryList = await _categoryUnitOfWork.CategoryList();
            SortedCategoryList = _commonUnitOfWork.CategoryDropDownList(categoryList, "CategoryId", "CategoryName");

            var user = await _userManager.GetUserAsync(User);
            string UserEmail = user.Email;
            FolderName = _uploadUnitOfWork.GetUserFolderName(UserEmail).ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(ICollection<IFormFile> files, IFormCollection form)
        {
            if (files == null || files.Count == 0)
            {
                Message = "files not selected";
                return Page();
            }

            string CategoryId = form["CategoryId"];
            string UserEmail = User.Identity.Name;
            string FolderName = form["FolderName"];

            if (string.IsNullOrEmpty(FolderName))
            {
               FolderName = GetUniqueName("Gal_");
            }
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

            foreach (var ImageFile in files)
            {
                if (ImageFile != null) // ImageFile = The IFormFile that was uploaded
                {
                    //var imageName = ImageFile.FileName;                   
                    string ext = System.IO.Path.GetExtension(ImageFile.FileName).ToLower();

                    string iName = ImageFile.FileName.Replace(ImageFile.FileName, FolderName);
                    iName = iName + ext;

                    var newImageName = GetUniqueName(iName);
                    string orignalPath = Path.Combine(uploadOriginalPath, newImageName);
                    string resizedPath = Path.Combine(uploadResisedPath, newImageName);                   
                    string thumbPath = Path.Combine(uploadThumbPath, newImageName);
                
                    try
                    {
                        using (FileStream fs = new FileStream(orignalPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fs);
                        }
                         //var image = Image.FromStream(ImageFile.OpenReadStream());
                        Image<Rgba32> image = Image.Load(ImageFile.OpenReadStream());

                        // Get the image's original width and height
                        int originalWidth = image.Width;
                        int originalHeight = image.Height;
                        int maxSize = 960;
                        int tSize = 200;

                        // To preserve the aspect ratio
                        float ratioX = (float)maxSize / (float)originalWidth;
                        float ratioY = (float)maxSize / (float)originalHeight;
                        float ratio = Math.Min(ratioX, ratioY);

                        float ratioTX = (float)tSize / (float)originalWidth;
                        float ratioTY = (float)tSize / (float)originalHeight;
                        float ratioT = Math.Min(ratioTX, ratioTY);

                        // New width and height based on aspect ratio
                        int newWidth = (int)(originalWidth * ratio);
                        int newHeight = (int)(originalHeight * ratio);

                        int newTWidth = (int)(originalWidth * ratioT);
                        int newTHeight = (int)(originalHeight * ratioT);
                        string Format = "";

                        if (image.Width > image.Height)
                        {
                            Format = "l";
                        }
                        else
                        {
                            Format = "p";
                        }

                        ResizeAndSaveImage(ImageFile.OpenReadStream(), resizedPath, newWidth, newHeight);
                        ResizeAndSaveImage(ImageFile.OpenReadStream(), thumbPath, newTWidth, newTHeight);

                        //save information to the database
                        int ImageId = 0;
                        await this._uploadUnitOfWork.SaveUploadedImages(ImageId, Convert.ToInt32(CategoryId), newImageName, UserEmail, Format, FolderName);
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
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

        ////private void SaveImage(Image image, string path, Image resizedImage, int newWidth, int newHeight)
        ////{
        ////    using (Graphics graphics = Graphics.FromImage(resizedImage))
        ////    {
        ////        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        ////    }

        ////    resizedImage.Save(path);
        ////    resizedImage.Dispose();
        ////}

        ////private Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        ////{
        ////    Image destinationImage = new Bitmap(destinationWidth, destinationHeight);

        ////    using (Graphics g = Graphics.FromImage(destinationImage))
        ////        g.DrawImage(
        ////          sourceImage,
        ////          new Rectangle(0, 0, destinationWidth, destinationHeight),
        ////          new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        ////          GraphicsUnit.Pixel
        ////        );

        ////    return destinationImage;
        ////}

        //public static Bitmap ResizeImage(Image image, int width, int height)
        //{
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}
    }
}