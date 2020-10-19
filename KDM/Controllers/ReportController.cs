using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class ReportController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BeforePVgeneration()
        {
            return View();
        }

        public ActionResult BeforePVgenerationAction()
        {
            var model = db.tbl_member_tree.Where(x => x.PV > 0 ).ToList();
            return View(model);
        }
        public ActionResult AfterPVGeneration()
        {
            return View();
        }
        public ActionResult AfterPVGenerationAction()
        {
            return View();
        }

        public ActionResult ProcessDetails()
        {
            return View();
        }

        public ActionResult ShowprocessDetails(string reportdate)
        {
            DateTime today = Convert.ToDateTime(reportdate);
            var model = db.tbl_member_tree_history.Where(x => x.ProcessDate == today).OrderBy(o=>o.MemberID).ToList();
            
            return View(model);
        }

        public ActionResult clearpointandHistory()
        {
            using (var KDMDB = new KDMDB())
            {
                int noOfRowUpdated = db.Database.ExecuteSqlCommand("Update tbl_member_tree set LeftPoint =0,RightPoint=0,PV=0,BV=0 ");
 
                 int noOfRowDeleted = db.Database.ExecuteSqlCommand("delete from tbl_member_tree_history ");
                 int matchdel = db.Database.ExecuteSqlCommand("delete from tbl_binary_matching_data ");
            }

            return View();
        }
    }
}