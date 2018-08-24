using System;
using System.Collections.Generic;
using System.Text;

namespace eGallery.UnitOfWork.ViewModels
{
    public class StatusViewModel
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int StatusTypeId { get; set; }
        public string StatusConstant { get; set; }
    }
}
