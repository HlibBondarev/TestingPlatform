using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;

namespace SiteWithAuthentication.BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        // Authentication method interface.
        Task<ClaimsIdentity> AuthenticateAsync(UserProfileDTO userProfileDTO);
        // Initialization method interface.
        Task SetInitialDataAsync(UserProfileDTO adminProfileDto, List<string> userProfileDTO);
        // Role methods.
        string FindUserRoleById(string id);
        Task AddUserRole(string id, string roleName);

        // Search methods interface.
        IEnumerable<UserProfileDTO> GetAll();
        Task<UserProfileDTO> GetAsync(string id);
        string FindUserIdByEmail(string userEmail);
        IEnumerable<UserProfileDTO> Find(Func<UserProfileDTO, Boolean> predicate);
        
        // CRUD methods interface.
        Task<OperationDetails> CreateAsync(UserProfileDTO userProfileDTO);
        Task<OperationDetails> UpdateAsync(UserProfileDTO userProfileDTO);
        Task<OperationDetails> DeleteAsync(int id);
    }
}
