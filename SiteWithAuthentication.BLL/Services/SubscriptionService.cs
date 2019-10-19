using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.Util;
using System.Linq;

namespace SiteWithAuthentication.BLL.Services
{
    class SubscriptionService : ICommonService<SubscriptionDTO>
    {
        IUnitOfWork Database { get; set; }

        public SubscriptionService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.   
        public IEnumerable<SubscriptionDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Subscription> source = Database.Subscription.GetAll().ToList();
                return iMapper.Map<IEnumerable<Subscription>, IEnumerable<SubscriptionDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubscriptionDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Subscription source = await Database.Subscription.GetAsync(id);
                return iMapper.Map<Subscription, SubscriptionDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SubscriptionDTO> Find(Func<SubscriptionDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Subscription, bool> converted = d => predicate(
                    new SubscriptionDTO
                    {
                        SubscriptionId = d.SubscriptionId,
                        UserProfileId = d.UserProfileId,
                        CourseId = d.CourseId,
                        StartDate = d.StartDate,
                        SubscriptionPeriod = d.SubscriptionPeriod,
                        IsApproved = d.IsApproved,
                        Email = d.UserProfile.ApplicationUser.Email
                    });

                IEnumerable<Subscription> sorce = Database.Subscription.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Subscription>, IEnumerable<SubscriptionDTO>>(sorce);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(SubscriptionDTO item, string userId)
        {
            try
            {
                // I. Checks.
                if (item.UserProfileId == null)
                {
                    if (item.Email == null)
                    {
                        return new OperationDetails(false, "You haven't specified any emails!", "Subscription");
                    }
                    // Checking for:  is there this user in the DB?
                    var user = await Database.UserManager.FindByEmailAsync(item.Email);
                    if (user == null)
                    {
                        return new OperationDetails(false, "User with this Email is absent in DB!", "Subscription");
                    }
                    // Assign the found value to property - UserProfileId.
                    item.UserProfileId = user.Id;
                }
                // Checking for:  has the user already been assigned on this course?
                // Get all subscriptions of the current user.
                IEnumerable<Subscription> subscriptions = Database.Subscription.Find(obj =>
                                                                                obj.UserProfileId == item.UserProfileId
                                                                                && obj.CourseId == item.CourseId
                                                                                && DateTime.Now > obj.StartDate
                                                                                && (DateTime.Now - obj.StartDate < TimeSpan.FromDays(obj.SubscriptionPeriod)));
                if (subscriptions.Count() > 1)
                {
                    return new OperationDetails(false, "The current user has more than 1 active subscription on the same course in the same period.", "Subscription");
                }

                // II. Create a new subscription.
                // Get an active subscription.
                Subscription activeSubscription = subscriptions.Where(obj => obj.IsApproved).FirstOrDefault();
                Subscription subscription = new Subscription
                {
                    UserProfileId = item.UserProfileId,
                    CourseId = item.CourseId,
                    SubscriptionPeriod = item.SubscriptionPeriod,
                    StartDate = DateTime.Now,
                    IsApproved = item.IsApproved
                };
                if (activeSubscription != null)
                {
                    subscription.StartDate = DateTime.Now + TimeSpan.FromDays(activeSubscription.SubscriptionPeriod - (DateTime.Now - activeSubscription.StartDate).Days);
                }
                Database.Subscription.Create(subscription);
                await Database.SaveAsync();
                return new OperationDetails(true, "Subscription adding completed successfully.", "Subscription");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subscription");
            }
        }
        public async Task<OperationDetails> UpdateAsync(SubscriptionDTO item, string userId)
        {
            try
            {
                // I. Checks.
                // Checking for: Does the current user have a permission from the test creator for changing subscriptions?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                bool isCourseAssigned = Database.CourseAssignment.Find(obj =>
                                                                        obj.UserProfileId == userId
                                                                        && obj.CourseId == item.CourseId
                                                                        && obj.IsApproved).Count() > 0;
                if (item.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't update this course subscription. This course has been created by the other user so apply to the course creator for the permission.", "Subscription");
                }

                // II. Update the subscription.
                // Get the updatable subscription from DB.
                Subscription subscription = await Database.Subscription.GetAsync(item.SubscriptionId);
                if (subscription != null)
                {
                    subscription.SubscriptionPeriod = item.SubscriptionPeriod;
                    subscription.IsApproved = item.IsApproved;
                    Database.Subscription.Update(subscription);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subscription updating completed successfully.", "Subscription");
                }
                return new OperationDetails(false, "Subscription with this Id doesn't exists.", "Subscription");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subscription");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // I. Checks.
                // Checking for: Does the current user have a permission from the test creator for changing subscriptions?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Get the deletable subscription from DB.
                Subscription subscription = await Database.Subscription.GetAsync(id);
                bool isCourseAssigned = Database.CourseAssignment.Find(obj =>
                                                                        obj.UserProfileId == userId
                                                                        && obj.CourseId == subscription.CourseId
                                                                        && obj.IsApproved).Count() > 0;
                Course course = await Database.Course.GetAsync(subscription.CourseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't delete this course subscription. This course has been created by the other user so apply to the course creator for the permission.", "Subscription");
                }

                // II. Delete the subscription.
                if (subscription != null)
                {
                    await Database.Subscription.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subscription deleting completed successfully.", "Subscription");
                }
                return new OperationDetails(false, "Subscription with this Id doesn't exists.", "Subscription");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subscription");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
