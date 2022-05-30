using eShop.Shared.Attributes;
using eShop.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eShop.Web.Areas.Dashboard.Controllers
{
    [PageDetail]
    [CustomAuthorize(Roles = "Administrator")]
    public class DashboardBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppDataHelper.Populate();

            base.OnActionExecuting(filterContext);
        }

        [AllowAnonymous]
        public ActionResult UnAuthorized()
        {
            Response.StatusCode = 403;

            return PartialView("UnAuthorized");
        }
    }
}