using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using eGallery.UnitOfWork.ViewModels;

namespace eGallery.UnitOfWork
{
    public interface ICommonUnitOfWork
    {
        List<SelectListItem> CategoryDropDownList(List<CategoryViewModel> categoryList, string categoryId, string categoryName);
    }
}
