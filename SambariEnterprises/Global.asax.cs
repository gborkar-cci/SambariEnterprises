using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SambariEnterprises
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies["SamEntCookie"];
            //var authCookie = req.Cookies["WebAccessManager"];
            if (authCookie != null && authCookie["UserID"] != null)
            {
                if (Convert.ToBoolean(authCookie["IsTempPassword"]))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "changepassword", controller = "home" })); 
                }
            }
        }

    }
}
