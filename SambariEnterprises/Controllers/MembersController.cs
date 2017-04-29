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

namespace SambariEnterprises.Controllers
{
    [BasicAuthenticationAttribute("sament", "$t@r22", BasicRealm = "Basic-Auth")]
    public class MembersController : Controller
    {
        private SambariEnterprisesEntities1 db = new SambariEnterprisesEntities1();

        // GET: Registrations
        [CustomAuthorize("SuperAdmin")]
        public ActionResult Index()
        {
            List<MemberViewModel> lstMemberViewModel = new List<MemberViewModel>();
            var memberRegistrations = db.MemberRegistrations.Where(m => m.Member.UserName == null || m.Member.UserName == string.Empty).Include(m => m.Member);

            if(memberRegistrations != null && memberRegistrations.Count() > 0)
            {
                foreach(var memberRegistration in memberRegistrations)
                {
                    var member = new MemberViewModel();
                    member.ID = memberRegistration.ID;
                    member.MemberID = memberRegistration.Member.ID;
                    member.DrugLicenceNumber = memberRegistration.DrugLicenceNumber;
                    member.PharmacyName = memberRegistration.PharmacyName;
                    member.Email = memberRegistration.Member != null ? memberRegistration.Member.Email : string.Empty;
                    member.Address = memberRegistration.Address;
                    member.GstRegistrationNumber = memberRegistration.GSTResgistrationNumber;
                    member.TinNumber = memberRegistration.TinNumber;
                    member.MobileNumber = memberRegistration.Phone;

                    lstMemberViewModel.Add(member);
               
                }
            }
            return View(lstMemberViewModel);
        }

        public ActionResult Create()
        {
            var model = new RegistrationViewModel();
            model.DrugLicenceExpiry = DateTime.Now.ToString("MM/dd/yyyy");
     
            return View(model);
        }

        [CustomAuthorize("SuperAdmin")]
        public ActionResult GenerateUserNameAndPassword(long id)
        {
            var memberViewModel = new MemberViewModel();

            var member = db.Members.Where(m => m.ID == id && m.IsActive == true).FirstOrDefault();
            memberViewModel.ID = id;
            memberViewModel.Email = member.Email;
            return View("LoginDetails", memberViewModel);
        }

        [CustomAuthorize("SuperAdmin")]
        public ActionResult RegisteredMembers()
        {
            List<MemberViewModel> lstMemberViewModel = new List<MemberViewModel>();
            var registeredMembers = db.MemberRegistrations.Where(m => m.Member.UserName != null).Include(m => m.Member);

            if (registeredMembers != null && registeredMembers.Count() > 0)
            {
                foreach (var registeredMember in registeredMembers)
                {
                    var member = new MemberViewModel();
                    member.ID = registeredMember.ID;
                    member.MemberID = registeredMember.Member.ID;
                    member.OwnerName = registeredMember.OwnerName;
                    member.DrugLicenceNumber = registeredMember.DrugLicenceNumber;
                    member.PharmacyName = registeredMember.PharmacyName;
                    member.Email = registeredMember.Member != null ? registeredMember.Member.Email : string.Empty;
                    member.Address = registeredMember.Address;
                    member.GstRegistrationNumber = registeredMember.GSTResgistrationNumber;
                    member.TinNumber = registeredMember.TinNumber;
                    member.MobileNumber = registeredMember.Phone;

                    lstMemberViewModel.Add(member);

                }
            }

            return View(lstMemberViewModel);
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
                try
                {
                    if(db.Members.Any(m => m.Email == registrationViewModel.Email && m.IsActive == true))
                    {
                        ModelState.AddModelError("Email", "Email address already exist in the system.");
                        return View("Create", registrationViewModel);
                    }

                    member.ResourceID = Guid.NewGuid(); ;
                    member.Email = registrationViewModel.Email;
                    member.Role = db.Roles.FirstOrDefault(x => x.RoleName == "User" && x.IsActive == true);
                    member.IsActive = true;
                    member.CreatedDate = DateTime.UtcNow;
                    member.CreatedBy = 1;

                    memberRegistration.ResourceID = Guid.NewGuid(); ;
                    memberRegistration.Member = member;
                    memberRegistration.PharmacyName = string.IsNullOrWhiteSpace(registrationViewModel.PharmacyName) ? string.Empty : registrationViewModel.PharmacyName;
                    memberRegistration.OwnerName = string.IsNullOrWhiteSpace(registrationViewModel.OwnerName) ? string.Empty : registrationViewModel.OwnerName;
                    memberRegistration.ContactPersonName = string.IsNullOrWhiteSpace(registrationViewModel.ContactPersonName) ? string.Empty : registrationViewModel.ContactPersonName;
                    memberRegistration.TinNumber = string.IsNullOrWhiteSpace(registrationViewModel.TinNumber) ? string.Empty : registrationViewModel.TinNumber;
                    memberRegistration.Address = string.IsNullOrWhiteSpace(registrationViewModel.Address) ? string.Empty : registrationViewModel.Address;
                    memberRegistration.DrugLicenceNumber = string.IsNullOrWhiteSpace(registrationViewModel.DrugLicenceNumber) ? string.Empty : registrationViewModel.DrugLicenceNumber;
                    memberRegistration.DrugLicenceExpiry = Convert.ToDateTime(registrationViewModel.DrugLicenceExpiry);
                    memberRegistration.PanNumber = registrationViewModel.PanNumber;
                    memberRegistration.RegisteredForGst = registrationViewModel.HasGstRegistrationNumber;
                    memberRegistration.Constitution = registrationViewModel.Constitution;
                    memberRegistration.GSTResgistrationNumber = string.IsNullOrWhiteSpace(registrationViewModel.GstRegistrationNumber)
                                                                    ? string.Empty
                                                                    : registrationViewModel.GstRegistrationNumber.Trim();
                    memberRegistration.IsActive = true;
                    memberRegistration.CreatedBy = 1;
                    memberRegistration.CreatedDate = DateTime.UtcNow;

                    db.MemberRegistrations.Add(memberRegistration);
                    db.SaveChanges();

                    EmailHelper.SendUserMail(memberRegistration.PharmacyName, member.Email);
                    EmailHelper.SendAdminMail(registrationViewModel);

                    TempData["Success"] = "You are registered successfully. You will receive an email with instruction.";
                    return Redirect("/Home/Login");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("Message", "Error occured while performing you request.");
                    return View(registrationViewModel);
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

                        db.Members.Attach(member);
                        var entry = db.Entry(member);
                        entry.Property(e => e.UserName).IsModified = true;
                        entry.Property(e => e.Password).IsModified = true;
                        entry.Property(e => e.HashCode).IsModified = true;
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
