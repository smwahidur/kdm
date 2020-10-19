using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using KDM.Helpers;
using Microsoft.AspNet.Identity;

namespace KDM.Filters
{
    public class KDMAuthenticationFilter:ActionFilterAttribute,IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
           
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

            var user = HttpContext.Current.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}