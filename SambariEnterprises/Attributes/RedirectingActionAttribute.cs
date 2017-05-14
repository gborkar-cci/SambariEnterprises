using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SambariEnterprises.Attributes
{
    public class RedirectingActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var authSession = HttpContext.Current.Session;

            if (authSession != null && authSession["UserID"] != null)
            {
                if(Convert.ToBoolean(authSession["IsTempPassword"]))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "home",
                        action = "changepassword"
                    }));
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "home",
                    action = "login"
                }));
            }
        }
    }
}