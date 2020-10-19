using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class HelpcodeController : Controller
    {
        // GET: Helpcode
        public ActionResult Index()
        {
            return View();
        }
    }
}