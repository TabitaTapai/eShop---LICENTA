using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eShop.Shared.Attributes
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //daca nu esti autentificat se face redirectionare catre login
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                //daca esti autentificat fara rol de acces se face redirectare catre actiunea personalizata a controlerului
                filterContext.Result = new RedirectToRouteResult("UnAuthorized", new RouteValueDictionary());
            }
        }
    }
}