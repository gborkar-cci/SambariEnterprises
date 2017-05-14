using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SambariEnterprises.Models;
using SambariEnterprises.ViewModels;
using SambariEnterprises.Helpers;
using SambariEnterprises.Attributes;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Configuration;

namespace SambariEnterprises.Controllers
{
    //[BasicAuthenticationAttribute("sament", "$t@r22", BasicRealm = "Basic-Auth")]
    public class MembersController : Controller
    {
        private SambariEnterprisesEntities db = new SambariEnterprisesEntities();

        // GET: Registrations
        [CustomAuthorize("SuperAdmin", "User")]
        [RedirectingActionAttribute]
        public ActionResult Index()
        {
            if(Convert.ToBoolean(Session["IsSuperAdmin"]))
            {
                List<MemberViewModel> lstMemberViewModel = new List<MemberViewModel>();
                var memberRegistrations = db.MemberRegistrations.Where(m => m.Member.UserName == null || m.Member.UserName == string.Empty).Include(m => m.Member);

                if (memberRegistrations != null && memberRegistrations.Count() > 0)
                {
                    foreach (var memberRegistration in memberRegistrations)
                    {
                        var member = new MemberViewModel();
                        member.ID = memberRegistration.ID;
                        member.MemberID = memberRegistration.Member.ID;
                        member.CustomerName = memberRegistration.CustomerName;
                        member.Email = memberRegistration.Member != null ? memberRegistration.Member.Email : string.Empty;
                        member.GstRegistrationNumber = memberRegistration.GSTResgistrationNumber;

                        lstMemberViewModel.Add(member);

                    }
                }
                return View(lstMemberViewModel);
            }
            else
            {
                return Redirect("/User/Dashboard");
            }
            
        }

        public ActionResult Create()
        {
            var model = new RegistrationViewModel();
            model.DrugLicenceExpiry1 = DateTime.Now.ToString("dd/MM/yyyy");
            //model.DrugLicenceExpiry2 = DateTime.Now.ToString("dd/MM/yyyy");
            //model.DrugLicenceExpiry3 = DateTime.Now.ToString("dd/MM/yyyy");
            //model.DrugLicenceExpiry4 = DateTime.Now.ToString("dd/MM/yyyy");

            return View(model);
        }

        [CustomAuthorize("SuperAdmin")]
        [RedirectingActionAttribute]
        public ActionResult GenerateUserNameAndPassword(long id)
        {
            var memberViewModel = new MemberViewModel();

            var member = db.Members.Where(m => m.ID == id && m.IsActive == true).FirstOrDefault();
            memberViewModel.ID = id;
            memberViewModel.Email = member.Email;
            memberViewModel.CustomerName = member.MemberRegistrations != null ? member.MemberRegistrations.FirstOrDefault().CustomerName : "Customer";
            return View("LoginDetails", memberViewModel);
        }

        [CustomAuthorize("SuperAdmin")]
        [RedirectingActionAttribute]
        public ActionResult RegisteredMembers()
        {
            List<MemberViewModel> lstMemberViewModel = new List<MemberViewModel>();
            var registeredMembers = db.MemberRegistrations.Where(m => m.Member.UserName != null && m.IsActive == true).Include(m => m.Member);

            if (registeredMembers != null && registeredMembers.Count() > 0)
            {
                foreach (var registeredMember in registeredMembers)
                {
                    var member = new MemberViewModel();
                    member.ID = registeredMember.ID;
                    member.MemberID = registeredMember.Member.ID;
                    member.OwnerName = registeredMember.OwnerName;
                    member.CustomerName = registeredMember.CustomerName;
                    member.Email = registeredMember.Member != null ? registeredMember.Member.Email : string.Empty;
                    member.Address = registeredMember.Address;
                    member.GstRegistrationNumber = registeredMember.GSTResgistrationNumber;
                    member.TinNumber = registeredMember.TinNumber;
                    //member.MobileNumber = registeredMember.Phone;

                    lstMemberViewModel.Add(member);

                }
            }

            return View(lstMemberViewModel);
        }

