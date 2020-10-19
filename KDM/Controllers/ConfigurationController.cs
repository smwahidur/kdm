using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class ConfigurationController : Controller
    {
        // GET: Configuration
        KDMDB db = new KDMDB();
        public ActionResult allparemeter()
        {
            var model = db.tbl_kdm_settings_master.ToList();
            return View(model);
        }

        public ActionResult BinaryProcessCode()
        {
            return View();
        }
    }
}