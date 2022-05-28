using eShop.Shared.Attributes;
using eShop.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.Web.Controllers
{
    [PageDetail]
    public class PublicBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppDataHelper.Populate();

            base.OnActionExecuting(filterContext);
        }
    }
}