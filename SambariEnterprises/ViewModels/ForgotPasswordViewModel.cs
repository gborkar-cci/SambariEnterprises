using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SambariEnterprises.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please Enter Username")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public string UserToken { get; set; }
        public string ImageUrl { get; set; }
    }
}