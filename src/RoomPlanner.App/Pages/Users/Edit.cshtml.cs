using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.Core;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Interfaces;

namespace RoomPlanner.App.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger<EditModel> logger;

        [BindProperty]
        public UserInputModel Input { get; set; }

        public EditModel(IUserRepository userRepository, IMapper mapper, ILogger<EditModel> logger)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogError("No userid provided");
                return BadRequest("No userId provided");
            }

            Guid userGuid = Guid.Parse(userId);

            var user = await userRepository.GetUserByIdAsync(userGuid);
            var roles = await userRepository.GetUserRolesAsync(userGuid);

            Input = mapper.Map<UserInputModel>(user);
            Input.AdminRole = roles.Contains(UserRoles.AdministrativeRoleName);
            Input.BookRole = roles.Contains(UserRoles.RoomBookingRoleName);
            Input.CleanPlanRole = roles.Contains(UserRoles.CleaningPlanViewerRoleName);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogError("No userid provided");
                return BadRequest("No userId provided");
            }

            Guid userGuid = Guid.Parse(userId);

            if (ModelState.IsValid)
            {
                var user = mapper.Map<User>(Input);
                user.Id = userGuid;

                var roles = new List<string>();
                if (Input.AdminRole) roles.Add(UserRoles.AdministrativeRoleName);
                if (Input.BookRole) roles.Add(UserRoles.RoomBookingRoleName);
                if (Input.CleanPlanRole) roles.Add(UserRoles.CleaningPlanViewerRoleName);

                await userRepository.UpdateUserRolesAsync(roles, userGuid);

                await userRepository.UpdateUserAsync(user);

                return Redirect("/Users");
            }

            return Page();
        }
    }
}
