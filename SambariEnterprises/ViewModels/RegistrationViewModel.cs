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

        [Required(ErrorMessage = "Please Enter Email Address")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Party name")]
        [Display(Name = "Party Name")]
        public string PharmacyName { get; set; }

        [Required(ErrorMessage = "Please select a Constitution")]
        [Display(Name = "Constitution")]
        public string Constitution { get; set; }

        [Required(ErrorMessage = "Please Enter Owner/Partner/Director name")]
        [Display(Name = "Name of Owner/Partner/Director")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Person name")]
        [Display(Name = "Contact Person Name")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage = "Please Enter Address")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile No")]
        [Display(Name = "Phone Number")]
        [StringLength(10, ErrorMessage = "The Mobile must contains 10 characters", MinimumLength = 10)]
        public string MobileNumber { get; set; }

        [Display(Name = "Drug Licence Number")]
        public string DrugLicenceNumber
        {
            get { return DrugLicenceNumber1 + "~" + DrugLicenceNumber2; }
        }
        public string DrugLicenceNumber1 { get; set; }
        public string DrugLicenceNumber2 { get; set; }

        [Required(ErrorMessage = "Please Enter Drug Licence Expiry")]
        [Display(Name = "Drug Licence Expiry")]
        public string DrugLicenceExpiry { get; set; }

        [Required(ErrorMessage = "Please Enter Tin Number")]
        [Display(Name = "TIN Number")]
        public string TinNumber { get; set; }

        [Required(ErrorMessage = "Please Enter GST Registration Number")]
        [Display(Name = "GST Registration Number")]
        public string GstRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please select one of the option")]
        [Display(Name = "Registered for GST?")]
        public bool HasGstRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Pan Number")]
        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }
    }
}