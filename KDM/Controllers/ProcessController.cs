using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Helpers;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class ProcessController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Process
        public ActionResult Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartBinaryProcess()
        {
            string ProcessNo = DateTime.Now.Date.ToString("yyyyMMdd");

            var isExits = db.tbl_process_code.Where(x => x.ProcessNo == ProcessNo).FirstOrDefault();
            if(isExits==null)
            {
                DateTime postingDate = DateTime.Now;
                var sql = "insert into tbl_PvBv_history(MemberID,PVPoint,BVPoint,PostingDate,ProcessCode) select PlacementID,PV,BV, {0},{1} from tbl_member_tree where PV>0 or BV>0  ";
                db.Database.ExecuteSqlCommand(sql, postingDate, ProcessNo);               

                BTreeHelpers bth = new BTreeHelpers();
                bth.ProcessBinaryPVBV();
              //  bth.BinaryMatchingnProcess(ProcessNo);
              //  bth.MatchingToMemberAccount(ProcessNo);
            }
            else
            {
                return Content("Already Processed current date");
            }

            tbl_process_code tbl = new tbl_process_code();
            tbl.ProcessNo = ProcessNo;
            db.tbl_process_code.Add(tbl);
            db.SaveChanges();

           //   bth.BinaryGenerationProcess();

            TempData["SuccessMessage"] = "Process Completed";
        
            return RedirectToAction("index");
        }

        public ActionResult GenerationProcessMatchCountList()
        {
            var model = db.tbl_binary_matching_data.ToList();
            return View(model);
        }

        public ActionResult startMatching()
        {
            return View();
        }

        public ActionResult startMatchingAction()
        {
            //string ProcessNo = Convert.ToString(db.tbl_process_code.OrderByDescending(x => x.ProcessNo).Select(s => s.ProcessNo).Take(1));
            BTreeHelpers bth = new BTreeHelpers();
            //bth.BinaryMatchingnProcess(ProcessNo);
            //bth.MatchingToMemberAccount(ProcessNo);
            string BPCode = DateTime.Now.Date.ToString("yyyyMMddhhmmss");
            bth.BinaryMatchingnProcess(BPCode);

            return View();
        }




    }
}