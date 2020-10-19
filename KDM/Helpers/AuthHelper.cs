using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;

namespace KDM.Helpers
{
    public class ApplicationUser : IdentityUser
    {
        ////[Required]
        //public string FirstName { get; set; }
        ////[Required]
        //public string LastName { get; set; }
        ////[Required]
        //public string UserEmail { get; set; }

        //public Int64 UserDetailsId { get; set; }

        public bool IsActive { get; set; }
        public bool ChangePassword { get; set; } = false;
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {

        }
        public static ApplicationDbContext Create()
        
        {
            return new ApplicationDbContext();
        }
    }
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {

        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var store = new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>());
            var manager = new ApplicationUserManager(store);
            return manager;
        }
    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {

        }
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
    public class IdentityManager
    {
        UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        RoleManager<IdentityRole> rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        ApplicationDbContext dbContext = new ApplicationDbContext();
        // --------------------- Role -----------------------

        public string GetUserIdByName(string userName)
        {
            ApplicationUser user = um.FindByName(userName);
            if (user != null)
            {
                return user.Id;
            }

            return String.Empty;
        }

        public string GetUserNameById(string id)
        {
            ApplicationUser user = um.FindById(id);
            if (user != null)
            {
                return user.UserName;
            }

            return String.Empty;
        }

        public bool IsChangePassword(string id)
        {
            ApplicationUser user = um.FindById(id);
            if (user != null)
            {
                return user.ChangePassword;
            }

            return false;
        }

        public bool DisablePasswordChangeFlag(string id)
        {
            using(var db=new KDMDB())
            {
                var user = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                if (user != null)
                    user.ChangePassword = false;
                db.SaveChanges();
                return true;
            }

           
        }
        public bool CreateRole(string name)
        {
            var result = rm.Create(new IdentityRole(name));
            return result.Succeeded;
        }
        public List<string> GetUsersRole(string userId)
        {
            if(String.IsNullOrWhiteSpace(userId))
            {
                return new List<string>();
            }

            IdentityManager im = new IdentityManager();
            string userName=im.GetUserNameById(userId);
            if(String.IsNullOrWhiteSpace(userName))
                return new List<string>();

            return um.GetRoles(userId).ToList<string>();
        }

        public List<string> GetUserRolesByUserName(string userName)
        {
            string userId = GetUserIdByName(userName);
            if (String.IsNullOrWhiteSpace(userId))
                return null;
            return um.GetRoles(userId).ToList<string>();
        }

        public List<IdentityRole> GetAllRoles()
        {
            return rm.Roles.OrderBy(x => x.Name).ToList();
        }
        public bool IsRoleExists(string name)
        {
            return rm.RoleExists(name);
        }
        public void RemoveAllRoles(string userId)
        {
            var user = um.FindById(userId);
            string[] allRoles = um.GetRoles(userId).ToArray();
            um.RemoveFromRoles(userId, allRoles);
        }

        public void RemoveRole(string userId, string role)
        {
            var user = um.FindById(userId);
            um.RemoveFromRole(userId, role);
        }

        public bool RemoveRoleByUserName(string userName, string role)
        {
            var user = um.FindByName(userName);
            IdentityResult result= um.RemoveFromRole(user.Id, role);

            return result.Succeeded;
        }

        public bool RemoveRole(string id)
        {
            var role = rm.FindById(id);
            var result = rm.Delete(role);
            return result.Succeeded;
        }
        // ------------------- Role End ------------------------

        // ------------------- User -------------------------
        public bool CreateUser(ApplicationUser user, string password)
        {
            var result = um.Create(user, password);

            return result.Succeeded;
        }
        public bool DeleteUser(string userId)
        {
            var user = um.FindById(userId);
            var result = um.Delete(user);
            return result.Succeeded;
        }
        public void UdateUser(string userId, ApplicationUser user)
        {
            var selectedUser = um.FindById(userId);
            //modify field here
            dbContext.SaveChanges();
        }

        public bool AddUserToRole(string userName, string role)
        {
            var user = um.FindByName(userName);
            var idResult = um.AddToRole(user.Id, role);
            return idResult.Succeeded;
        }

        public bool AddUserIdRoles(string userId, string[] roles)
        {
            IdentityResult result = um.AddToRoles(userId, roles);
            return result.Succeeded;
        }

        public bool ChangePassword(string userId, string oldPassword, string newPassword)
        {
            bool ret = false;
            try
            {
                IdentityResult result = um.ChangePassword(userId, oldPassword, newPassword);
                return result.Succeeded;
            }
            catch
            {

            }

            return ret;
        }

        public bool DirectResetPassword(string userId, string newPassword)
        {
            IdentityResult result = null;
            try
            {
                if(um.FindById(userId)==null)
                {
                    return false;
                }
                result = um.RemovePassword(userId);
                if (result.Succeeded)
                {
                    result = um.AddPassword(userId, newPassword);
                }
            }
            catch { }
            return result.Succeeded;
        }

        public bool ActivateUser(string userName)
        {
            bool ret = false;
            var user = um.FindByName(userName);
            if(user!=null)
            {
                user.IsActive = true;
                ret = true;
                um.Update(user);
            }

            return ret;
        }

        public bool DeactivateUser(string userName)
        {
            bool ret = false;
            var user = um.FindByName(userName);
            if (user != null)
            {
                user.IsActive = false;
                ret = true;
                um.Update(user);
            }

            return ret;
        }
        
    }
}