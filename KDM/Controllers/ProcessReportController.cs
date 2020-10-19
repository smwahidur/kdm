using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class ProcessReportController : Controller
    {
        // GET: ProcessReport
        KDMDB db = new KDMDB();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerBinaryReport()
        {
            return View();
        }
     
        public ActionResult CustomerBinaryReportAction(string MemberID)
        {
            var model = db.tbl_binary_matching_data.Where(x=>x.PlacementID==MemberID).ToList();
            return View(model);
          
        }
        
        
    }
}