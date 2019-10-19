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
    class CourseAssignmentService : ICommonService<CourseAssignmentDTO>
    {
        IUnitOfWork Database { get; set; }

        public CourseAssignmentService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<CourseAssignmentDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<CourseAssignment> source = Database.CourseAssignment.GetAll().ToList();
                return iMapper.Map<IEnumerable<CourseAssignment>, IEnumerable<CourseAssignmentDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<CourseAssignmentDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                CourseAssignment source = await Database.CourseAssignment.GetAsync(id);
                return iMapper.Map<CourseAssignment, CourseAssignmentDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<CourseAssignmentDTO> Find(Func<CourseAssignmentDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Func<CourseAssignment, bool> converted = d => predicate(
                    new CourseAssignmentDTO
                    {
                        CourseAssignmentId = d.CourseAssignmentId,
                        CourseId = d.CourseId,
                        UserProfileId = d.UserProfileId,
                        IsApproved = d.IsApproved,
                        AssignmentDate = d.AssignmentDate,
                    });
                IEnumerable<CourseAssignment> source = Database.CourseAssignment.Find(converted).ToList();
                return iMapper.Map<IEnumerable<CourseAssignment>, IEnumerable<CourseAssignmentDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OperationDetails> CreateAsync(CourseAssignmentDTO item, string userId)
        {
            try
            {
                // I. Checks.
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Course course = await Database.Course.GetAsync(item.CourseId);
                if (course.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't create this course assignment. This course has been created by other user.", "Course Assignment");
                }
                if (item.Email == null)
                {
                    return new OperationDetails(false, "You haven't selected any Email!", "Course assignment");
                }
                // Checking for:  is there this user in the DB?
                var user = await Database.UserManager.FindByEmailAsync(item.Email);
                if (user == null)
                {
                    return new OperationDetails(false, "User with this Email is absent in DB!", "Course assignment");
                }
                // Assign the found value to property - UserProfileId.
                item.UserProfileId = user.Id;
                // Checking for:  has a user already been assigned on this course?
                IEnumerable<CourseAssignment> courseAssignments = Database.CourseAssignment.Find(
                    obj =>
                    obj.UserProfileId == item.UserProfileId
                    && obj.CourseId == item.CourseId);
                if (courseAssignments.Count() > 0)
                {
                    return new OperationDetails(false, "This user has already been assigned on this course!", "Course assignment");
                }

                // II. Create a new course assignment.
                CourseAssignment courseAssignment = new CourseAssignment
                {
                    UserProfileId = item.UserProfileId,
                    CourseId = item.CourseId,
                    IsApproved = item.IsApproved,
                    AssignmentDate = DateTime.Now
                };

                // Add a moderator role.
                if (!BLLRepository.IsModerator(Database, item.UserProfileId))
                {
                    await Database.UserManager.AddToRoleAsync(item.UserProfileId, "moderator");
                }

                Database.CourseAssignment.Create(courseAssignment);
                await Database.SaveAsync();
                return new OperationDetails(true, "Course adding completed successfully.", "Course Assignment");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Course Assignment");
            }
        }
        public async Task<OperationDetails> UpdateAsync(CourseAssignmentDTO item, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Course course = await Database.Course.GetAsync(item.CourseId);
                if (course.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't update this course sssignment. This course has been created by other user.", "Course Assignment");
                }
                if (item.Email == null)
                {
                    return new OperationDetails(false, "You haven't selected any Email!", "Course assignment");
                }
                // Checking for:  is there this user in the DB?
                var user = await Database.UserManager.FindByEmailAsync(item.Email);
                if (user == null)
                {
                    return new OperationDetails(false, "User with this Email is absent in DB!", "Course assignment");
                }
                // Assign the found value to property - UserProfileId.
                item.UserProfileId = user.Id;
                // Find the changing entity in DB by CourseAssignmentId.
                CourseAssignment courseAssignment = await Database.CourseAssignment.GetAsync(item.CourseAssignmentId);
                if (courseAssignment != null)
                {
                    // Checking for:  has a user already been assigned on this course?
                    IEnumerable<CourseAssignment> courseAssignments = Database.CourseAssignment.Find(
                        obj =>
                        obj.UserProfileId == item.UserProfileId
                        && obj.CourseId == item.CourseId);
                    if (courseAssignments.Count() > 1)
                    {
                        return new OperationDetails(false, "This user has already been assigned on this course!", "Course Assignment");
                    }
                    if (courseAssignments.Count() == 1 && courseAssignment.UserProfileId != item.UserProfileId)
                    {
                        return new OperationDetails(false, "This user has already been assigned on this course!", "Course Assignment");
                    }
                    // Update the course assignment.
                    courseAssignment.UserProfileId = item.UserProfileId;
                    courseAssignment.IsApproved = item.IsApproved;
                    courseAssignment.AssignmentDate = DateTime.Now;
                    Database.CourseAssignment.Update(courseAssignment);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Course assignment updating completed successfully.", "Course Assignment");
                }
                return new OperationDetails(false, "Course assignment with this Id doesn't exists.", "Course Assignment");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Course Assignment");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                int courseId = (await Database.CourseAssignment.GetAsync(id)).CourseId;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't delete this course assignment. This course has been created by other user.", "Course Assignment");
                }
                var courseAssignment = await Database.CourseAssignment.GetAsync(id);
                if (courseAssignment != null)
                {
                    try
                    {
                        await Database.CourseAssignment.DeleteAsync(id);
                        await Database.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        return new OperationDetails(false, ex.Message, "");
                    }
                    return new OperationDetails(true, "Course assignment deleting completed successfully.", "Course Assignment");
                }
                return new OperationDetails(false, "Course assignment with this Id doesn't exists. Deleting is impossible.", "Course Assignment");

            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
