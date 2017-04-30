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
            var registeredMembers = db.MemberRegistrations.Where(m => m.Member.UserName != null && m.IsActive == true).Include(m => m.Member);

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

        [CustomAuthorize("SuperAdmin")]
        public ActionResult Edit(long id)
        {
            var model = new RegistrationViewModel();
            var registeredMember = db.MemberRegistrations.Where(m => m.ID == id).Include(m => m.Member).FirstOrDefault();

            if (registeredMember != null)
            {
                model.MemberRegistrationID = registeredMember.ID;
                model.PharmacyName = registeredMember.PharmacyName;
                model.Email = registeredMember.Member.Email;
                model.OwnerName = registeredMember.OwnerName;
                model.ContactPersonName = registeredMember.ContactPersonName;
                model.MobileNumber = registeredMember.Phone;
                model.PanNumber = registeredMember.PanNumber;
                model.TinNumber = registeredMember.TinNumber;
                model.GstRegistrationNumber = registeredMember.GSTResgistrationNumber;
                model.DrugLicenceExpiry = registeredMember.DrugLicenceExpiry.ToString();

                var drugLicenseNumber = registeredMember.DrugLicenceNumber.Split('~');
                model.DrugLicenceNumber1 = drugLicenseNumber[0];

                if(drugLicenseNumber.Count() > 1)
                    model.DrugLicenceNumber2 = string.IsNullOrWhiteSpace(drugLicenseNumber[1]) ? string.Empty : drugLicenseNumber[1];
                model.Constitution = registeredMember.Constitution;
                model.Address = registeredMember.Address;
            }

            return View("MemberEdit", model);
        }

        [CustomAuthorize("SuperAdmin")]
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
                    memberRegistration.Phone = registrationViewModel.MobileNumber;
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
                     
                    member.Email = registrationViewModel.Email;
                    member.IsActive = true;
                    member.UpdatedDate = DateTime.UtcNow;
                    member.UpdatedBy = 1;

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
                    memberRegistration.Phone = registrationViewModel.MobileNumber;
                    memberRegistration.IsActive = true;
                    memberRegistration.UpdatedBy = 1;
                    memberRegistration.UpdatedDate = DateTime.UtcNow;

                    db.MemberRegistrations.Attach(memberRegistration);
                    var entry = db.Entry(memberRegistration);
                    entry.Property(e => e.PharmacyName).IsModified = true;
                    entry.Property(e => e.OwnerName).IsModified = true;
                    entry.Property(e => e.ContactPersonName).IsModified = true;
                    entry.Property(e => e.TinNumber).IsModified = true;
                    entry.Property(e => e.Address).IsModified = true;
                    entry.Property(e => e.DrugLicenceExpiry).IsModified = true;
                    entry.Property(e => e.DrugLicenceNumber).IsModified = true;
                    entry.Property(e => e.PanNumber).IsModified = true;
                    entry.Property(e => e.RegisteredForGst).IsModified = true;
                    entry.Property(e => e.Constitution).IsModified = true;
                    entry.Property(e => e.GSTResgistrationNumber).IsModified = true;
                    entry.Property(e => e.Phone).IsModified = true;
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
