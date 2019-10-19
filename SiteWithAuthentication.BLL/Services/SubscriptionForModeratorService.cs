using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SiteWithAuthentication.BLL.Util;

namespace SiteWithAuthentication.BLL.Services
{
    class SubscriptionForModeratorService : ICommonService<SubscriptionForModeratorDTO>
    {
        IUnitOfWork Database { get; set; }

        public SubscriptionForModeratorService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.       
        public IEnumerable<SubscriptionForModeratorDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<SubscriptionForModerator> source = Database.SubscriptionForModerator.GetAll().ToList();
                return iMapper.Map<IEnumerable<SubscriptionForModerator>, IEnumerable<SubscriptionForModeratorDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubscriptionForModeratorDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                SubscriptionForModerator source = await Database.SubscriptionForModerator.GetAsync(id);
                return iMapper.Map<SubscriptionForModerator, SubscriptionForModeratorDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SubscriptionForModeratorDTO> Find(Func<SubscriptionForModeratorDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<SubscriptionForModerator, bool> converted = d => predicate(
                    new SubscriptionForModeratorDTO
                    {
                        SubscriptionForModeratorId = d.SubscriptionForModeratorId,
                        UserProfileId = d.UserProfileId,
                        StartDate = d.StartDate,
                        SubscriptionPeriod = d.SubscriptionPeriod,
                        CourseCount = d.CourseCount,
                        IsTrial = d.IsTrial,
                        IsApproved = d.IsApproved
                    });
                IEnumerable<SubscriptionForModerator> source = Database.SubscriptionForModerator.Find(converted).ToList();
                return iMapper.Map<IEnumerable<SubscriptionForModerator>, IEnumerable<SubscriptionForModeratorDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(SubscriptionForModeratorDTO item, string userId)
        {
            try
            {
                // Get all subscriptions of the current user.
                IEnumerable<SubscriptionForModerator> subscriptions = Database.SubscriptionForModerator.Find(obj => obj.UserProfileId == item.UserProfileId);
                // Get an active subscription.
                SubscriptionForModerator activeSubscription = (from sub in subscriptions
                                                               where sub.StartDate < DateTime.Now
                                                               && (DateTime.Now - sub.StartDate < TimeSpan.FromDays(sub.SubscriptionPeriod)
                                                               && sub.IsApproved)
                                                               select sub).FirstOrDefault();
                // Create a new subscription.
                SubscriptionForModerator subscription = new SubscriptionForModerator
                {
                    UserProfileId = item.UserProfileId,
                    CourseCount = item.CourseCount,
                    SubscriptionPeriod = item.SubscriptionPeriod,
                    StartDate = DateTime.Now,
                    IsTrial = item.IsTrial,
                    IsApproved = item.IsApproved
                };
                if (activeSubscription != null)
                {
                    if (!activeSubscription.IsTrial)
                    {
                        subscription.StartDate = DateTime.Now + TimeSpan.FromDays(activeSubscription.SubscriptionPeriod - (DateTime.Now - activeSubscription.StartDate).Days);
                    }
                }
                Database.SubscriptionForModerator.Create(subscription);
                // Add a moderator role.
                if (!BLLRepository.IsModerator(Database, item.UserProfileId))
                {
                    await Database.UserManager.AddToRoleAsync(item.UserProfileId, "moderator");
                }
                await Database.SaveAsync();
                return new OperationDetails(true, "Subscription adding completed successfully.", "SubscriptionForModerator");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "SubscriptionForModerator");
            }
        }
        public async Task<OperationDetails> UpdateAsync(SubscriptionForModeratorDTO item, string userId)
        {
            try
            {
                // I. Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't update any moderator subscription. Apply to Admin.", "SubscriptionForModerator");
                }

                // Get updatable subscription from DB.
                var subscription = await Database.SubscriptionForModerator.GetAsync(item.SubscriptionForModeratorId);
                if (subscription != null)
                {
                    // II. Deactivate the active trial subscription if it exists and the updatable subscription is approved.
                    // Get all subscriptions of the current user.
                    IEnumerable<SubscriptionForModerator> subscriptions = Database.SubscriptionForModerator.Find(obj => obj.UserProfileId == subscription.UserProfileId);
                    // Get an active subscription.
                    SubscriptionForModerator activeTrialSubscription = (from sub in subscriptions
                                                                        where sub.StartDate < DateTime.Now
                                                                        && (DateTime.Now - sub.StartDate < TimeSpan.FromDays(sub.SubscriptionPeriod)
                                                                        && sub.IsApproved
                                                                        && sub.IsTrial)
                                                                        select sub).FirstOrDefault();
                    if (activeTrialSubscription != null && item.IsApproved)
                    {
                        activeTrialSubscription.SubscriptionPeriod = 0;
                        Database.SubscriptionForModerator.Update(activeTrialSubscription);
                        await Database.SaveAsync();
                    }

                    // III. Update the subscription.
                    subscription.CourseCount = item.CourseCount;
                    subscription.SubscriptionPeriod = item.SubscriptionPeriod;
                    subscription.IsApproved = item.IsApproved;
                    Database.SubscriptionForModerator.Update(subscription);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subscription updating completed successfully.", "SubscriptionForModerator");
                }
                return new OperationDetails(false, "Subscription with this Id doesn't exists.", "SubscriptionForModerator");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "SubscriptionForModeratorId");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't delete any moderator subscription. Apply to Admin.", "SubscriptionForModerator");
                }
                var subscription = await Database.SubscriptionForModerator.GetAsync(id);
                if (subscription != null)
                {
                    // Delete the subscription.
                    await Database.SubscriptionForModerator.DeleteAsync(id);
                    // Question: 
                    //
                    //
                    // Do I have to delete "moderator"-role?
                    //
                    //
                    //string currentUserId = (await Database.SubscriptionForModerator.GetAsync(id)).UserProfileId;
                    //await Database.UserManager.RemoveFromRoleAsync(currentUserId, "moderator");
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subscription deleting completed successfully.", "SubscriptionForModerator");
                }
                return new OperationDetails(false, "Subscription with this Id doesn't exists.", "SubscriptionForModerator");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "SubscriptionForModerator");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}