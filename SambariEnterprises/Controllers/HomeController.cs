using SambariEnterprises.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SambariEnterprises.Models;
using System.Data.Entity;
using SambariEnterprises.Helpers;
using SambariEnterprises.Attributes;

namespace SambariEnterprises.Controllers
{
    [BasicAuthenticationAttribute("sament", "$t@r22", BasicRealm = "Basic-Auth")]
    public class HomeController : Controller
    {
        private SambariEnterprisesEntities1 db = new SambariEnterprisesEntities1();

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            Session.RemoveAll();
            return RedirectToAction("Login");              
        }

        #region POST

        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if(loginViewModel !=null)
            {
                var systemUser = db.SystemUsers.FirstOrDefault(su => su.UserName == loginViewModel.UserName.Trim());

                if(systemUser != null)
                {
                    var encodedPassword = PasswordHelper.EncodePassword(loginViewModel.Password.Trim(), systemUser.HashCode);

                    if (systemUser.Password.Equals(encodedPassword))
                    {
                        Session.Add("UserID", systemUser.ID);
                        Session.Add("UserName", systemUser.UserName);
                        return Redirect("/members/index");
                    }
                }
            }

            return View();

        }
        #endregion
    }
}