        [CustomAuthorize("SuperAdmin")]
        [RedirectingActionAttribute]
        public ActionResult Edit(long id)
        {
            var model = new RegistrationViewModel();
            var registeredMember = db.MemberRegistrations.Where(m => m.ID == id).Include(m => m.Member).FirstOrDefault();

            if (registeredMember != null)
            {
                model.MemberRegistrationID = registeredMember.ID;
                model.CustomerName = registeredMember.CustomerName;
                model.Email = registeredMember.Member.Email;
                model.OwnerName = registeredMember.OwnerName;
                model.ContactPersonName = registeredMember.ContactPersonName;
                //model.MobileNumber = registeredMember.Phone;
                model.PanNumber = registeredMember.PanNumber;
                model.TinNumber = registeredMember.TinNumber;
                model.GstRegistrationNumber = registeredMember.GSTResgistrationNumber;
                model.DrugLicenceNumber1 = registeredMember.DrugLicenceNumber1.ToString();
                model.DrugLicenceNumber2 = registeredMember.DrugLicenceNumber2.ToString();
                model.DrugLicenceNumber3 = registeredMember.DrugLicenceNumber3.ToString();
                model.DrugLicenceNumber4 = registeredMember.DrugLicenceNumber4.ToString();
                model.DrugLicenceExpiry1 = registeredMember.DrugLicenceExpiry1.ToString();
                model.DrugLicenceExpiry2 = registeredMember.DrugLicenceExpiry2.ToString();
                model.DrugLicenceExpiry3 = registeredMember.DrugLicenceExpiry3.ToString();
                model.DrugLicenceExpiry4 = registeredMember.DrugLicenceExpiry4.ToString();
                model.Constitution = registeredMember.Constitution;
                model.Address = registeredMember.Address;
            }

            return View("MemberEdit", model);
        }

        [CustomAuthorize("SuperAdmin")]
        [RedirectingActionAttribute]
        public ActionResult Delete(long id)
        {
            var registeredMember = db.MemberRegistrations.Where(m => m.ID == id).Include(m => m.Member).FirstOrDefault();

            if (registeredMember != null)
            {
                registeredMember.IsActive = false;
                registeredMember.Member.IsActive = false;

                db.MemberRegistrations.Attach(registeredMember);
                var entry = db.Entry(registeredMember);
                entry.Property(e => e.IsActive).IsModified = true;
                db.SaveChanges();
            }

            TempData["Success"] = "Succefully deleted";
            return RedirectToAction("RegisteredMembers");
        }

