using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eGallery.Contracts.Models
{
    public class StatusModel
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int StatusTypeId { get; set; }
    }
}
