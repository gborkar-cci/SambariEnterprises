using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SambariEnterprises.ViewModels
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Please Enter Old Password")]
        [Display(Name = "Enter Old Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please Enter New Password")]
        [Display(Name = "Enter New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please Confirm your Password")]
        [Display(Name = "Confirm New Password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}