using KDM.Helpers;
using KDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class UserMemberLinkController : Controller
    {
        // GET: UserMemberLink
        KDMDB db = new KDMDB();
        [HttpGet]
        public ActionResult UserMemberLink()
        {
            if(TempData["SuccessMessage"]!=null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            ViewBag.PageTitle = "Member MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["UserMemberLink"] = "User Member Link";
            ViewBag.PanelTitles = PanelTitles;
            ViewBag.UserID = User.Identity.GetUserId();
            var model = new UserMemberLinkVModel()
            {
                UserName = User.Identity.GetUserName()
            };
            ViewBag.Role = (new IdentityManager()).GetUsersRole(User.Identity.GetUserId());
            
            return View(model);
        }

        [HttpPost]
        public ActionResult UserMemberLink(UserMemberLinkVModel member)
        {
            ViewBag.PageTitle = "Member MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["UserMemberLink"] = "User Member Link";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Linked Member Failed.";
                return View(member);
            }
            else
            {
                TempData["SuccessMessage"] = "Successfully Linked Member.";

                //need to add condition for crossmatch member and User 
                tbl_user_member_master master = new tbl_user_member_master();
                master.MemberId = member.MemberId.Trim();

                IdentityManager im = new IdentityManager();
                List<string> role=im.GetUsersRole(User.Identity.GetUserId());
                if(role.Contains("ADMIN"))
                {
                    master.UserId = im.GetUserIdByName(member.UserName);
                }
                else
                {
                    master.UserId = User.Identity.GetUserId();
                }
                
                master.ModificationDate = DateTime.Now;

                db.tbl_user_member_master.Add(master);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("UserMemberLink");
            }
        }

        [HttpGet]
        public ActionResult LinkedMember()
        {
            ViewBag.PageTitle = "Member MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["LinkedMember"] = "Linked Member";
            ViewBag.PanelTitles = PanelTitles;

            string user = User.Identity.GetUserId();
            var umlList = db.tbl_user_member_master.Where(x => x.UserId == user).ToList();

            List<UserMemberLinkVModel> model = new List<UserMemberLinkVModel>();
            umlList.ForEach(x =>
            {
                UserMemberLinkVModel uml = new UserMemberLinkVModel();
                uml.UserId = x.UserId;
                uml.MemberId = x.MemberId;
                uml.ID = x.ID;
                uml.ModificationDate = (DateTime)x.ModificationDate;
                model.Add(uml);
            });

            return View(model);
        }

        public ActionResult UserMemberLinkDelete(int id)
        {
            var isExist = db.tbl_user_member_master.FirstOrDefault(x => x.ID == id);
            if (isExist != null)
            {
                db.tbl_user_member_master.Remove(isExist);
                db.SaveChanges();
                return RedirectToAction("LinkedMember");
            }
            else
            {
                return RedirectToAction("LinkedMember");
            }
        }

        [HttpGet]
        public ActionResult LinkedMemberTransfer()
        {
            ViewBag.PageTitle = "Member MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["MemberTransfer"] = "Member Transfer";
            ViewBag.PanelTitles = PanelTitles;

            return View();
        }

        [HttpPost]
        public ActionResult LinkedMemberTransfer(UserMemberLinkVModel model)
        {
            ViewBag.PageTitle = "Member MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["MemberTransfer"] = "Member Transfer";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Linked Member Transfer Failed.";
                return View(model);
            }
            else
            {
                ViewBag.SuccessMessage = "Successfully Transfer Linked Member.";

                tbl_user_member_transfer_history transfer = new tbl_user_member_transfer_history();
                transfer.MemberId = model.MemberId;
                transfer.From = model.From;
                transfer.To = model.To;
                transfer.TransferBy = User.Identity.GetUserId();
                transfer.TransferDate = DateTime.Now;
                db.tbl_user_member_transfer_history.Add(transfer);

                var IsExistInMaster = db.tbl_user_member_master.FirstOrDefault(x => x.MemberId == model.MemberId.Trim() && x.UserId == model.From.Trim());
                if(IsExistInMaster != null)
                {
                    db.tbl_user_member_master.Remove(IsExistInMaster);

                    
                    tbl_user_member_master master = new tbl_user_member_master();
                    master.MemberId = model.MemberId;
                    master.UserId = User.Identity.GetUserId();
                    master.ModificationDate = DateTime.Now;

                    db.tbl_user_member_master.Add(master);

                    tbl_members member = new tbl_members();
                    member = db.tbl_members.FirstOrDefault(x => x.Phone1 == model.From);
                    member.Phone1 = model.To;
                    db.Entry(member).State = EntityState.Modified;
                }
                
                db.SaveChanges();
                ModelState.Clear();
                return View();
            }
        }
    }
}