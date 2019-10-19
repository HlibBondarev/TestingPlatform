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
    class TopicService : ICommonService<TopicDTO>
    {
        IUnitOfWork Database { get; set; }

        public TopicService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<TopicDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Topic> source = Database.Topic.GetAll().ToList();
                return iMapper.Map<IEnumerable<Topic>, IEnumerable<TopicDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TopicDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Topic source = await Database.Topic.GetAsync(id);
                return iMapper.Map<Topic, TopicDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<TopicDTO> Find(Func<TopicDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Topic, bool> converted = d => predicate(
                    new TopicDTO
                    {
                        TopicId = d.TopicId,
                        CourseId = d.CourseId,
                        TopicTitle = d.TopicTitle,
                        Description = d.Description,
                        TopicNumber = d.TopicNumber,
                        IsFree = d.IsFree
                    });

                IEnumerable<Topic> source = Database.Topic.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Topic>, IEnumerable<TopicDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(TopicDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have a permission from the test creator for creating topics?
                bool isCourseAssigned = Database.CourseAssignment.Find(obj =>
                                                                       obj.UserProfileId == userId
                                                                       && obj.CourseId == item.CourseId
                                                                       && obj.IsApproved).Count() > 0;
                Course course = await Database.Course.GetAsync(item.CourseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't create this topic. This course has been created by the other user so apply to the course creator for the permission.", "Topic");
                }
                // Checking for: Has the topic with the same name already existed in DB?
                IEnumerable<Topic> topics = Database.Topic.Find(obj => obj.CourseId == item.CourseId && obj.TopicTitle.Trim() == item.TopicTitle.Trim());
                if (topics.ToList().Count == 0)
                {
                    if (course.IsFree)
                    {
                        item.IsFree = true;
                    }
                    Topic topic = new Topic
                    {
                        CourseId = item.CourseId,
                        TopicTitle = item.TopicTitle.Trim(),
                        Description = item.Description?.Trim(),
                        TopicNumber = item.TopicNumber,
                        LastModifiedDateTime = DateTime.Now,
                        IsFree = item.IsFree
                    };
                    Database.Topic.Create(topic);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Topic adding completed successfully.", "Topic");
                }
                else
                {
                    return new OperationDetails(false, "Topic with the same name has already existed in DB.", "Topic");
                }
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Topic");
            }
        }
        public async Task<OperationDetails> UpdateAsync(TopicDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have a permission from the test creator for creating topics?
                bool isCourseAssigned = Database.CourseAssignment.Find(obj =>
                                                                       obj.UserProfileId == userId
                                                                       && obj.CourseId == item.CourseId
                                                                       && obj.IsApproved).Count() > 0;
                Course course = await Database.Course.GetAsync(item.CourseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't update this topic. This course has been created by the other user so apply to the course creator for the permission.", "Topic");
                }
                var topic = await Database.Topic.GetAsync(item.TopicId);
                if (topic != null)
                {
                    // Checking for: Has the topic with the same name already existed in DB?
                    IEnumerable<Topic> topics = Database.Topic.Find(obj => obj.CourseId == item.CourseId && obj.TopicTitle.Trim() == item.TopicTitle.Trim());
                    if (topics.ToList().Count > 0 && item.TopicTitle.Trim() != topic.TopicTitle.Trim())
                    {
                        return new OperationDetails(false, "Topic with the same name has already existed in DB.", "Topic");
                    }
                    if (course.IsFree)
                    {
                        item.IsFree = true;
                    }
                    // Update the topic.
                    topic.TopicTitle = item.TopicTitle.Trim();
                    topic.Description = item.Description?.Trim();
                    topic.TopicNumber = item.TopicNumber;
                    topic.LastModifiedDateTime = DateTime.Now;
                    topic.IsFree = item.IsFree;
                    Database.Topic.Update(topic);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Topic updating completed successfully.", "Topic");
                }
                return new OperationDetails(false, "Topic with this Id doesn't exists.", "Topic");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Topic");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // I. Checks.
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have a permission from the test creator for creating topics?
                Topic topic = await Database.Topic.GetAsync(id);
                bool isCourseAssigned = Database.CourseAssignment.Find(obj =>
                                                                       obj.UserProfileId == userId
                                                                       && obj.CourseId == topic.CourseId
                                                                       && obj.IsApproved).Count() > 0;
                Course course = await Database.Course.GetAsync(topic.CourseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't delete this topic. This course has been created by the other user so apply to the course creator for the permission.", "Topic");
                }

                // II. Delete the subscription.
                if (topic != null)
                {
                    if (topic.Questions.Count > 0)
                    {
                        return new OperationDetails(false, "You can't delete this topic. Before you have to delete depended questions.", "Topic");
                    }
                    await Database.Topic.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Topic deleting completed successfully.", "Topic");
                }
                return new OperationDetails(false, "Topic with this Id doesn't exists. Deleting is impossible.", "Topic");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Topic");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
