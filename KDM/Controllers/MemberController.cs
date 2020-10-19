using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Models;
using KDM.Helpers;
using System.IO;
using Serilog;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class MemberController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TreeView(string uID)
        {
            if(!String.IsNullOrWhiteSpace(uID))
            {
                BTreeHelpers bHelper = new BTreeHelpers();

                NodeView root = new NodeView(uID);
                var jsonData = bHelper.BuildMemberTreeBFS(root, 30);
                ViewBag.JsonData = jsonData;
            }
            else
            {
                ViewBag.JsonData = "{}";
            }
            

            return View();
        }

        public ActionResult AddMember()
        {
            //AddOLDMember();

            ViewBag.PageTitle = "Member Registration";

            if (TempData["SMsg"] != null)
                ViewBag.SMsg = TempData["SMsg"];
            if (TempData["EMsg"] != null)
                ViewBag.EMsg = TempData["EMsg"];
            if (TempData["WMsg"] != null)
                ViewBag.WMsg = TempData["WMsg"];

            return View();
        }

        public ActionResult GetSponsorName(string spId)
        {
            if (spId != "")
            {
                string SPName = db.tbl_members.Where(x => x.MemberID == spId).Select(x => x.DistributorName).FirstOrDefault();
                if(SPName!=null)
                return Json(SPName, JsonRequestBehavior.AllowGet);
                
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }

       public string GETMemberID(string UserName)
        {
            var SPName = db.tbl_members.FirstOrDefault(x => x.Phone1 == UserName);
            string memberId = "";

            if (SPName != null)
            {
                memberId = SPName.MemberID;
            }
            return memberId;
        }

        public ActionResult GetPlacementDetails(string PlacementID)
        {
            if (PlacementID != "")
            {
                string oldplacement = db.tbl_members.Where(x => x.MemberID == PlacementID).Select(s => s.PlacementID).FirstOrDefault();
                
                var placement = db.tbl_member_tree.Where(x => x.PlacementID == oldplacement).FirstOrDefault();
                if (placement != null)
                    return Json(placement, JsonRequestBehavior.AllowGet);

                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        // Method ID: M_0000011
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember(MemberRegistrationViewModel member)
        {
           string oldplacementId = db.tbl_members.Where(x => x.MemberID == member.PlacementID).Select(s => s.PlacementID).FirstOrDefault();
            string newMemmberID = "-1";
            tbl_members newMember = new tbl_members();
            using (var tr=db.Database.BeginTransaction())
            {
                //try
                //{
                    if(!ModelState.IsValid)
                    {
                        ViewBag.EMsg = "Input validation failed.";
                        return View(member);
                    }

                    newMemmberID = KDMSettings.GetNextMemberID(db);
                    if(newMemmberID == "-1")
                    {
                        Log.Warning("[M_0000011] Can't create new placement ID");
                        ViewBag.EMsg = "Can't create the new placement id";
                        return View();

                    }
                    var memberIDExist = db.tbl_members.Where(x => x.MemberID==newMemmberID).FirstOrDefault();
                    var phoneExist = db.tbl_members.Where(x => x.Phone1 == member.Phone1 ).FirstOrDefault();
                     if (memberIDExist!=null)
                    {
                        ViewBag.EMsg = "Member No already exits";
                        ViewBag.EMsg = "Phone number already exist. Register with new phone numbers";
                        return View(member);
                    }
                    if(phoneExist!=null)
                    {
                   
                    ViewBag.EMsg = "Phone number already exist. Register with new phone numbers";
                    return View(member);
                    }
                int checkdata = MemberTreeAvaility(oldplacementId, Convert.ToString(member.PlacementPosition), newMemmberID);
                if(checkdata!=1)
                {
                    ViewBag.EMsg = "Placement is not ok";
                    return View(member);
                }
                newMember.MemberID = newMemmberID;
                    newMember.SponsorID = member.SponsorID;
                 //   newMember.PlacementID = member.PlacementID;  // child of                    
                    newMember.PlacementID = newMemmberID;  // for new member                   
                    newMember.DistributorName = member.DistributorName;
                    newMember.FathersName = member.FathersName;
                    newMember.MothersName = member.MothersName;
                    newMember.PresentAddress = member.PresentAddress;
                    newMember.PermanentAddress = member.PermanentAddress;
                    newMember.NID = member.NID;
                    newMember.Phone1 = member.Phone1;
                    newMember.Phone2 = member.Phone2;
                    newMember.NomineeName = member.NomineeName;
                    newMember.RelationWithNominee = member.RelationWithNominee;
                    newMember.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;
                    newMember.CreateBy = User.Identity.Name;
                   
                    newMember.CreateDate = DateTime.Now;
                    db.tbl_members.Add(newMember);

                    if (member.Photo != null)
                    {
                        //string photoId = ControllerHelper.SaveFile(member.Photo);
                        string photoId = SaveFile(member.Photo);
                        //newMember.PhotoID = Convert.ToInt64(photoId);


                        if (!String.IsNullOrWhiteSpace(photoId))
                        {
                            db.tbl_file.Add(new tbl_file()
                            {
                                file_title = "Member Id",
                                file_type = "Member",
                                file_details = photoId
                            });
                        }
                    }


                    #region create new entry for new member in tbl_member_tree

                    db.tbl_member_tree.Add(new tbl_member_tree() { 
                        PlacementID = newMemmberID,
                        LeftPoint=0,
                        RightPoint=0,
                        PV=0,
                        BV=0,
                        TotalBV=0,
                        LeftChildCount=0,
                        RightChildCount=0
                    });

                    #endregion

                    #region Placement the member
                    var placement = db.tbl_member_tree.Where(x => x.PlacementID == oldplacementId).FirstOrDefault();
                if (placement != null)
                {
                    if (!String.IsNullOrWhiteSpace(placement.LeftID) && !String.IsNullOrWhiteSpace(placement.RightID))
                    {
                        ViewBag.EMsg = "Placement id left and right is not blank";
                        return View(member);
                    }
                    if (member.PlacementPosition == "Left" && placement.PlacementID != newMemmberID)
                    {
                        placement.LeftID = newMemmberID;
                        //placement.TotalBV = 0;
                        placement.LeftPoint = 0;
                        //placement.RightPoint = 0;
                        //placement.BV = 0;
                        //placement.PV = 0;

                    }
                    else if (member.PlacementPosition == "Right" && placement.PlacementID != newMemmberID)
                    {
                        placement.RightID = newMemmberID;
                        //placement.TotalBV = 0;
                        //placement.LeftPoint = 0;
                        placement.RightPoint = 0;
                        //placement.BV = 0;
                        ///placement.PV = 0;
                    }
                }
                else
                {
                    ViewBag.EMsg = "Placement id does not exist.";
                    return View(member);
                }
                    #endregion

                    #region create new user if withpassword set true
                    if (member.WithPassword && !String.IsNullOrWhiteSpace(member.Phone1))
                    {
                        IdentityManager im = new IdentityManager();
                        ApplicationUser newUser = new ApplicationUser();
                        newUser.UserName = member.Phone1;
                        newUser.IsActive = true;
                        newUser.ChangePassword =true;
                        
                        if(im.CreateUser(newUser, KDMEnvironmentConstants.DefaultUserPassword))
                        {
                            if(!im.IsRoleExists(KDMEnvironmentConstants.DefaultUserRole))
                            {
                                im.CreateRole(KDMEnvironmentConstants.DefaultUserRole);

                            }
                            if(!im.AddUserToRole(member.Phone1, KDMEnvironmentConstants.DefaultUserRole))
                            {
                                ViewBag.EMsg = "Can't create the role";
                                return View(member);
                            }
                        }
                        else
                        {
                            ViewBag.EMsg = "Can't create the user";
                            return View(member);
                        }
                    }
                    #endregion
                    db.SaveChanges();
                    tr.Commit();

                    ViewBag.SMsg = "Member Successfully created. Your new member id " + newMemmberID;
             //   }
                //catch(Exception ex)
                //{
                //    tr.Rollback();
                //    Log.Error(ex, "[M_0000011]");
                //    ViewBag.EMsg = "Can't create new member";
                //}
            }

            return View(member);

        }
        public int MemberTreeAvaility(string ParentID, string ChildPosition, string newMemmberID)
        {
            int returnid = 1;
            var placement = db.tbl_member_tree.Where(x => x.PlacementID == ParentID).FirstOrDefault();
          
            if (placement != null && (placement.PlacementID != newMemmberID || placement.LeftID!= newMemmberID || placement.RightID != newMemmberID))
            {
                if (ChildPosition == "Left" && String.IsNullOrWhiteSpace(placement.LeftID) )
                {
                    returnid = 1;
                }
                else if (ChildPosition == "Right" && String.IsNullOrWhiteSpace(placement.RightID))
                {
                    returnid = 1;
                }
                else
                {
                    returnid = 0;
                }
            }
            else
            {
                returnid = 0;

            }

            return returnid;
        }

        public ActionResult SelectForUpdate()
        {
            return View();
        }
        public ActionResult UpdateMember(string MemberID="")
        {
            MemberRegistrationViewModel member = new MemberRegistrationViewModel();
            member.PlacementID = MemberID;
            var data = db.tbl_members.Where(x => x.MemberID == MemberID).FirstOrDefault();
            
            if(data!=null)
            {
                var treeData = db.tbl_member_tree.Where(x => x.PlacementID == data.PlacementID).FirstOrDefault();
                member.ID = data.ID;
                member.PlacementID = data.PlacementID;
                member.MemberID = data.MemberID;
                member.SponsorID = data.SponsorID;
                member.SponsorName = data.SponsorName;
                member.Position = data.Position??-1;
                member.DistributorName = data.DistributorName;
                member.FathersName = data.FathersName;
                member.MothersName = data.MothersName;
                member.PresentAddress = data.PresentAddress;
                member.PermanentAddress = data.PermanentAddress;
                member.NID = data.NID;
                member.Phone1 = data.Phone1;
                member.Phone2 = data.Phone2;
                member.NomineeName = data.NomineeName;
                member.RelationWithNominee = data.RelationWithNominee;
                member.NomineeNIDOrBirthCertificate = data.NomineeNIDOrBirthCertificate;
                member.PhotoID = data.PhotoID??-1;

                if(treeData!=null)
                {
                    member.LeftID = treeData.LeftID;
                    member.RightID = treeData.RightID;
                }
            }
            
            if(data==null && MemberID!=null)
            {
                ViewBag.EMsg = "Member not found.";
            }

            return View(member);
        }

        // Method ID: M_0000015
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMember(MemberRegistrationViewModel model)
        {
            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.EMsg = "Input validation failed.";
                        return View(model);
                    }

                    #region Update member details

                    var member = db.tbl_members.Where(x => x.MemberID == model.MemberID).FirstOrDefault();
                    if(member!=null)
                    {

                        if(User.IsInRole("admin"))
                        {
                            member.SponsorID = model.SponsorID;
                        }
                        
                        member.Position = model.Position;
                        member.DistributorName = model.DistributorName;
                        member.FathersName = model.FathersName;
                        member.MothersName = model.MothersName;
                        member.PresentAddress = model.PresentAddress;
                        member.PermanentAddress = model.PermanentAddress;
                        member.NID = model.NID;
                        member.Phone2 = model.Phone2;
                        member.NomineeName = model.NomineeName;
                        member.RelationWithNominee = model.RelationWithNominee;
                        member.NomineeNIDOrBirthCertificate = model.NomineeNIDOrBirthCertificate;
                    }

                    #endregion 

                    #region Placement the member
                    var placement = db.tbl_member_tree.Where(x => x.PlacementID == member.PlacementID).FirstOrDefault();
                    if (placement != null)
                    {
                        if (model.PlacementPosition == "Left" && String.IsNullOrWhiteSpace(placement.LeftID))
                        {
                            placement.LeftID = model.MemberID;
                        }
                        else if (model.PlacementPosition == "Right" && String.IsNullOrWhiteSpace(placement.RightID))
                        {
                            placement.RightID = model.MemberID;
                        }
                    }
                    else
                    {
                        ViewBag.EMsg = "Placement id does not exist.";
                        return View(model);
                    }
                    #endregion

                    db.SaveChanges();
                    tr.Commit();

                    ViewBag.SMsg = "Member "+model.MemberID+" successfully saved";
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Log.Error(ex, "[M_0000015]");
                    ViewBag.EMsg = "Can't save member details";
                }
            }

            return View(model);

        }

        public ActionResult ShowPlacementId(string placementId)
        {
            var member = db.tbl_members.FirstOrDefault(x => x.PlacementID == placementId);

            MemberRegistrationViewModel model = new MemberRegistrationViewModel();
            model.ID = member.ID;
            model.PlacementID = member.PlacementID;
            model.SponsorID = member.SponsorID;
            model.SponsorName = member.SponsorName;
            model.Position = (int)member.Position;
            model.DistributorName = member.DistributorName;
            model.FathersName = member.FathersName;
            model.MothersName = member.MothersName;
            model.PresentAddress = member.PresentAddress;
            model.PermanentAddress = member.PermanentAddress;
            model.NID = member.NID;
            model.Phone1 = member.Phone1;
            model.Phone2 = member.Phone2;
            model.NomineeName = member.NomineeName;
            model.RelationWithNominee = member.RelationWithNominee;
            model.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;

            return View(model);

        }

        public ActionResult SelectMemberList()
        {
            return View();
        }
        public ActionResult MemberList()
        {
            // var memberList = db.tbl_members.ToList();

            using (var db = new KDMDB())
            {
                var memberList = db.tbl_members
                                    .SqlQuery("Select * from tbl_members")
                                    .ToList<tbl_members>();
                return View(memberList);
            }
            
        }

        public ActionResult MemberDetails(int id, string returnUrl)
        {
            ViewBag.Error = "";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "Member Management";

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();

            var member = db.tbl_members.FirstOrDefault(x => x.ID == id);

            if (member != null)
            {
                MemberRegistrationViewModel model = new MemberRegistrationViewModel();
                model.ID = member.ID;
                model.MemberID = member.MemberID;
                model.PlacementID = member.PlacementID;
                model.SponsorID = member.SponsorID;
                model.SponsorName = member.SponsorName;
                //model.Position = (int)member.Position;
                model.DistributorName = member.DistributorName;
                model.FathersName = member.FathersName;
                model.MothersName = member.MothersName;
                model.PresentAddress = member.PresentAddress;
                model.PermanentAddress = member.PermanentAddress;
                model.NID = member.NID;
                model.Phone1 = member.Phone1;
                model.Phone2 = member.Phone2;
                model.NomineeName = member.NomineeName;
                model.RelationWithNominee = member.RelationWithNominee;
                model.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;
                return View(model);
            }
            else
            {
                ViewBag.Error = "Member Not Found";
                return View();
            }
        }

        //public ActionResult UpdateMember(int id, string returnUrl)
        //{
        //    ViewBag.Error = "";
        //    ViewBag.ReturnUrl = returnUrl;
        //    ViewBag.PageTitle = "Member Management";

        //    if (TempData["ErrorMessage"] != null)
        //        ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

        //    if (TempData["SuccessMessage"] != null)
        //        ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();

        //    var member = db.tbl_members.FirstOrDefault(x => x.ID == id);

        //    if (member != null)
        //    {
        //        MemberRegistrationViewModel model = new MemberRegistrationViewModel();
        //        model.ID = member.ID;
        //        model.SponsorID = member.SponsorID;
        //        model.SponsorName = member.SponsorName;
        //        model.Position = (int)member.Position;
        //        model.DistributorName = member.DistributorName;
        //        model.FathersName = member.FathersName;
        //        model.MothersName = member.MothersName;
        //        model.PresentAddress = member.PresentAddress;
        //        model.PermanentAddress = member.PermanentAddress;
        //        model.NID = member.NID;
        //        model.Phone1 = member.Phone1;
        //        model.Phone2 = member.Phone2;
        //        model.NomineeName = member.NomineeName;
        //        model.RelationWithNominee = member.RelationWithNominee;
        //        model.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;
        //        ViewBag.Id = member.ID;
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.Error = "Member Not Found";
        //        return View();
        //    }
        //}

        //[HttpPost]
        //public ActionResult UpdateMemberResponse(MemberRegistrationViewModel member)
        //{
        //    var memberExist = db.tbl_members.FirstOrDefault(x => x.ID == member.ID);
        //    if (memberExist != null)
        //    {
        //        db.tbl_members.Remove(memberExist);

        //        tbl_members newMember = new tbl_members();
        //        newMember.ID = memberExist.ID;
        //        newMember.SponsorID = member.SponsorID;
        //        newMember.SponsorName = member.SponsorName;
        //        newMember.Position = member.Position;
        //        newMember.DistributorName = member.DistributorName;
        //        newMember.FathersName = member.FathersName;
        //        newMember.MothersName = member.MothersName;
        //        newMember.PresentAddress = member.PresentAddress;
        //        newMember.PermanentAddress = member.PermanentAddress;
        //        newMember.NID = member.NID;
        //        newMember.Phone1 = member.Phone1;
        //        newMember.Phone2 = member.Phone2;
        //        newMember.NomineeName = member.NomineeName;
        //        newMember.RelationWithNominee = member.RelationWithNominee;
        //        newMember.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;
        //        newMember.CreateBy = "";
        //        newMember.CreateDate = DateTime.Now;
        //        //newMember.PhotoID = "";
        //        //newMember.Photo = "";

        //        db.tbl_members.Add(newMember);
        //        //db.Entry(newMember).State = EntityState.Modified;

        //        db.SaveChanges();

        //        return Json(new { newMember }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberDelete(int id)
        {
            var memberExist = db.tbl_members.FirstOrDefault(x => x.ID == id);
            if (memberExist != null)
            {
                db.tbl_members.Remove(memberExist);
                db.SaveChanges();
                return RedirectToAction("MemberList", "Member");
            }
            else
            {
                return RedirectToAction("MemberList", "Member");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public string SaveFile(HttpPostedFileBase file)
        {
            string fileID = "";
            string fileType = "";
            string fileRelativePath = "";
            string filePath = "";

            if (file != null)
            {
                try
                {
                    fileID = Guid.NewGuid().ToString();
                    fileType = "jpg";
                    var fileName = fileID + "." + fileType;
                    fileRelativePath = Path.Combine("~/Content/Member/Photos", fileName);
                    filePath = System.IO.Path.GetFullPath(fileRelativePath);
                    //filePath = HostingEnvironment.MapPath(fileRelativePath);
                    //filePath = HttpContext.Current.Server.MapPath(fileRelativePath);
                    file.SaveAs(filePath);
                }
                catch
                {

                }

            }

            return fileID;
        }


        public ActionResult SetPoints()
        {
            ViewBag.PageTitle = "SET POINTS";

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPoints(string MemberID, double PV=0, double BV=0)
        {
            if(!String.IsNullOrWhiteSpace(MemberID))
            {
                var member = db.tbl_member_tree.Where(x => x.PlacementID == MemberID).FirstOrDefault();
                if(member!=null)
                {
                    member.PV = PV;
                    member.BV = BV;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Successfully saved";
                }
                else
                {
                    TempData["ErrorMessage"] = "Can't save the data";
                }
            }
            else
                TempData["ErrorMessage"] = "Member id is empty";

            return RedirectToAction("SetPoints");
        }



        public ActionResult MemberAddToBinaryList()
        {
            return View();
        }

        //MethodID  M_0000012
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberAddToBinaryList(string ParentID, string ChildID)
        {
            try
            {
                var parent = db.tbl_member_tree.Where(x => x.PlacementID == ParentID).FirstOrDefault();
                if (parent != null)
                {
                    var child = db.tbl_members.Where(x => x.PlacementID == ChildID).FirstOrDefault();
                    if(child!=null)
                    {
                        if (String.IsNullOrWhiteSpace(parent.LeftID))
                            parent.LeftID = ChildID;
                        else if (String.IsNullOrWhiteSpace(parent.RightID))
                            parent.RightID = ChildID;
                        db.SaveChanges();
                        ViewBag.SMsg = "Successfully Member Placed";
                    }
                    else
                    {
                        ViewBag.EMsg = "Child Member not Found";
                    }
                    
                }
                else
                {
                    ViewBag.EMsg = "Parent Member not Found";
                }
            }
            catch(Exception ex)
            {
                ViewBag.EMsg = "Can't Placed the Member";
                Log.Error(ex, "[M_0000012]");
            }

            return View();
        }

        public void AddOLDMember()
        {

            //var memberTree = db.tbl_member_tree.ToList();
            var memberTree = (from mt in db.tbl_member_tree
                             where !(from m in db.tbl_members
                                     select m.PlacementID)
                                    .Contains(mt.PlacementID)
                             select mt).OrderBy(o=>o.SerialNo).ToList();

            string newMemmberID = "-1";
           // using (var tr = db.Database.BeginTransaction())
           // {
                //    try
                //    {
                memberTree.ForEach(x =>
                    {
                        if (x.PlacementID != "al-arafa01" && x.PlacementID != "al-arafa02" && x.PlacementID != "al-arafa03" &&
                        x.PlacementID != "al-baraka01" && x.PlacementID != "al-baraka02" && x.PlacementID != "al-baraka03" &&
                        x.PlacementID != "jalal-01" && x.PlacementID != "jalal-02" && x.PlacementID != "w-founder81" && 
                        x.PlacementID != "bpd(r)" && x.PlacementID != "khairul0?0" && x.PlacementID != "rasad,05")
                        {
                            tbl_members newMember = new tbl_members();
                            newMemmberID = KDMSettings.GetNextMemberID(db);
                            newMember.MemberID = newMemmberID;
                            newMember.PlacementID = x.PlacementID;
                            newMember.Phone1 = x.PlacementID;
                            newMember.CreateBy = "Jobaed";
                            newMember.CreateDate = DateTime.Now;
                            db.tbl_members.Add(newMember);
                             db.SaveChanges();
                            IdentityManager im = new IdentityManager();
                            ApplicationUser newUser = new ApplicationUser();
                            newUser.UserName = x.PlacementID;
                            newUser.IsActive = true;

                            im.CreateUser(newUser, KDMEnvironmentConstants.DefaultUserPassword);
                            im.AddUserToRole(x.PlacementID, KDMEnvironmentConstants.DefaultUserRole);
                            //db.tbl_members.Add(newMember);
                            db.SaveChanges();
                        }
                    });
                    //tr.Commit();
                //}
                //catch (Exception ex)
                //{
                //    //tr.Rollback();
                //    Log.Error(ex, "[M_0000011]");
                //}
           // }
        }

        public ActionResult SearchForMember()
        {
           
            return View();
        }
        public ActionResult getMemberToUser(string MemberID)
        {
            MemberToUserViewModel model = new MemberToUserViewModel();
            model = db.tbl_members.Where(x => x.MemberID == MemberID).Select(x => new MemberToUserViewModel()
            {

                MemberID = x.MemberID,
                Phone1 = x.Phone1

            }).FirstOrDefault();

            if (model != null)
            {
                if (String.IsNullOrWhiteSpace(model.Phone1))
                {
                    ViewBag.EMsg = "Please update the member Phone 1 field";
                }

                var userExists = db.AspNetUsers.Where(x => x.UserName == model.Phone1 || x.UserName == model.MemberID).FirstOrDefault();
                if (userExists != null)
                {
                    ViewBag.SMsg = "User already exists";
                }
            }
            else
                ViewBag.EMsg = "Member does'nt exists.";


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult getMemberToUser(MemberToUserViewModel member)
        {
            if(!ModelState.IsValid)
            {
                return View(member);
            }
            var isexits = db.AspNetUsers.Where(x => x.UserName == member.Phone1).FirstOrDefault();
            if(isexits!=null)
            {
                ViewBag.EMsg = "User already exists";
                return View(member);
            }

            #region create new user if withpassword set true
            if (!String.IsNullOrWhiteSpace(member.Phone1))
            {
                IdentityManager im = new IdentityManager();
                ApplicationUser newUser = new ApplicationUser();
                newUser.UserName = member.Phone1;
                newUser.IsActive = true;
                newUser.ChangePassword = true;
                db.SaveChanges();
                if (im.CreateUser(newUser, KDMEnvironmentConstants.DefaultUserPassword))
                {
                    if (!im.IsRoleExists(KDMEnvironmentConstants.DefaultUserRole))
                    {
                        im.CreateRole(KDMEnvironmentConstants.DefaultUserRole);

                    }
                    if (!im.AddUserToRole(member.Phone1, KDMEnvironmentConstants.DefaultUserRole))
                    {
                        ViewBag.EMsg = "Can't create the role";
                        return View(member);
                    }
                }
                else
                {
                    ViewBag.EMsg = "Can't create the user";
                    return View(member);
                }
            }
            #endregion
            if (!ModelState.IsValid)
            {
                ViewBag.EMsg = "Input validation failed.";
                return View(member);
            }

            return View(member);
        }

        
        public string GetUserRole(string UserName)
        {
            var user = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName);
            string role = "";
            if (user != null)
            {
                role = (from r in db.AspNetRoles
                        join ur in db.AspNetUserRoles on r.Id equals ur.RoleId
                        where ur.UserId == user.Id
                        select r).FirstOrDefault().Name;
            }

            return role;
        }
    }
}