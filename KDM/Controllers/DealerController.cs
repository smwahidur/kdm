using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf.qrcode;
using KDM.Models;
using Microsoft.AspNet.Identity;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class DealerController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Dealer
        [HttpGet]
        public ActionResult AddDealer() 
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddDealer"] = "Add Dealer";
            ViewBag.PanelTitles = PanelTitles;

            return View();
        }

        [HttpPost]
        public ActionResult AddDealer(DealerVModel model)
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddDealer"] = "Add Dealer";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dealer Created Failed.";
                return View(model);
            }
            else
            {
                ViewBag.SuccessMessage = "Dealer Created Successfully.";

                tbl_dealers dealer = new tbl_dealers();
                dealer.DealerType = model.DealerType;
                var lastdealer = db.tbl_dealers.ToList();
                if (lastdealer == null)
                {
                    dealer.DealerID = 1;
                }
                else
                {
                    dealer.DealerID = lastdealer.Max(x => x.DealerID) + 1;
                }
                dealer.Name = model.Name;
                dealer.District = model.District;
                dealer.Thana = model.Thana;
                dealer.DealerUnion = model.DealerUnion;
                dealer.Address = model.Address;
                dealer.Phone1 = model.Phone1;
                dealer.Phone2 = model.Phone2;
                dealer.DateOfOperation = model.DateOfOperation;
                dealer.PrimaryDeposit = model.PrimaryDeposit;
                //dealer.PhotoID = 
                dealer.UserID = model.UserID;
                dealer.CreateBy = User.Identity.Name;
                dealer.CreateDate = DateTime.Now;

                db.tbl_dealers.Add(dealer);
                db.SaveChanges();
                ModelState.Clear();
                return View();
            }
        }

        public ActionResult DealerList()
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";
            var dealerList = db.tbl_dealers.ToList();

            List<DealerVModel> model = new List<DealerVModel>();
            dealerList.ForEach(x =>
            {
                DealerVModel dealer = new DealerVModel();
                dealer.DealerType = (int)x.DealerType;
                dealer.DealerID = x.DealerID;
                dealer.Name = x.Name;
                dealer.District = (int)x.District;
                dealer.Thana = (int)x.Thana;
                dealer.DealerUnion = (int)x.DealerUnion;
                dealer.Address = x.Address;
                dealer.Phone1 = x.Phone1;
                dealer.Phone2 = x.Phone2;
                dealer.DateOfOperation = (DateTime)x.DateOfOperation;
                dealer.PrimaryDeposit = (decimal)x.PrimaryDeposit;
                //dealer.PhotoID = 
                dealer.UserID = x.UserID;
                dealer.CreateBy = x.CreateBy;
                dealer.CreateDate = (DateTime)x.CreateDate;

                model.Add(dealer);
            });

            return View(model);
        }

        [HttpGet]
        public ActionResult DealerEdit(int DealerId)
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["DealerEdit"] = "Dealer Edit";
            ViewBag.PanelTitles = PanelTitles;
            var dealerList = db.tbl_dealers.FirstOrDefault(x => x.DealerID == DealerId);

            DealerVModel dealer = new DealerVModel();
            dealer.DealerType = (int)dealerList.DealerType;
            dealer.DealerID = dealerList.DealerID;
            dealer.Name = dealerList.Name;
            dealer.District = (int)dealerList.District;
            dealer.Thana = (int)dealerList.Thana;
            dealer.DealerUnion = (int)dealerList.DealerUnion;
            dealer.Address = dealerList.Address;
            dealer.Phone1 = dealerList.Phone1;
            dealer.Phone2 = dealerList.Phone2;
            dealer.DateOfOperation = (DateTime)dealerList.DateOfOperation;
            dealer.PrimaryDeposit = (decimal)dealerList.PrimaryDeposit;
            //dealer.PhotoID = 
            dealer.UserID = dealerList.UserID;
            dealer.CreateBy = dealerList.CreateBy;
            dealer.CreateDate = (DateTime)dealerList.CreateDate;

            return View(dealer);
        }

        [HttpPost]
        public ActionResult DealerEdit(DealerVModel model)
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["DealerEdit"] = "Dealer Edit";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dealer Updated Failed.";
                return View(model);
            }
            else
            {
                ViewBag.SuccessMessage = "Dealer Updated Successfully.";

                var dealerExist = db.tbl_dealers.FirstOrDefault(x => x.DealerID == model.DealerID);
                db.tbl_dealers.Remove(dealerExist);

                tbl_dealers dealer = new tbl_dealers();
                dealer.DealerType = model.DealerType;
                var lastdealer = db.tbl_dealers.ToList();
                if (lastdealer == null)
                {
                    dealer.DealerID = 1;
                }
                else
                {
                    dealer.DealerID = lastdealer.Max(x => x.DealerID) + 1;
                }
                dealer.Name = model.Name;
                dealer.District = model.District;
                dealer.Thana = model.Thana;
                dealer.DealerUnion = model.DealerUnion;
                dealer.Address = model.Address;
                dealer.Phone1 = model.Phone1;
                dealer.Phone2 = model.Phone2;
                dealer.DateOfOperation = model.DateOfOperation;
                dealer.PrimaryDeposit = model.PrimaryDeposit;
                //dealer.PhotoID = 
                dealer.UserID = model.UserID;
                dealer.CreateBy = User.Identity.Name;
                dealer.CreateDate = DateTime.Now;

                db.tbl_dealers.Add(dealer);
                db.SaveChanges();
                //ModelState.Clear();
                return View(model);
            }
        }

        public ActionResult DealerDetails(int DealerId) 
        {
            ViewBag.PageTitle = "Dealer MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["DealerDetails"] = "Dealer Details";
            ViewBag.PanelTitles = PanelTitles;
            var dealerList = db.tbl_dealers.FirstOrDefault(x => x.DealerID == DealerId);

            DealerVModel dealer = new DealerVModel();
            dealer.DealerType = (int)dealerList.DealerType;
            dealer.DealerID = dealerList.DealerID;
            dealer.Name = dealerList.Name;
            dealer.District = (int)dealerList.District;
            dealer.Thana = (int)dealerList.Thana;
            dealer.DealerUnion = (int)dealerList.DealerUnion;
            dealer.Address = dealerList.Address;
            dealer.Phone1 = dealerList.Phone1;
            dealer.Phone2 = dealerList.Phone2;
            dealer.DateOfOperation = (DateTime)dealerList.DateOfOperation;
            dealer.PrimaryDeposit = (decimal)dealerList.PrimaryDeposit;
            //dealer.PhotoID = 
            dealer.UserID = dealerList.UserID;
            dealer.CreateBy = dealerList.CreateBy;
            dealer.CreateDate = (DateTime)dealerList.CreateDate;

            return View(dealer);
        }

        public ActionResult DealerDelete(int DealerId)
        {
            var dealerList = db.tbl_dealers.FirstOrDefault(x => x.DealerID == DealerId);
            db.tbl_dealers.Remove(dealerList);
            db.SaveChanges();
            return RedirectToAction("DealerList");
        }

        public ActionResult ListForDear()
        {
            var dealerID = User.Identity.GetUserId();
            //var orders=db.tbl_orders.Where(x=>x.DealerID==dealerID && x.OrderStatus==)
            return View();
        }
    }
}