using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Models;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class PaymentController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Withdraw()
        {
            ViewBag.PageTitle = "Payment MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PaymentWithdraw"] = "ADD NEW Payment";
            ViewBag.PanelTitles = PanelTitles;
            return View();
        }

        [HttpPost]
        public ActionResult Withdraw(TransectionVModel model)
        {
            ViewBag.PageTitle = "Payment MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PaymentWithdraw"] = "ADD NEW Payment";
            ViewBag.PanelTitles = PanelTitles;

            if (ModelState.IsValid)
            {
                using (var tr = db.Database.BeginTransaction())
                {
                    try
                    {
                        tbl_transaction transection = new tbl_transaction();
                        transection.CreditACcount = model.CreditACcount;
                        transection.DebitAccount = model.DebitAccount;
                        transection.Amount = model.Amount;
                        transection.PostingDates = DateTime.Now;
                        transection.Particulars = model.Particulars;
                        transection.TFNumber = "";
                        db.tbl_transaction.Add(transection);

                        db.SaveChanges();
                        tr.Commit();
                        ViewBag.SMsg = "Payment Withdraw successful.";
                        return View();
                    }
                    catch
                    {
                        tr.Rollback();
                        ViewBag.EMsg = "Payment Withdraw failed";
                        return View(model);
                    }
                }
            }
            else
            {
                return View(model);
            }    
        }

        public ActionResult WithdrawAction()
        {
            return View();
        }

        public ActionResult CashReceive()
        {
            return View();
        }

        public ActionResult CashReceiveAction()
        {
            return View();
        }
    }
}