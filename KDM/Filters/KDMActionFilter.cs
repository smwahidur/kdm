using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using KDM.Helpers;
using KDM.Models;

namespace KDM.Filters
{
    public class KDMActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            var area = "";
            if(routeData.Values["area"]!=null)
            area = routeData.Values["area"].ToString();
            var controller = routeData.Values["controller"].ToString();
            var action = routeData.Values["action"].ToString();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            ControllerHelper cHelper = new ControllerHelper();
            UserRoute userRoute = new UserRoute();
            userRoute.Area = area;
            userRoute.Controller = controller;
            userRoute.Action = action;

            if((!controller.ToLower().Contains("account") 
               && (!action.ToLower().Contains("logoff") 
               || !action.ToLower().Contains("login")))
               && (!controller.ToLower().Contains("home") &&
               (!action.ToLower().Contains("index"))))
            {
                if (!cHelper.AuthorizeAction(userId, userRoute))
                {
                    filterContext.Result = new HttpUnauthorizedResult();

                    filterContext.Controller.TempData["ErrorMessage"] = "You are not authorized to access this page";
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Default", action = "NotAuthorized" }));

                }
            }
            
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            
        }
        
    }
}