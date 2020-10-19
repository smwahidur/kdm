using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using KDM.Models;
using KDM.Helpers;
using KDM.Filters;


namespace KDM.Controllers
{
   // [Authorize]
   //[KDMActionFilter]
    public class AccountController : Controller
    {
        IdentityManager im = new IdentityManager();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                
                _signInManager = value;
            }

        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        
        //GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password,false, false);
            switch (loginResult)
            {
                case SignInStatus.Success:
                    {
                        ControllerHelper cHelper = new ControllerHelper();
                        IdentityManager im = new IdentityManager();
                        var userId = im.GetUserIdByName(model.UserName);
                        Session["user"] = userId;

                        if (im.IsChangePassword(userId))
                        {
                            return RedirectToAction("ChangeMyPassword", "Account", new { Area = "" });
                        }

                        List<string> roles = im.GetUsersRole(userId);
                        if (roles.Contains(KDMEnvironmentConstants.DefaultUserRole))
                        {
                            return RedirectToAction("Dashboard", "KDMMember", new { Area = "" });
                        }

                        UserRoute userRoute = cHelper.GetDefaultRoute(userId);
                        if(userRoute==null)
                        {
                            return RedirectToAction("Index", "Default", new { Area = "" });
                        }
                        if(!String.IsNullOrWhiteSpace(userRoute.Controller)&&!String.IsNullOrWhiteSpace(userRoute.Action))
                        return RedirectToAction(userRoute.Action,userRoute.Controller);
                        else
                            return RedirectToAction("Index", "Default", new { Area = "" });

                    }
                    
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                    ViewBag.ErrorMessage = "Wrong user name or password";
                    return View(model);
                default:
                    ViewBag.ErrorMessage = "Wrong user name or password";
                    return View(model);
            }

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Logoff(string returnUrl)
        {
            if(User.Identity.IsAuthenticated)
            {
                ViewBag.ReturnUrl = returnUrl;
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            
            return RedirectToAction("index", "Home");
        }

        [HttpGet]
        public ActionResult UserCreate(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddNewUser"] = "ADD NEW USER";
            ViewBag.PanelTitles = PanelTitles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreate(CreateNewUserViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddNewUser"] = "ADD NEW USER";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Can't create the user";
                return View(model);
            }

            var userId = im.GetUserIdByName(model.UserName);

            if (!String.IsNullOrWhiteSpace(userId))
            {
                ViewBag.ErrorMessage = "User already exits.";
                return View(model);
            }
            
            ApplicationUser user = new ApplicationUser();
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.IsActive = false;

            if (im.CreateUser(user, model.Password))
            {
                ViewBag.SuccessMessage = "User successfully created";
            }
            
            ModelState.Clear();
            return View();
        }


        public ActionResult SearchUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchUser(string userName)
        {
            ViewBag.UserID = null;
            ViewBag.UserName = null;
            ViewBag.Roles = null;

            var userID= im.GetUserIdByName(userName);
            if(!String.IsNullOrWhiteSpace(userID))
            {
                ViewBag.UserName = userName;
                ViewBag.UserID = userID;
                ViewBag.Roles = im.GetUserRolesByUserName(userName);
            }
            else
            {
                ViewBag.EMsg = "User not found";
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateUser(string userName)
        {
            
                im.ActivateUser(userName);

            return RedirectToAction("UsersList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeActivateUser(string userName)
        {

            im.DeactivateUser(userName);

            return RedirectToAction("UsersList");
        }

        
        public ActionResult UsersList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "USERS LIST";
            ViewBag.PanelTitles = PanelTitles;


            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["WarningMessage"] != null)
                ViewBag.WarningMessage = TempData["WarningMessage"];

            
            List<UsersListViewModel> model = new List<UsersListViewModel>();
            using (var userContext = new ApplicationDbContext())
            {
                model = (from u in userContext.Users
                         select new UsersListViewModel
                         {
                             UserName = u.UserName,
                             Email = u.Email,
                             PhoneNumber = u.PhoneNumber,
                             IsActive = u.IsActive
                         }).Take(10).ToList();
            }
            return View(model);
        }

        public ActionResult UpdateUser(string userName, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "User Management";
            Dictionary<string, string> sectionNames = new Dictionary<string, string>();
            Dictionary<string, string> subSectionNames = new Dictionary<string, string>();

            sectionNames["section1"] = "Change User Profile";
            subSectionNames["section1"] = "";

            ViewBag.PageSectionTitle = sectionNames;
            ViewBag.PageSectionSubTitle = subSectionNames;

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();

            var user=UserManager.FindByName(userName);
            UpdateUserViewModel model = new UpdateUserViewModel();

            if (user != null)
            {
                model.UserId = user.Id;
                model.EmailAddress = user.Email;
                model.Roles = (from rid in user.Roles
                               select rid.RoleId).ToList();
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser (UpdateUserViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "User Management";
            Dictionary<string, string> sectionNames = new Dictionary<string, string>();
            Dictionary<string, string> subSectionNames = new Dictionary<string, string>();

            sectionNames["section1"] = "Change User Profile";
            subSectionNames["section1"] = "";

            ViewBag.PageSectionTitle = sectionNames;
            ViewBag.PageSectionSubTitle = subSectionNames;

            if (!ModelState.IsValid)
            {
               
                TempData["ErrorMessage"]= "Please check the input"; 
                return View(model);
            }

            var user = UserManager.FindByName(model.UserName);
            if (user != null)
            {
                user.Email = model.EmailAddress;
                im.RemoveAllRoles(model.UserId);
                if(model.Roles!=null)
                {
                    foreach (var role in model.Roles)
                    {
                        im.AddUserToRole(model.UserName, role);
                    }
                }
                try
                {
                    UserManager.Update(user);
                    TempData["SuccessMessage"]= "Successfully saved";

                }
                catch
                {
                    TempData["ErrorMessage"]= "Can't update user profile";
                    return View(model);
                }
                
            }

            ModelState.Clear();
            return RedirectToAction("UpdateUser", new { userName = model.UserName, returnUrl });
        }

        [HttpGet]
        public ActionResult RoleCreate(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "ADD NEW ROLE";
            ViewBag.PanelTitles = PanelTitles;

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["WarningMessage"] != null)
                ViewBag.WarningMessage = TempData["WarningMessage"];


            Dictionary<string, string> allRoles = new Dictionary<string, string>();
            using (var userContext = new ApplicationDbContext())
            {
                var roles = userContext.Roles.ToList();
                foreach (var role in roles)
                {
                    allRoles.Add(role.Id, role.Name);
                }
            }
            ViewBag.AllRoles = allRoles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string RoleName, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                if(!im.IsRoleExists(RoleName))
                {
                    if (im.CreateRole(RoleName))
                    {
                       TempData["SuccessMessage"] = "Role successfully created";
                    }
                    else
                        TempData["ErrorMessage"] = "Can't create the role";

                }
                else
                    TempData["ErrorMessage"] = "Role name already exists";

            }
            return RedirectToAction("RoleCreate", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleDelete(string RoleId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (String.IsNullOrWhiteSpace(RoleId))
            {
                TempData["ErrorMessage"] = "Role not found";
                return RedirectToAction("RoleCreate", "Account");
            }

            if (im.RemoveRole(RoleId))
            {
                TempData["SuccessMessage"] = "Role delete successfull";
            }
            else
                TempData["ErrorMessage"] = "Cant delete the role";

            return RedirectToAction("RoleCreate","Account");
        }

       //[HttpPost]
       //[ValidateAntiForgeryToken]
       //public ActionResult UserDelete (string userName)
       // {
       //     IdentityManager im = new IdentityManager();
       //     string uid = im.GetUserIdByName(userName);
       //     im.RemoveAllRoles(uid);
       //     im.DeleteUser(uid);
       //     return RedirectToAction("UsersList", "Account");
       // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDelete(string UserID)
        {
            if(!String.IsNullOrWhiteSpace(UserID))
            {
                IdentityManager im = new IdentityManager();
                //string uid = im.GetUserIdByName(userName);
                im.RemoveAllRoles(UserID);
                im.DeleteUser(UserID);
            }

            return Redirect(Request.UrlReferrer.ToString());

        }

        [HttpGet]
        public ActionResult ChangeMyPassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "CHANGE MY PASSWORD";
            ViewBag.PanelTitles = PanelTitles;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeMyPassword(ChangeMyPasswordViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "CHANGE MY PASSWORD";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please check the inputs";
                return View(model);
            }

            IdentityManager im = new IdentityManager();

            string userId = User.Identity.GetUserId();
            //string userId = "398574f6-c390-4f76-b6a8-84109e330f46";

            if (im.ChangePassword(userId, model.OldPassword, model.NewPassword))
            {
                im.DisablePasswordChangeFlag(userId);
                ViewBag.SuccessMessage = "Password Successfully Changed";
            }
            else
                ViewBag.ErrorMessage = "Can't Change the Password";

            return View();
        }

        //public ActionResult ResetPassword(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    ViewBag.PageTitle = "User Management";
        //    Dictionary<string, string> sectionNames = new Dictionary<string, string>();
        //    Dictionary<string, string> subSectionNames = new Dictionary<string, string>();

        //    sectionNames["section1"] = "Reset Password";
        //    subSectionNames["section1"] = "";

        //    ViewBag.PageSectionTitle = sectionNames;
        //    ViewBag.PageSectionSubTitle = subSectionNames;
               
        //    return View();
        //}

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string UserName,string returnUrl)
        {
            if (String.IsNullOrWhiteSpace(UserName))
            {
                ViewBag.EMsg= "RESET PASSWORD: User Name is Required";
                return View();
            }

            IdentityManager im = new IdentityManager();
            string userId = im.GetUserIdByName(UserName);
            string defaultPassword = Helpers.KDMEnvironmentConstants.DefaultUserPassword;

            //using (KDMDB db = new KDMDB())
            //{
            //    defaultPassword = (from rw in db.tbl_config
            //                       select rw.cnfg_app_user_default_pass).FirstOrDefault();
            //}

            if (!String.IsNullOrWhiteSpace(defaultPassword))
            {
                if (im.DirectResetPassword(userId, defaultPassword))
                    ViewBag.SMsg="Reset Password Successfully";
                else
                    ViewBag.EMsg = "Can't Reset the Password";
            }
            else
            {
                ViewBag.EMsg = "Default Password Not Found";
            }

            return View();
        }

        #region Assign Roles To User
        [HttpGet]
        public ActionResult AssignRolesToUser(string returnUrl, string id = "")
        {
            //userName = "niaz";

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "ROLES ASSIGNED TO";

            ViewBag.PanelTitles = PanelTitles;

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["WarningMessage"] != null)
                ViewBag.WarningMessage = TempData["WarningMessage"];


            List<SelectListItem> allRoles = (new ViewHelper()).AllRolesToSelectListItem();
            List<string> existingRoles = im.GetUserRolesByUserName(id);

            foreach (var role in allRoles)
            {
                if (existingRoles != null)
                {
                    if (existingRoles.Contains(role.Text))
                    {
                        role.Selected = true;
                    }
                }

            }

            AssignRolesToUserViewModel model = new AssignRolesToUserViewModel()
            {
                UserName = id,
                Roles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRolesToUser(AssignRolesToUserViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = Messages.CheckInput;
                return RedirectToAction("AssignRolesToUser");
            }

            IdentityManager im = new IdentityManager();
            List<string> existingRoles = im.GetUserRolesByUserName(model.UserName);

            foreach (var role in model.Roles)
            {
                if (role != null)
                {
                    if (role.Selected)
                    {
                        if (!existingRoles.Contains(role.Text))
                        {
                            im.AddUserToRole(model.UserName, role.Text);
                        }
                    }
                    else
                    {
                        if (existingRoles.Contains(role.Text))
                        {
                            im.RemoveRoleByUserName(model.UserName, role.Text);
                        }
                    }
                }
            }

            TempData["SuccessMessage"] = Messages.UpdateSuccessfull;

            return RedirectToAction("AssignRolesToUser", new { userName = model.UserName });
        }
        #endregion

        #region Map Role Module Action
        public ActionResult MapRoleModuleAction()
        {
            ViewBag.MapList = GetMapLists();
            return View();
        }

        [NonAction]
        private List<MapRoleModuleActionsViewModel> GetMapLists()
        {
            using (KDMDB db = new KDMDB())
            {
                var list = (from rw in db.tbl_RoleActionMappings
                            select new MapRoleModuleActionsViewModel()
                            {
                                Id = rw.ID,
                                Role = rw.Role,
                                Module = rw.Area,
                                Controller = rw.Controller,
                                Action = rw.Action,
                                Status = rw.IsActive == true ? "Active" : "InActive"
                            }).ToList();

                return list;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapRoleModuleAction(MapRoleModuleActionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MapList = GetMapLists();
                ViewBag.ErrorMessage = "Please check inputs";
                return View(model);
            }

            try
            {
                using (KDMDB db = new KDMDB())
                {

                    var rw = db.tbl_RoleActionMappings.Where(x => x.Role == model.Role
                      && x.Area == model.Module
                      && x.Controller == model.Controller
                      && x.Action == model.Action).Select(x => x).FirstOrDefault();

                    if(rw!=null)
                    {
                        ViewBag.MapList = GetMapLists();
                        ViewBag.ErrorMessage = "Entry already exists";
                        return View(model);
                    }

                    tbl_RoleActionMappings tbl = new tbl_RoleActionMappings();
                    tbl.Role = model.Role;
                    tbl.Area = model.Module;
                    tbl.ControllerDisplayText= db.tbl_modules.Where(x => x.Value == model.Module).Select(x => x.Text).FirstOrDefault();
                    tbl.Controller = model.Controller;
                    tbl.ControllerDisplayText= db.tbl_controllers.Where(x => x.Value == model.Controller).Select(x => x.Text).FirstOrDefault();
                    tbl.Action = model.Action;
                    tbl.ActionDisplayText = db.tbl_actions.Where(x => x.Value == model.Action).Select(x => x.Text).FirstOrDefault();
                    tbl.IsActive = false;

                    db.tbl_RoleActionMappings.Add(tbl);

                    db.SaveChanges();

                    ViewBag.SuccessMessage = "Successfully saved";
                }
            }
            catch
            {
                ViewBag.ErrorMessage = "Can't save";
            }

            ViewBag.MapList = GetMapLists();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMapRoleModuleAction(Int64 id)
        {
            using (KDMDB db = new KDMDB())
            {
                try
                {
                    db.tbl_RoleActionMappings.Remove(db.tbl_RoleActionMappings.Find(id));
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Successfully deleted";
                    return RedirectToAction("MapRoleModuleAction");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Can't delete";
                    return RedirectToAction("MapRoleModuleAction");
                }
            }
        }

        #endregion

        [BasicAuthentication("kdmapi","kdmapi@1234",BasicRealm ="Requires User Name and Password")]
        #region Init Identity User, Role, Assign Role. Call it only first creation of new application
        [HttpGet]
        public ActionResult InitIdentity()
        {
            string messages = "";

            ApplicationUser user = new ApplicationUser();
            user.UserName = "admin";
            user.Email = "admin@tibd.com";
            user.PhoneNumber = "";
            user.IsActive = true;
            string password = "tibd@1234";
            string roleName = "ADMIN";

            // Create Default User

            var userId = im.GetUserIdByName(user.UserName);

            if (String.IsNullOrWhiteSpace(userId))
            {
                if (im.CreateUser(user, password))
                {
                    messages = "Default User Created" + Environment.NewLine;

                    // Create Default Role

                    if (!im.IsRoleExists(roleName))
                    {
                        if (im.CreateRole(roleName))
                        {
                            messages = "Default Role Created" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        messages = "Default Role Already Exists" + Environment.NewLine;
                    }
                    //\\ Create Default Role


                    // Assign Role To User

                    if (im.AddUserToRole(user.UserName, roleName))
                    {
                        messages = "Default Role Assigned" + Environment.NewLine;
                    }

                    //\\ Assign Role To User


                }
            }
            else
            {
                messages = "admin user already exists" + Environment.NewLine;
            }
            //\\ Create Default User

            return Content(messages);

        }

        #endregion





        //public ActionResult SetDefaults
    }
}