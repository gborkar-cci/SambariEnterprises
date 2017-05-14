using SambariEnterprises.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SambariEnterprises.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private SambariEnterprisesEntities context = new SambariEnterprisesEntities();
        private readonly string[] allowedroles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                var userID = Convert.ToInt64(httpContext.Session["UserID"]);
                var user = context.SystemUsers.Where(m => m.ID == userID && m.Role.RoleName == role && m.IsActive == true); // checking active users with allowed roles.  
                if (user.Count() > 0)
                {
                    authorize = true; /* return true if Entity has current user(active) with specific role */
                }
                else
                {
                    var member = context.Members.Where(m => m.ID == userID && m.Role.RoleName == role && m.IsActive == true);

                    if (member.Count() > 0)
                    {
                        authorize = true; /* return true if Entity has current user(active) with specific role */
                    }
                }
            }
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult();
            filterContext.Result = new RedirectToRouteResult(
                                  new RouteValueDictionary
                                  {
                                       { "action", "Login" },
                                       { "controller", "Home" }
                                  });
        }
    }
}