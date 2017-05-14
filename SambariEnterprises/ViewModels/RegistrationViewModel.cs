using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SambariEnterprises.ViewModels
{
    public class RegistrationViewModel
    {
        public long MemberRegistrationID { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Please Enter Customer name")]
        [Display(Name = "Name of the Customer")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please Enter Address")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter Pin Code")]
        [Display(Name = "PinCode")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pin Code must be numeric")]
        [StringLength(6, ErrorMessage = "Pin Code must contains 6 characters", MinimumLength = 6)]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Please Enter Customer Phone")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number must be numeric")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a Constitution")]
        [Display(Name = "Constitution")]
        public string Constitution { get; set; }

        [Required(ErrorMessage = "Please Enter Owner/Partner/Director name")]
        [Display(Name = "Name of Owner/Partner/Director")]
        public string OwnerName { get; set; }

        [Display(Name = "Authorised Person (If Different from above)")]
        public string AuthorizedPersonName { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [Display(Name = "Mobile Number(to receive business related updates)")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        [StringLength(10, ErrorMessage = "The Mobile must contains 10 characters", MinimumLength = 10)]
        public string MobileNumber { get; set; }


        [Display(Name = "Drug Licence Number")]
        [Required(ErrorMessage = "Please Enter Drug Licence Number")]
        public string DrugLicenceNumber1 { get; set; }
        public string DrugLicenceExpiry1 { get; set; }

        public string DrugLicenceNumber2 { get; set; }
        public string DrugLicenceExpiry2 { get; set; }

        public string DrugLicenceNumber3 { get; set; }
        public string DrugLicenceExpiry3 { get; set; }

        public string DrugLicenceNumber4 { get; set; }
        public string DrugLicenceExpiry4 { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Person name")]
        [Display(Name = "Contact Person for all GST related enquiries")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Person Mobile Number")]
        [Display(Name = "Mobile No. of Contact Person")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile Number must be numeric")]
        [StringLength(10, ErrorMessage = "The Mobile must contains 10 characters", MinimumLength = 10)]
        public string ContactPersonMobileNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Person Email")]
        [Display(Name = "Email Address of Contact Person")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        public string ContactPersonEmail { get; set; }

        [Required(ErrorMessage = "Please Enter Provisional ID Number")]
        [Display(Name = "Provisional ID Number (GSTIN)")]
        public string GstRegistrationNumber { get; set; }

        [Display(Name = "Tin Number")]
        public string TinNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Application Reference Number")]
        [Display(Name = "Application Reference Number (ARN)")]
        public string ApplicationReferenceNumber { get; set; }

        [Required(ErrorMessage = "Please upload GST acknowledgement file.")]
        [Display(Name = "GST Acknowledgement")]
        public HttpPostedFileBase GSTAcknowledgement { get; set; }
        public string GSTAcknowledgementName { get; set; }

        [Required(ErrorMessage = "Please select one of the option")]
        [Display(Name = "Are you registered for GST?")]
        public bool HasGstRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Pan Number")]
        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }

        [Required(ErrorMessage = "Please Select the checkbox")]
        public bool TermsAndCondition { get; set; }

        public string HasGstRegistrationNumberString { get; set; }
    }
}