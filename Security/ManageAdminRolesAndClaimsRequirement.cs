using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    // To create a custom authorization requirement, create a class that implements 
    // the IAuthorizationRequirement interface
    public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
    {
    }
}
