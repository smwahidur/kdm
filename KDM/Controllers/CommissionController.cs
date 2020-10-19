using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class CommissionController : Controller
    {
        // GET: Commission
        KDMDB db = new KDMDB();
        public ActionResult ShowBinaryCommission()
        {
            var model = db.tbl_member_tree.Where(x => x.LeftPoint > 0 || x.RightPoint > 0 || x.PV>0 || x.BV>0 ).OrderBy(o => o.PlacementID).ToList();
            return View(model);
        }

        public ActionResult ShowGenerationCommission()
        {
            var model = db.tbl_binary_matching_data.ToList();
            return View(model);
        }


        public ActionResult ShowCustomerBincome()
        {
            return View();
        }

        public ActionResult CustomerBinaryIncomeAction(string MemberID)
        {
            var model = db.tbl_binary_matching_data.Where(x => x.PlacementID == MemberID).OrderByDescending(o=>o.BPCode).ToList();
            return View(model);
        }

        public ActionResult binarytoaccount()
        {
            return View();
        }
        public ActionResult PcodeToAccount(string PCode)
        {
            //var model=db.
            return View();
        }
    }
}