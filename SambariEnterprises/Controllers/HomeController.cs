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
using System.Security.Cryptography;

namespace SambariEnterprises.Controllers
{
    //[BasicAuthenticationAttribute("sament", "$t@r22", BasicRealm = "Basic-Auth")]
    public class HomeController : Controller
    {
        private SambariEnterprisesEntities db = new SambariEnterprisesEntities();

        public ActionResult Login(string id)
        {
            ViewBag.RegistrationSuccess = !string.IsNullOrWhiteSpace(id) && id.ToLower() == "regsuccess" ? "regsuccess" : string.Empty;
            ViewBag.InvalidToken = !string.IsNullOrWhiteSpace(id) && id.ToLower() == "invalidusertoken" ? "invalidusertoken" : string.Empty;
            ViewBag.InvalidLogin = !string.IsNullOrWhiteSpace(id) && id.ToLower() == "invalidlogin" ? "invalidlogin" : string.Empty;
            ViewBag.ResetPasswordSuccess = !string.IsNullOrWhiteSpace(id) && id.ToLower() == "resetpasswordsuccess" ? "resetpasswordsuccess" : string.Empty; 


            return View();
        }

        public ActionResult LogOff()
        {
            Session.RemoveAll();
            return RedirectToAction("Login");              
        }

        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        public ActionResult ForgotPassword()
        {
            var forgotPasswordViewModel = new ForgotPasswordViewModel();

            return View(forgotPasswordViewModel);
        }

        public ActionResult ResetPassword(string id)
        {
            var changePasswordViewModel = new ChangePasswordViewModel();

            var member = db.Members.FirstOrDefault(x => x.UserToken == id);

            if(member == null)
            {
                //TempData["Error"] = "Invalid User token.";
                return Redirect("/home/login/invalidusertoken");
            }

            changePasswordViewModel.UserID = member.ID;
            return View(changePasswordViewModel);
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
                        Session.Add("UserRole", systemUser.Role.RoleName);
                        Session.Add("IsSuperAdmin", true);
                        Session.Add("IsTempPassword", false);
                        return Redirect("/members/index");
                    }
                    else
                    {
                        //TempData["Error"] = "Invalid login details. Please try again.";
                        return Redirect("/home/login/invalidlogin");
                    }
                }
                else
                {
                    var member = db.Members.FirstOrDefault(su => su.UserName == loginViewModel.UserName.Trim());

                    if (member != null)
                    {
                        var encodedPassword = PasswordHelper.EncodePassword(loginViewModel.Password.Trim(), member.HashCode);

                        if (member.Password.Equals(encodedPassword))
                        {
                            //HttpCookie myCookie = System.Web.HttpContext.Current.Request.Cookies["SamEntCookie"] ?? new HttpCookie("SamEntCookie");
                            //myCookie.Values["UserId"] = member.ID.ToString();
                            //myCookie.Values["UserName"] = member.UserName;
                            //myCookie.Values["UserRole"] = member.Role.RoleName;
                            //myCookie.Values["IsSuperAdmin"] = "false";
                            //myCookie.Values["IsTempPassword"] = member.IsTempPassword.ToString();
                            ////myCookie.Expires = DateTime.Now.AddDays(365);
                            //System.Web.HttpContext.Current.Response.Cookies.Add(myCookie);

                            Session.Add("UserID", member.ID);
                            Session.Add("UserName", member.UserName);
                            Session.Add("UserRole", member.Role.RoleName);
                            Session.Add("IsSuperAdmin", false);
                            Session.Add("IsTempPassword", member.IsTempPassword);
                            return Redirect("/members/index");
                        }
                        else
                        {
                            //TempData["Error"] = "Invalid login details. Please try again.";
                            return Redirect("/home/login/invalidlogin");
                        }
                    }
                    else
                    {
                        //TempData["Error"] = "Invalid login details. Please try again.";
                        return Redirect("/home/login/invalidlogin");
                    }
                }
            }

            return View();

        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (changePasswordViewModel != null)
            {
                var userID = Convert.ToInt64(Session["UserID"]);
                var member = db.Members.FirstOrDefault(m => m.ID == userID);

                var hashCode = PasswordHelper.GeneratePassword(10);
                var password = PasswordHelper.EncodePassword(changePasswordViewModel.NewPassword, hashCode);

                member.Password = password;
                member.HashCode = hashCode;
                member.IsTempPassword = false;

                db.Members.Attach(member);
                var entry = db.Entry(member);
                entry.Property(e => e.Password).IsModified = true;
                entry.Property(e => e.HashCode).IsModified = true;
                entry.Property(e => e.IsTempPassword).IsModified = true;
                db.SaveChanges();

                Session["IsTempPassword"] = false;

                TempData["Success"] = "You have successfully changed your password.";
                return Redirect("/members/index");
            }

            return View(changePasswordViewModel);

        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if(forgotPasswordViewModel != null)
            {
                var member = db.Members.FirstOrDefault(x => x.UserName == forgotPasswordViewModel.UserName);
                if (member == null)
                {
                    ModelState.AddModelError("username", "Username entered does not exists.");
                    return View(forgotPasswordViewModel);
                }

                member.UserToken = PasswordHelper.GeneratePassword(10); ;

                //if (isTokenNull)
                //    db.Members.Add(member);
                //else
                //{
                    db.Members.Attach(member);
                    var entry = db.Entry(member);
                    entry.Property(e => e.UserToken).IsModified = true;
                //}

                db.SaveChanges();

                EmailHelper.SendResetPasswordMail(member.Email, forgotPasswordViewModel);

                //TempData["Success"] = "Reset password successful. Email has been sent for further instructions.";
                return Redirect("/home/login/resetpasswordsuccess");
            }

            return View(forgotPasswordViewModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (changePasswordViewModel != null)
            {
                var userID = changePasswordViewModel.UserID;
                var member = db.Members.FirstOrDefault(m => m.ID == userID);

                var hashCode = PasswordHelper.GeneratePassword(10);
                var password = PasswordHelper.EncodePassword(changePasswordViewModel.NewPassword, hashCode);

                member.Password = password;
                member.HashCode = hashCode;
                member.UserToken = string.Empty;

                db.Members.Attach(member);
                var entry = db.Entry(member);
                entry.Property(e => e.Password).IsModified = true;
                entry.Property(e => e.HashCode).IsModified = true;
                entry.Property(e => e.UserToken).IsModified = true;
                db.SaveChanges();

                TempData["Success"] = "You have successfully changed your password.";
                return Redirect("/members/index");
            }

            return View(changePasswordViewModel);
        }

        #endregion
    }
}