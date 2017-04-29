using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SambariEnterprises.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter Username")]
        [Display(Name="Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}