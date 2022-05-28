using eShop.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eShop.Web.ViewModels
{
    public class CategoriesMenuViewModel
    {
        public List<CategoryWithChildren> CategoryWithChildrens { get; set; }
    }
}