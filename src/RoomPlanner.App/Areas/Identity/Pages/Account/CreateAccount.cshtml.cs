using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RoomPlanner.Business.Services;
using RoomPlanner.Core.Interfaces;
using RoomPlanner.Infrastructure.Dbo;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoomPlanner.App.Areas.Identity.Pages.Account
{
    public class CreateAccountModel : PageModel
    {
        private readonly SignInManager<RoomPlannerIdentityUserDbo> _signInManager;
        private readonly UserManager<RoomPlannerIdentityUserDbo> _userManager;
        private readonly IUserStore<RoomPlannerIdentityUserDbo> _userStore;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateAccountModel> _logger;
        private readonly CustomEmailSenderService _emailSenderService;

        public CreateAccountModel(
            UserManager<RoomPlannerIdentityUserDbo> userManager,
            IUserStore<RoomPlannerIdentityUserDbo> userStore,
            IUserRepository userRepository,
            SignInManager<RoomPlannerIdentityUserDbo> signInManager,
            ILogger<CreateAccountModel> logger,
            CustomEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _userRepository = userRepository;
            _signInManager = signInManager;
            _logger = logger;
            _emailSenderService = emailSenderService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Name { get; private set; }

        public string Lastname { get; private set; }

        public string Email { get; private set; }   

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(Guid? invitationId)
        {
            if(invitationId == null)
            {
                return BadRequest($"No invitation id set");
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.InvitationId == invitationId.Value);

            if (user == null)
            {
                return NotFound($"Invitation id [{invitationId.Value}] not found");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            await _userRepository.SetEmailConfirmedAsync(user.Id);
            if (result.Succeeded)
            {
                return RedirectToPage("./CreateAccountSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        public async Task<IActionResult> OnGetAsync(Guid? invitationId = null, string code = null)
        {
            if(invitationId== null)
            {
                return BadRequest("An invitationId must be supplied");
            }

            if(code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }

                var user = await _userRepository.GetUserByInvitationIdAsync(invitationId.Value);
                Name = user.FirstName;
                Lastname = user.LastName;
                Email = user.Email;

                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };

            return Page();
        }
    }
}
