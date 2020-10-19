using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KDM.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace KDM.Helpers
{
    public class ControllerHelper
    {
        KDMDB db = new KDMDB();
        IdentityManager im = new IdentityManager();
        public static IEnumerable<Error> AllErrors(ModelStateDictionary modelState)
        {
            var result = from ms in modelState
                         where ms.Value.Errors.Any()
                         let fieldKey = ms.Key
                         let errors = ms.Value.Errors
                         from error in errors
                         select new Error(fieldKey, error.ErrorMessage);

            return result;
        }

        public UserRoute GetDefaultRoute()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var defaults = db.tbl_user_defaults.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
            if (defaults != null)
            {
                return new UserRoute()
                {
                    Area=defaults.ModuleDefault,
                    Controller=defaults.ControllDefault,
                    Action=defaults.ActionDefault
                };
            }

            return null;
        }

        public UserRoute GetDefaultRoute(string userId)
        {
            var defaults = db.tbl_user_defaults.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
            if (defaults != null)
            {
                return new UserRoute()
                {
                    Area = defaults.ModuleDefault,
                    Controller = defaults.ControllDefault,
                    Action = defaults.ActionDefault
                };
            }

            return null;
        }

        public bool AuthorizeAction(string userId, UserRoute userRoute)
        {
            bool ret = false;

            var actions = db.tbl_RoleActionMappings.Where(x => x.Area == userRoute.Area
              && x.Controller == userRoute.Controller
              && x.Action == userRoute.Action)
            .Select(x => x).ToList();

            var userRoles = im.GetUsersRole(userId);
            
            foreach (var action in actions)
            {
                if (userRoles.Contains(action.Role))
                {
                    ret = true;
                    break;
                }
            }
            
            return ret;
        }


    }
}