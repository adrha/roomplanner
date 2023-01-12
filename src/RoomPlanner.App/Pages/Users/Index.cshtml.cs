using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.ViewModels;
using RoomPlanner.Core;
using RoomPlanner.Core.Interfaces;

namespace RoomPlanner.App.Pages.Users
{
    [Authorize(Roles = UserRoles.AdministrativeRoleName)]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public IList<UserViewModel> Users { get; private set; }

        public IList<Guid> AdminUsers { get; private set; }

        public IList<Guid> CleaningUsers { get; set; }

        public IList<Guid> BookingUsers { get; set; }

        public IndexModel(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task OnGetAsync()
        {
            var users = await userRepository.GetAllUsersAsync();
            Users = mapper.Map<List<UserViewModel>>(users);

            AdminUsers = await GetUidsOfUsersAsync(UserRoles.AdministrativeRoleName);
            CleaningUsers = await GetUidsOfUsersAsync(UserRoles.CleaningPlanViewerRoleName);
            BookingUsers = await GetUidsOfUsersAsync(UserRoles.RoomBookingRoleName);
        }

        private async Task<List<Guid>> GetUidsOfUsersAsync(string roleName)
        {
            var users = await userRepository.GetUsersFromRoleAsync(roleName);
            return users.Select(u => u.Id).ToList();
        }
    }
}
