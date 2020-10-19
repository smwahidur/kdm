using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KDM.Controllers
{
    public class ErrorPageController : Controller
    {
        public ActionResult Oops(int id)
        {
            Response.StatusCode = id;
            return View();
        }
    }
}