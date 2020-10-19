using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace KDM.Filters
{
    public class KDMSessionTimeoutFilter: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return false;
            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Account" },
                     { "action", "Login" }
                });
            }
        }
    }
}