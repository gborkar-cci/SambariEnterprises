using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SambariEnterprises.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            ValidateCookie();
            
        }

        public ActionResult ValidateCookie()
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies["SamEntCookie"];
            //var authCookie = req.Cookies["WebAccessManager"];
            if (authCookie != null && authCookie["UserID"] != null)
            {
                if (Convert.ToBoolean(authCookie["IsTempPassword"]))
                {
                    return Redirect("home/changepassword");
                }
            }


            return Redirect("home/login");
        }
    }
}