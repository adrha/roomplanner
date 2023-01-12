using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Core.Interfaces;
using RoomPlanner.Infrastructure.Context;
using RoomPlanner.Infrastructure.Dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<RoomPlannerIdentityUserDbo> userManager;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UserRepository(UserManager<RoomPlannerIdentityUserDbo> userManager, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId.ToString());

            // Detach user from change-tracker. TODO: Implement proper change-tracking
            context.Entry(user).State = EntityState.Detached;

            return await userManager.GetRolesAsync(user);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return mapper.Map<User>(user);
        }

        public async Task<bool> HasUserRoleAsync(Guid userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return await HasRole(user, roleName);
        }

        public async Task<bool> HasUserRoleAsync(string userName, string roleName)
        {
            var user = await userManager.FindByNameAsync(userName);
            return await HasRole(user, roleName);
        }

        private async Task<bool> HasRole(RoomPlannerIdentityUserDbo user, string roleName)
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles.Contains(roleName);
        }

        public async Task<IList<User>> GetAllUsersAsync()
        {
            var users = await context.Users.ToListAsync();           
            return mapper.Map<List<User>>(users);
        }

        public async Task<User> GetUserByInvitationIdAsync(Guid invitationId)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.InvitationId == invitationId);
            return mapper.Map<User>(user);
        }

        public async Task SetEmailConfirmedAsync(string userId)
        {
            var user = await GetDbUserByIdAsync(userId);

            user.EmailConfirmed = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        private async Task<RoomPlannerIdentityUserDbo> GetDbUserByIdAsync(string userId)
        {
            var user = await userManager.Users.AsTracking().SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException($"User with id {userId} not found");

            return user;
        }

        public async Task<IList<User>> GetUsersFromRoleAsync(string roleName)
        {
            var users = await userManager.GetUsersInRoleAsync(roleName);
            return mapper.Map<List<User>>(users);
        }

        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpper());
            return mapper.Map<User>(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var dbUser = await GetDbUserByIdAsync(user.Id.ToString());
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.PhoneNumber = user.PhoneNumber;
            
            if(dbUser.Email != user.Email)
            {
                dbUser.Email = user.Email;
                dbUser.NormalizedEmail = user.Email.ToUpper();
                await userManager.SetUserNameAsync(dbUser, user.Email);
                await userManager.UpdateNormalizedUserNameAsync(dbUser);
            }
            
            await userManager.UpdateAsync(dbUser);
        }

        public async Task UpdateUserRolesAsync(IList<string> roles, Guid userId)
        {
            var currentRoles = await GetUserRolesAsync(userId);

            var rolesToRemove = currentRoles.Where(cr => !roles.Contains(cr)).ToList();
            var rolesToAdd = roles.Where(r => !currentRoles.Contains(r)).ToList();

            var user = await GetDbUserByIdAsync(userId.ToString());

            if (rolesToAdd.Any())
            {
                await userManager.AddToRolesAsync(user, rolesToAdd);
            }

            if (rolesToRemove.Any())
            {
                await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            // Detach user from change-tracker. TODO: Implement proper change-tracking
            context.Entry(user).State = EntityState.Detached; 
        }
    }
}
