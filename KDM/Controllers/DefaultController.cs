using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class DefaultController : Controller
    {
        // GET: Default
        [OverrideActionFilters]
        public ActionResult Index()
        {
            ViewBag.User = User.Identity.Name;
            return View();
        }

        [OverrideActionFilters]
        public ActionResult NotAuthorized()
        {

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }

        public ActionResult DashboardV1()
        {
            return View();
        }

        public ActionResult DashboardV2()
        {
            return View();
        }
        
    }
}