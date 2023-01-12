using RoomPlanner.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IList<string>> GetUserRolesAsync(Guid userId);

        Task<IList<User>> GetAllUsersAsync();

        Task<User> GetUserByIdAsync(Guid userId);

        Task<bool> HasUserRoleAsync(Guid userId, string roleName);

        Task<bool> HasUserRoleAsync(string userName, string roleName);

        Task<User> GetUserByInvitationIdAsync(Guid invitationId);

        Task SetEmailConfirmedAsync(string userId);

        Task<IList<User>> GetUsersFromRoleAsync(string roleName);

        Task<User> GetUserByUsernameAsync(string userName);

        Task UpdateUserAsync(User user);

        Task UpdateUserRolesAsync(IList<string> roles, Guid userId);
    }
}
