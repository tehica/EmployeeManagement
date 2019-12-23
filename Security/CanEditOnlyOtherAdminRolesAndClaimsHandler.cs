using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    /*
        Creating a custom authorization handler

        It is in the authorization handler that we write our logic to allow or deny access 
        to a resource like a controller action for example. To implement a handler you inherit 
        from AuthorizationHandler<T>, and implement the HandleRequirementAsync() method. 
        The generic parameter <T> on the AuthorizationHandler<T> is the type of requirement.
    */
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                           ManageAdminRolesAndClaimsRequirement requirement)
        {
            /*
                on this 'context' object we have .Resource property with return type object
                we are using this custom requirement ( HandleRequirementAsync ) to protect
                ManageUserRoles action and ManageUserClaims in AdministrationController

                .Resource property is going to return those action methods that we are protecting
                as AuthorizationFilterContext
            */
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInAdminId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
