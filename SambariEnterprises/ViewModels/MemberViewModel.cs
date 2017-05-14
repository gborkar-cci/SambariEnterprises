using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SambariEnterprises.ViewModels
{
    public class MemberViewModel
    {
        public long ID { get; set; }
        public string ImageUrl { get; set; }
        
        public long MemberID { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [Display(Name = "Customer Number")]
        public string CustomerName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string MobileNumber { get; set; }

        [Display(Name = "Drug Licence Number")]
        public string DrugLicenceNumber { get; set; }

        [Display(Name = "TIN Number")]
        public string TinNumber { get; set; }

        [Display(Name = "GST Registration Number")]
        public string GstRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Username")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Confirm Password")]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] 
        public string ConfirmPassword { get; set; }
    }
}