using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using RoomPlanner.Business.Services;
using RoomPlanner.Core;
using RoomPlanner.Core.Interfaces;
using RoomPlanner.Infrastructure.Adapters;
using RoomPlanner.Infrastructure.Context;
using RoomPlanner.Infrastructure.Dbo;
using RoomPlanner.Infrastructure.Repositories;
using RoomPlanner.Options;
using System.Globalization;
using System.Reflection;

namespace RoomPlanner.App
{
    public class Startup
    {
        public const string AdminUserMail = "admin@admin.com";
        public const string AdminUserInitialPwd = "RoomPlannerAdmin#1234";

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var smtpServerOptions = new SmtpServerOptions();
            Configuration.Bind(SmtpServerOptions.Position, smtpServerOptions);
            services.AddSingleton(smtpServerOptions);

            ConfigureDatabase(services);

            services.AddIdentity<RoomPlannerIdentityUserDbo, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            // Store the key for encrypting Cookies and CSRF-Tokens in database
            // This leads to the warning "No XML encryptor configured. Key {...} may be persisted to storage in unencrypted form."
            // which can be ignored, because when someone gets access to the DB we are fucked up anyways.
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-3.1
            services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>();

            services.AddAutoMapper(new Assembly[] {
            Assembly.Load("RoomPlanner.App"),
            Assembly.Load("RoomPlanner.Infrastructure"),
            Assembly.Load("RoomPlanner.Core")});


            services.AddScoped<IEmailAdapter, SmtpEmailAdapter>();
            services.AddScoped<IEmailSender, SmtpEmailAdapter>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoomReservationRepository, RoomReservationRepository>();

            services.AddScoped<RoomReservationService>();
            services.AddScoped<CustomEmailSenderService>();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            // Configure the HTTP request pipeline.
            if (Environment.IsDevelopment())
            {
                //app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var supportedCultures = new[]
            {
             new CultureInfo("en-CH"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-CH"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                CreateRoleIfInexistentAsync(roleManager, UserRoles.AdministrativeRoleName).Wait();
                CreateRoleIfInexistentAsync(roleManager, UserRoles.CleaningPlanViewerRoleName).Wait();
                CreateRoleIfInexistentAsync(roleManager, UserRoles.RoomBookingRoleName).Wait();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RoomPlannerIdentityUserDbo>>();
                SeedAdminUser(userManager, AdminUserMail, AdminUserInitialPwd).Wait();
            }
        }

        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            // Add services to the container.
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(10, 7));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, serverVersion)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                );
        }

        private async Task CreateRoleIfInexistentAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            //Adding Role
            var roleCheck = await roleManager.RoleExistsAsync(role);
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedAdminUser(UserManager<RoomPlannerIdentityUserDbo> userManager, string mail, string pwd)
        {
            if(userManager.FindByEmailAsync(AdminUserMail).Result == null)
            {
                RoomPlannerIdentityUserDbo user = new RoomPlannerIdentityUserDbo
                {
                    UserName = mail,
                    Email = mail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };

                IdentityResult result = await userManager.CreateAsync(user, pwd);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRoles.AdministrativeRoleName);
                    await userManager.AddToRoleAsync(user, UserRoles.RoomBookingRoleName);
                    await userManager.AddToRoleAsync(user, UserRoles.CleaningPlanViewerRoleName);
                }
            }
        }
    }
}
