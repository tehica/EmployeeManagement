using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    // this is an exiample how we can extend identityUser class with some properties
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
