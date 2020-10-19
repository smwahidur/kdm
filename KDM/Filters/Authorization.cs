using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace KDM.Filters
{
    public class AuthorizationAttribute:IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {

        }
    }
}