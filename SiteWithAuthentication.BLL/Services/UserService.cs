using Microsoft.AspNet.Identity;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SiteWithAuthentication.BLL.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Authentication method.
        public async Task<ClaimsIdentity> AuthenticateAsync(UserProfileDTO userProfileDTO)
        {
            ClaimsIdentity claim = null;
            // Find User by Email.
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userProfileDTO.Email);
            if (user != null)
            {
                // Check out User by UserName and Password.
                user = await Database.UserManager.FindAsync(user.UserName, userProfileDTO.Password);
            }
            // Authorize User and return ClaimsIdentity object.
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        // Initialization method. 
        public async Task SetInitialDataAsync(UserProfileDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await CreateAsync(adminDto);
        }

        // Role methods.
        public string FindUserRoleById(string id)
        {
            ApplicationUser currentUser = Database.UserManager.FindById(id);
            string role = string.Empty;
            string roleId = string.Empty;
            if (currentUser != null)
            {
                List<string> roles = new List<string>();
                foreach (var item in currentUser.Roles)
                {
                    roleId = item.RoleId;
                    role = Database.RoleManager.FindById(roleId).Name;
                    if (role != null) role = role.ToLower();
                    roles.Add(role);
                }
                if (roles.Contains("admin")) return "admin";
                if (roles.Contains("moderator")) return "moderator";
                if (roles.Contains("user")) return "user";
                if (roles.Contains(null)) return null;
            }
            return null;
        }
        public async Task AddUserRole(string id, string roleName)
        {
            if (id != null && roleName != null)
            {
                await Database.UserManager.AddToRoleAsync(id, roleName);
            }
            throw new Exception("Parameteres id or roleName equil null");
        }

        // Search methods.
        public IEnumerable<UserProfileDTO> GetAll()
        {
            // AutoMapper Setup.
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<UserProfile> source = iMapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserProfile>>(Database.UserManager.Users).ToList();
                return iMapper.Map<IEnumerable<UserProfile>, IEnumerable<UserProfileDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserProfileDTO> GetAsync(string id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                UserProfile source = iMapper.Map<ApplicationUser, UserProfile>(await Database.UserManager.FindByIdAsync(id));
                return iMapper.Map<UserProfile, UserProfileDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string FindUserIdByEmail(string userEmail)
        {
            return Database.UserManager.FindByEmail(userEmail).Id;
        }

        // This method is not implemented!!!
        public IEnumerable<UserProfileDTO> Find(Func<UserProfileDTO, bool> predicate)
        {
            throw new NotImplementedException();
            // Thise code throws an exception!!!
            //try
            //{
            //    // AutoMapper Setup.
            //    var iMapper = BLLAutoMapper.GetMapper;
            //    Func<UserProfile, bool> converted = d => predicate(
            //        new UserProfileDTO
            //        {
            //            Id = d.UserProfileId,
            //            Email = d.ApplicationUser.Email,
            //            UserName = d.ApplicationUser.UserName,
            //            Address = d.Address,
            //        });

            //    // Thise code throws an exception!!!
            //    var source = Database.UserProfile.Find(converted).ToList();                         //Exception!!!
            //    return iMapper.Map<IEnumerable<UserProfile>, IEnumerable<UserProfileDTO>>(source);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(UserProfileDTO userProfileDTO)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userProfileDTO.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userProfileDTO.Email, UserName = userProfileDTO.UserName };
                var result = await Database.UserManager.CreateAsync(user, userProfileDTO.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "Method - UserService.CreateAsync()");
                // Add role.
                await Database.UserManager.AddToRoleAsync(user.Id, userProfileDTO.Role);
                // Create user profile.
                UserProfile userProfile = new UserProfile { UserProfileId = user.Id, Address = userProfileDTO.Address };
                Database.UserProfile.Create(userProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Registration completed successfully.", "");
            }
            else
            {
                return new OperationDetails(false, "User with this email already exists.", "Email");
            }
        }
        // Unimplemented methods.
        public Task<OperationDetails> UpdateAsync(UserProfileDTO userProfileDTO)
        {
            throw new NotImplementedException();
        }
        public Task<OperationDetails> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
