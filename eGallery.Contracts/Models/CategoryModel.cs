using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace eGallery.Contracts.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string CategoryImage { get; set; }
        public int ParentCategoryId { get; set; }
        public int StatusId { get; set; }
    }
}
