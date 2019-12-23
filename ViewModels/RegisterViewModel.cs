using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        /*
            we use remote attribute on the email property on RegisterViewModel class
            thats because for this registerview the model is this RegisterViewModel
            and this email field is bound to this Email property, thats the reason we use
            Remote attribut on the Email property
        */
        // call action method IsEmailInUSe from AccountController
        [Remote(action: "IsEmailInUSe", controller: "Account")]
        [ValidEmailDomainAttribute(allowedDomain: "gmail.com", 
                ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // [DataType(DataType.Password)] - hides password when we type it
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        // comapre password in ConfirmPassword field with password in 'Password' field
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
    }
}
