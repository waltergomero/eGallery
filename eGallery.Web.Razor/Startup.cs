using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eGallery.Web.Razor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eGallery.Contracts.Services;
using eGallery.Contracts.Repositories;
using eGallery.Infrastructure.BaseClass.ApplicationProperties;
using eGallery.UnitOfWork;
using eGallery.Business;
using eGallery.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace eGallery.Web.Razor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

               //// User settings.
                //options.User.AllowedUserNameCharacters =
                //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            //Setting the Account Login page 
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings 
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here,  
                                                      // ASP.NET Core will default to /Account/Login 
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here,  
                                                        // ASP.NET Core will default to /Account/Logout 
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is  
                                                                    // not set here, ASP.NET Core  
                                                                    // will default to  
                                                                    // /Account/AccessDenied 
                options.SlidingExpiration = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DatabaseConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);



            services.AddScoped<IApplicationProperties, ApplicationProperties>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IStatusUnitOfWork, StatusUnitOfWork>();
            services.AddScoped<IStatusService, StatusManager>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<ICategoryUnitOfWork, CategoryUnitOfWork>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICommonUnitOfWork, CommonUnitOfWork>();
            services.AddScoped<IUploadUnitOfWork, UploadUnitOfWork>();
            services.AddScoped<IUploadService, UploadManager>();
            services.AddScoped<IUploadRepository, UploadRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
            CreateUserRoles(services).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //IdentityResult roleResult;
            ////Adding Admin Role 
            //var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            //if (!roleCheck)
            //{
            //    //create the roles and seed them to the database 
            //    roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            //}
            //Assign Admin role to the main User here we have given our newly registered  
            //login id for Admin management 
            IdentityUser user = await UserManager.FindByEmailAsync("walter.gomero@gmail.com");
            var User = new IdentityUser();
            await UserManager.AddToRoleAsync(user, "Admin");
        }
    }
}