        #region POST

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(RegistrationViewModel registrationViewModel)
        {
            var member = new Member();
            var memberRegistration = new MemberRegistration();

            if(registrationViewModel != null)
            {
                var log = "Start of upload -----";

                try
                {
                    //if(db.members.any(m => m.email == registrationviewmodel.email && m.isactive == true))
                    //{
                    //    modelstate.addmodelerror("email", "email address already exist in the system.");
                    //    return view("create", registrationviewmodel);
                    //}

                    registrationViewModel.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"];

                    member.ResourceID = Guid.NewGuid(); ;
                    member.Email = registrationViewModel.Email;
                    member.Role = db.Roles.FirstOrDefault(x => x.RoleName == "User" && x.IsActive == true);
                    member.IsActive = true;
                    member.CreatedDate = DateTime.UtcNow;
                    member.CreatedBy = 1;

                    memberRegistration.ResourceID = Guid.NewGuid(); ;
                    memberRegistration.Member = member;
                    memberRegistration.CustomerName = string.IsNullOrWhiteSpace(registrationViewModel.CustomerName) ? string.Empty : registrationViewModel.CustomerName;
                    memberRegistration.Address = string.IsNullOrWhiteSpace(registrationViewModel.Address) ? string.Empty : registrationViewModel.Address;
                    memberRegistration.PinCode = string.IsNullOrWhiteSpace(registrationViewModel.PinCode) ? 000000 : Convert.ToInt32(registrationViewModel.PinCode);
                    memberRegistration.CustomerPhone = string.IsNullOrWhiteSpace(registrationViewModel.CustomerPhone) ? string.Empty : registrationViewModel.CustomerPhone;
                    memberRegistration.Constitution = registrationViewModel.Constitution;
                    memberRegistration.OwnerName = string.IsNullOrWhiteSpace(registrationViewModel.OwnerName) ? string.Empty : registrationViewModel.OwnerName;
                    memberRegistration.AuthorizedPersonName = registrationViewModel.AuthorizedPersonName;
                    memberRegistration.MobileNumber = registrationViewModel.MobileNumber;
                    memberRegistration.DrugLicenceNumber1 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber1) ? string.Empty : registrationViewModel.DrugLicenceNumber1;
                    memberRegistration.DrugLicenceNumber2 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber2) ? string.Empty : registrationViewModel.DrugLicenceNumber2;
                    memberRegistration.DrugLicenceNumber3 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber3) ? string.Empty : registrationViewModel.DrugLicenceNumber3;
                    memberRegistration.DrugLicenceNumber4 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber4) ? string.Empty : registrationViewModel.DrugLicenceNumber4;
                    memberRegistration.DrugLicenceExpiry1 = DateTime.ParseExact(registrationViewModel.DrugLicenceExpiry1, "dd/MM/yyyy", null);
                    memberRegistration.DrugLicenceExpiry2 = !string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceExpiry2) ? DateTime.ParseExact(registrationViewModel.DrugLicenceExpiry2, "dd/MM/yyyy", null) : DateTime.Now;
                    memberRegistration.DrugLicenceExpiry3 = !string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceExpiry3) ? DateTime.ParseExact(registrationViewModel.DrugLicenceExpiry3, "dd/MM/yyyy", null) : DateTime.Now;
                    memberRegistration.DrugLicenceExpiry4 = !string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceExpiry4) ? DateTime.ParseExact(registrationViewModel.DrugLicenceExpiry4, "dd/MM/yyyy", null) : DateTime.Now;

                    memberRegistration.ContactPersonName = string.IsNullOrWhiteSpace(registrationViewModel.ContactPersonName) ? string.Empty : registrationViewModel.ContactPersonName;
                    memberRegistration.ContactPersonMobile = string.IsNullOrWhiteSpace(registrationViewModel.ContactPersonMobileNumber) ? string.Empty : registrationViewModel.ContactPersonMobileNumber;
                    memberRegistration.ContactPersonEmail = string.IsNullOrWhiteSpace(registrationViewModel.ContactPersonEmail) ? string.Empty : registrationViewModel.ContactPersonEmail;
                    memberRegistration.TinNumber = string.IsNullOrWhiteSpace(registrationViewModel.TinNumber) ? string.Empty : registrationViewModel.TinNumber;
                    memberRegistration.ApplicationReferenceNumber = string.IsNullOrWhiteSpace(registrationViewModel.ApplicationReferenceNumber) ? string.Empty : registrationViewModel.ApplicationReferenceNumber;
                    memberRegistration.PanNumber = registrationViewModel.PanNumber;
                    memberRegistration.RegisteredForGst = registrationViewModel.HasGstRegistrationNumber;
                    registrationViewModel.HasGstRegistrationNumberString = registrationViewModel.HasGstRegistrationNumber ? "Yes" : "No";


                    memberRegistration.GSTResgistrationNumber = string.IsNullOrWhiteSpace(registrationViewModel.GstRegistrationNumber)
                                                                    ? string.Empty
                                                                    : registrationViewModel.GstRegistrationNumber.Trim();
                    //memberRegistration.Phone = registrationViewModel.MobileNumber;
                    memberRegistration.IsActive = true;
                    memberRegistration.CreatedBy = 1;
                    memberRegistration.CreatedDate = DateTime.UtcNow;

                    if (registrationViewModel.GSTAcknowledgement != null && registrationViewModel.GSTAcknowledgement.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(registrationViewModel.GSTAcknowledgement.FileName);
                        string _path = Path.Combine(Server.MapPath("~/GST"));
                        log += _path + "-----";

                        if (!Directory.Exists(_path))
                        {
                            Directory.CreateDirectory(_path);

                            DirectoryInfo directory = new DirectoryInfo(_path);
                            DirectorySecurity security = directory.GetAccessControl();

                            security.AddAccessRule(new FileSystemAccessRule(@"everyone",
                                                    FileSystemRights.Modify,
                                                    AccessControlType.Deny));

                            directory.SetAccessControl(security);
                        }

                        var extension = Path.GetExtension(_FileName);
                        var newName = DateTime.Now.ToString("yyyyMMddHHmmdd") + extension;

                        _path = Path.Combine(Server.MapPath("~/GSTAcknowledgement/"), newName);
                        log += "Saving uploaded file -----";
                        registrationViewModel.GSTAcknowledgement.SaveAs(_path);
                        memberRegistration.GSTAcknowledgement = newName;
                        registrationViewModel.GSTAcknowledgementName = newName;
                        log += "FIles uploaded";
                    }

                    db.MemberRegistrations.Add(memberRegistration);
                    db.SaveChanges();

                    EmailHelper.SendUserMail(registrationViewModel);
                    EmailHelper.SendAdminMail(registrationViewModel);

                    TempData["Success"] = "You are registered successfully. You will receive an email with instruction.";
                    return Redirect("/Home/Login");
                }
                catch(Exception ex)
                {
                    log += "In exception ------";
                    log += ex.Message;
                    log += ex.InnerException.Message;

                    ViewBag.exception = log;
                    ModelState.AddModelError("Message", "Error occured while performing you request.");
                    return View(registrationViewModel);
                }
            }

            return null;
        }

        [HttpPost]
        [CustomAuthorize("SuperAdmin")]
        public ActionResult Edit(RegistrationViewModel registrationViewModel)
        {
            var member = new Member();
            var memberRegistration = new MemberRegistration();

            if (registrationViewModel != null)
            {
                try
                {
                    memberRegistration = db.MemberRegistrations.Where(mr => mr.ID == registrationViewModel.MemberRegistrationID).FirstOrDefault();
                    member = memberRegistration.Member;

                    if (db.Members.Any(m => m.Email == registrationViewModel.Email && m.IsActive == true && m.ID != member.ID))
                    {
                        ModelState.AddModelError("Email", "Email address already exist in the system.");
                        return View("MemberEdit", registrationViewModel);
                    }             
                    
                    //if(string.IsNullOrWhiteSpace(registrationViewModel.Password))
                    //{
                    //    var hashCode = PasswordHelper.GeneratePassword(10);
                    //    var password = PasswordHelper.EncodePassword(registrationViewModel.Password, hashCode);

                    //    member.Password = password;
                    //    member.HashCode = hashCode;

                    //    EmailHelper.SendPasswordChangeMail(registrationViewModel.Email, registrationViewModel.Email);
                    //}

                    member.Email = registrationViewModel.Email;
                    member.IsActive = true;
                    member.UpdatedDate = DateTime.UtcNow;
                    member.UpdatedBy = 1;

                    memberRegistration.Member = member;
                    //memberRegistration.PharmacyName = string.IsNullOrWhiteSpace(registrationViewModel.CustomerName) ? string.Empty : registrationViewModel.CustomerName;
                    memberRegistration.OwnerName = string.IsNullOrWhiteSpace(registrationViewModel.OwnerName) ? string.Empty : registrationViewModel.OwnerName;
                    memberRegistration.ContactPersonName = string.IsNullOrWhiteSpace(registrationViewModel.ContactPersonName) ? string.Empty : registrationViewModel.ContactPersonName;
                    memberRegistration.TinNumber = string.IsNullOrWhiteSpace(registrationViewModel.TinNumber) ? string.Empty : registrationViewModel.TinNumber;
                    memberRegistration.Address = string.IsNullOrWhiteSpace(registrationViewModel.Address) ? string.Empty : registrationViewModel.Address;
                    memberRegistration.DrugLicenceNumber1 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber1) ? string.Empty : registrationViewModel.DrugLicenceNumber1;
                    memberRegistration.DrugLicenceNumber2 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber2) ? string.Empty : registrationViewModel.DrugLicenceNumber2;
                    memberRegistration.DrugLicenceNumber3 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber3) ? string.Empty : registrationViewModel.DrugLicenceNumber3;
                    memberRegistration.DrugLicenceNumber4 = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber4) ? string.Empty : registrationViewModel.DrugLicenceNumber4;
                    memberRegistration.DrugLicenceExpiry1 = Convert.ToDateTime(registrationViewModel.DrugLicenceExpiry1);
                    memberRegistration.DrugLicenceExpiry2 = Convert.ToDateTime(registrationViewModel.DrugLicenceExpiry2);
                    memberRegistration.DrugLicenceExpiry3 = Convert.ToDateTime(registrationViewModel.DrugLicenceExpiry3);
                    memberRegistration.DrugLicenceExpiry4 = Convert.ToDateTime(registrationViewModel.DrugLicenceExpiry4);
                    memberRegistration.PanNumber = registrationViewModel.PanNumber;
                    memberRegistration.RegisteredForGst = registrationViewModel.HasGstRegistrationNumber;
                    memberRegistration.Constitution = registrationViewModel.Constitution;
                    memberRegistration.GSTResgistrationNumber = string.IsNullOrWhiteSpace(registrationViewModel.GstRegistrationNumber)
                                                                    ? string.Empty
                                                                    : registrationViewModel.GstRegistrationNumber.Trim();
                    //memberRegistration.Phone = registrationViewModel.MobileNumber;
                    memberRegistration.IsActive = true;
                    memberRegistration.UpdatedBy = 1;
                    memberRegistration.UpdatedDate = DateTime.UtcNow;

                    db.MemberRegistrations.Attach(memberRegistration);
                    var entry = db.Entry(memberRegistration);
                    entry.Property(e => e.CustomerName).IsModified = true;
                    entry.Property(e => e.OwnerName).IsModified = true;
                    entry.Property(e => e.ContactPersonName).IsModified = true;
                    entry.Property(e => e.TinNumber).IsModified = true;
                    entry.Property(e => e.Address).IsModified = true;
                    entry.Property(e => e.DrugLicenceExpiry1).IsModified = true;
                    entry.Property(e => e.DrugLicenceExpiry2).IsModified = true;
                    entry.Property(e => e.DrugLicenceExpiry3).IsModified = true;
                    entry.Property(e => e.DrugLicenceExpiry4).IsModified = true;
                    entry.Property(e => e.DrugLicenceNumber1).IsModified = true;
                    entry.Property(e => e.DrugLicenceNumber2).IsModified = true;
                    entry.Property(e => e.DrugLicenceNumber3).IsModified = true;
                    entry.Property(e => e.DrugLicenceNumber4).IsModified = true;
                    entry.Property(e => e.PanNumber).IsModified = true;
                    entry.Property(e => e.RegisteredForGst).IsModified = true;
                    entry.Property(e => e.Constitution).IsModified = true;
                    entry.Property(e => e.GSTResgistrationNumber).IsModified = true;
                    //entry.Property(e => e.Phone).IsModified = true;
                    entry.Property(e => e.IsActive).IsModified = true;
                    entry.Property(e => e.UpdatedBy).IsModified = true;
                    entry.Property(e => e.UpdatedDate).IsModified = true;
                    db.SaveChanges();

                    //EmailHelper.SendUserMail(memberRegistration.PharmacyName, member.Email);
                    //EmailHelper.SendAdminMail(registrationViewModel);

                    TempData["Success"] = "Succefully updated";
                    return Redirect("/Members/RegisteredMembers");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Message", "Error occured while performing you request.");
                    return View("MemberEdit", registrationViewModel);
                }
            }

            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateUserNameAndPassword(MemberViewModel memberViewModel)
        {
            if(memberViewModel != null)
            {
                try
                {
                    memberViewModel.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"];
                    if (db.Members.Any(m => m.UserName == memberViewModel.Username.Trim()))
                    {
                        ModelState.AddModelError("Username", "Username has been already assigned to other member.");
                        return View("LoginDetails", memberViewModel);
                    }

                    var member = db.Members.FirstOrDefault(m => m.ID == memberViewModel.ID);

                    if (member != null)
                    {
                        memberViewModel.Email = member.Email;
                        var hashCode = PasswordHelper.GeneratePassword(10);
                        var password = PasswordHelper.EncodePassword(memberViewModel.Password, hashCode);

                        member.UserName = memberViewModel.Username;
                        member.Password = password;
                        member.HashCode = hashCode;
                        member.IsTempPassword = true;

                        db.Members.Attach(member);
                        var entry = db.Entry(member);
                        entry.Property(e => e.UserName).IsModified = true;
                        entry.Property(e => e.Password).IsModified = true;
                        entry.Property(e => e.HashCode).IsModified = true;
                        entry.Property(e => e.IsTempPassword).IsModified = true;
                        db.SaveChanges();

                        EmailHelper.SendUserDetailsMail(memberViewModel);

                        TempData["Success"] = "Username and password successfully generated.";
                        return Redirect("/Members/Index");
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("Message", "Error occured while performing you request.");
                    return View("LoginDetails", memberViewModel);
                }
            }
            return null;
        }

        #endregion POST

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
