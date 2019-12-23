using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// 121. pogledo https://www.youtube.com/watch?v=mCKdMgFv8MI&t=343s
// edited with out errors
namespace EmployeeManagement
{
    public class Startup
    {

        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // CONNECT TO DataBase
            // connection string is in 'appsettings.json'
            // EmployeeDBConnection is the name of conn. String
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            // override few default password options with this lambda expression
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;

                options.SignIn.RequireConfirmedEmail = true;


                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                /*
                    MaxFailedAccessAttempts - Specifies the number of failed logon attempts allowed before the account is locked out. 
                                              The default is 5.
                    DefaultLockoutTimeSpan - Specifies the amount of the time the account should be locked. The default it 5 minutes.
                */
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                /*
                    to use asp.net core default token providers all we need to do is call
                    .AddDefaultTokenProviders() method
                */
            }).AddEntityFrameworkStores< AppDbContext>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<CustomEmailConfirmationTokenProvider
                <ApplicationUser>>("CustomEmailConfirmation");

            // life time 6 hours for all tokens who implemented in DataProtectorTokenProviderOptions class
            // The default token life time span is 1 day
            services.Configure<DataProtectionTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromHours(6));

            // Custom token provider only for Email 3 days
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromDays(3)); 

            // options ...
            // for authorization
            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role"));

                /*
                     For Edit some roll user needs to be in 'Admin' roll and has Edit Role claim
                     with claim value 'true'
                     or
                     user needs to be in Super Admin roll

                    and if this assertion ( policy => policy.RequireAssertion(context ... ) 
                    returns true you have access to Edit some roll
                */
                options.AddPolicy("EditRolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                /*
                    If you do not wont the rest of the handlers to be called, when a failure is
                    returned, set InvokeHandlersAfterFailure property to FALSE. The default is TRUE
                */
                options.InvokeHandlersAfterFailure = false;
                /*
                // ClaimType ( "Edit Role" ) comparasion is case in-sensitive
                // ClaimValue ( "true" ) comparation is case sensitive
                options.AddPolicy("EditRolePolicy",
                    policy => policy.RequireClaim("Edit Role", "true"));
                */
                options.AddPolicy("AdminRolePolicy",
                    policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler,
                    CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
