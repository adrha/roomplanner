// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.Business.Services;
using RoomPlanner.Core;
using RoomPlanner.Infrastructure.Dbo;

namespace RoomPlanner.App.Areas.Identity.Pages.Account
{
    [Authorize(Roles = UserRoles.AdministrativeRoleName)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<RoomPlannerIdentityUserDbo> _signInManager;
        private readonly UserManager<RoomPlannerIdentityUserDbo> _userManager;
        private readonly IUserStore<RoomPlannerIdentityUserDbo> _userStore;
        private readonly IUserEmailStore<RoomPlannerIdentityUserDbo> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly CustomEmailSenderService _emailSenderService;

        public RegisterModel(
            UserManager<RoomPlannerIdentityUserDbo> userManager,
            IUserStore<RoomPlannerIdentityUserDbo> userStore,
            SignInManager<RoomPlannerIdentityUserDbo> signInManager,
            ILogger<RegisterModel> logger,
            CustomEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
        }

        [BindProperty]
        public UserInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.InvitationId = Guid.NewGuid();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user);

                List<string> roles = new List<string>();
                if (Input.CleanPlanRole)
                {
                    roles.Add(UserRoles.CleaningPlanViewerRoleName);
                }

                if (Input.AdminRole)
                {
                    roles.Add(UserRoles.AdministrativeRoleName);
                }

                if (Input.BookRole)
                {
                    roles.Add(UserRoles.RoomBookingRoleName);
                }

                var roleResult = await _userManager.AddToRolesAsync(user, roles);

                if (result.Succeeded && roleResult.Succeeded)
                {
                    _logger.LogInformation("User created.");

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var userId = await _userManager.GetUserIdAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/CreateAccount",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code, invitationId = user.InvitationId },
                        protocol: Request.Scheme);

                    await _emailSenderService.SendInvitationMail(Input.Email, callbackUrl);
                    return Redirect("/Users");
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        

        private RoomPlannerIdentityUserDbo CreateUser()
        {
            try
            {
                return Activator.CreateInstance<RoomPlannerIdentityUserDbo>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<RoomPlannerIdentityUserDbo> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<RoomPlannerIdentityUserDbo>)_userStore;
        }
    }
}
