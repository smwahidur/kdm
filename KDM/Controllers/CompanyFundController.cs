using KDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class CompanyFundController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: CompanyFund
        public ActionResult CompanyFundHistory(DateTime? fDate = null, DateTime? tDate = null)
        {
            ViewBag.PageTitle = "Fund MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["FundHistory"] = "Fund History";
            ViewBag.PanelTitles = PanelTitles;

            ViewBag.fDate = (fDate == null) ? DateTime.Now : fDate;
            ViewBag.tDate = (tDate == null) ? DateTime.Now : tDate;

            if (fDate != null & tDate != null)
            {
                DateTime TDate = (DateTime) tDate;
                TDate = TDate.AddDays(1);
                ViewBag.FundHistoryList = db.tbl_company_funds_history.Where(x => x.CreateDate >= fDate && x.CreateDate < TDate);
            }
            else
            {
                ViewBag.FundHistoryList = db.tbl_company_funds_history.ToList();
            }

            return View();
        }

        public ActionResult SearchFund(CommonSearchVModel model)
        {
            return RedirectToAction("CompanyFundHistory", new
            {
                fDate = model.FDate,
                tDate = model.TDate
            });
        }

        public ActionResult CompanyFundMaster()
        {
            ViewBag.PageTitle = "Fund MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["FundMaster"] = "Fund Master";
            ViewBag.PanelTitles = PanelTitles;

            ViewBag.FundMasterList = db.tbl_company_funds_master.ToList();

            return View();
        }

        public ActionResult MemberBonusHistory(DateTime? fDate = null, DateTime? tDate = null)  
        {
            ViewBag.PageTitle = "Fund MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["BonusHistory"] = "Member Bonus History";
            ViewBag.PanelTitles = PanelTitles;

            ViewBag.fDate = (fDate == null) ? DateTime.Now : fDate;
            ViewBag.tDate = (tDate == null) ? DateTime.Now : tDate;

            if (fDate != null & tDate != null)
            {
                DateTime TDate = (DateTime)tDate;
                TDate = TDate.AddDays(1);
                ViewBag.MemberBonusHistoryList = db.tbl_member_bonus_history.Where(x => x.BonusDate >= fDate && x.BonusDate < TDate);
            }
            else
            {
                ViewBag.MemberBonusHistoryList = db.tbl_member_bonus_history.ToList();
            }

            return View();
        }

        public ActionResult SearchMemberBonusHistory(CommonSearchVModel model)
        {
            return RedirectToAction("MemberBonusHistory", new
            {
                fDate = model.FDate,
                tDate = model.TDate
            });
        }

        public ActionResult MemberBonusMaster()
        {
            ViewBag.PageTitle = "Fund MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["MemberBonusMaster"] = "Member Bonus Master";
            ViewBag.PanelTitles = PanelTitles;

            ViewBag.FundMasterList = db.tbl_member_bonus_master.ToList();

            return View();
        }

    }
}