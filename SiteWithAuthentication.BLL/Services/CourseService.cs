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
    class CourseService : ICommonService<CourseDTO>
    {
        IUnitOfWork Database { get; set; }

        public CourseService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<CourseDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Course> source = Database.Course.GetAll().ToList();
                return iMapper.Map<IEnumerable<Course>, IEnumerable<CourseDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<CourseDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Course source = await Database.Course.GetAsync(id);
                return iMapper.Map<Course, CourseDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<CourseDTO> Find(Func<CourseDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Course, bool> converted = d => predicate(
                    new CourseDTO
                    {
                        CourseId = d.CourseId,
                        SpecialityId = d.SpecialityId,
                        UserProfileId = d.UserProfileId,
                        CourseTitle = d.CourseTitle,
                        Description = d.Description,
                        CourseTestQuestionsNumber = d.CourseTestQuestionsNumber,
                        TopicTestQuestionsNumber = d.TopicTestQuestionsNumber,
                        TimeToAnswerOneQuestion = d.TimeToAnswerOneQuestion,
                        AttemptsNumber = d.AttemptsNumber,
                        PassingScore = d.PassingScore,
                        IsApproved = d.IsApproved,
                        IsFree = d.IsFree
                    });

                IEnumerable<Course> source = Database.Course.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Course>, IEnumerable<CourseDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(CourseDTO item, string userId)
        {
            try
            {
                // Checking for:  is the course speciality approved?
                if (!(await Database.Speciality.GetAsync(item.SpecialityId)).IsApproved)
                {
                    return new OperationDetails(false, "The specialty of the course being created is not approved.", "Speciality");
                }
                // Checking for: does the course with the same name already exist in DB?
                IEnumerable<Course> courses = Database.Course.Find(obj => obj.SpecialityId == item.SpecialityId && obj.CourseTitle.Trim() == item.CourseTitle.Trim());
                if (courses.Count() == 0)
                {
                    Course course = new Course
                    {
                        UserProfileId = item.UserProfileId,
                        SpecialityId = item.SpecialityId,
                        CourseTitle = item.CourseTitle.Trim(),
                        Description = item.Description?.Trim(),
                        CourseTestQuestionsNumber = item.CourseTestQuestionsNumber,
                        TopicTestQuestionsNumber = item.TopicTestQuestionsNumber,
                        TimeToAnswerOneQuestion = item.TimeToAnswerOneQuestion,
                        AttemptsNumber = item.AttemptsNumber,
                        PassingScore = item.PassingScore,
                        LastModifiedDateTime = DateTime.Now,
                        IsApproved = item.IsApproved,
                        IsFree = item.IsFree
                    };
                    Database.Course.Create(course);
                    await Database.SaveAsync();
                    // Create the first assignment on the new course for the course creator. 
                    CourseAssignment courseAssignment = new CourseAssignment()
                    {
                        CourseId = this.Find(c => c.CourseTitle == item.CourseTitle.Trim() && c.SpecialityId == item.SpecialityId).FirstOrDefault().CourseId,
                        UserProfileId = item.UserProfileId,
                        AssignmentDate = DateTime.Now,
                        IsApproved = true
                    };
                    Database.CourseAssignment.Create(courseAssignment);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Course adding completed successfully.", "Course");
                }
                else
                {
                    return new OperationDetails(false, "Course with the same name has already existed in DB.", "Course");
                }
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Course");
            }
        }
        public async Task<OperationDetails> UpdateAsync(CourseDTO item, string userId)
        {
            try
            {
                // Checking for:  is the course speciality approved?
                if (!(await Database.Speciality.GetAsync(item.SpecialityId)).IsApproved)
                {
                    return new OperationDetails(false, "The specialty of the course being created is not approved.", "Speciality");
                }
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Course course = await Database.Course.GetAsync(item.CourseId);
                if (course.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't update this subject. It has been created by the other user.", "Subject");
                }
                if (course != null)
                {
                    // Checking for: does the course with the same name already exist in DB?
                    IEnumerable<Course> courses = Database.Course.Find(obj => obj.SpecialityId == item.SpecialityId && obj.CourseTitle.Trim() == item.CourseTitle.Trim());
                    if (courses.Count() > 0 && item.CourseTitle.Trim() != course.CourseTitle.Trim())
                    {
                        return new OperationDetails(false, "Course with the same name has already existed in DB.", "Course");
                    }
                    course.CourseTitle = item.CourseTitle.Trim();
                    course.Description = item.Description?.Trim();
                    course.CourseTestQuestionsNumber = item.CourseTestQuestionsNumber;
                    course.TopicTestQuestionsNumber = item.TopicTestQuestionsNumber;
                    course.TimeToAnswerOneQuestion = item.TimeToAnswerOneQuestion;
                    course.AttemptsNumber = item.AttemptsNumber;
                    course.PassingScore = item.PassingScore;
                    course.LastModifiedDateTime = DateTime.Now;
                    course.IsApproved = item.IsApproved;
                    course.IsFree = item.IsFree;
                    Database.Course.Update(course);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Course updating completed successfully.", "Course");
                }
                return new OperationDetails(false, "Course with this Id doesn't exists.", "Course");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Course");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Course course = await Database.Course.GetAsync(id);
                if (course.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't delete this course. It has been created by the other user.", "Course");
                }
                if (course != null)
                {
                    if (course.CourseAssignments.Count > 0
                        || course.Subscriptions.Count > 0
                        || course.Topics.Count > 0)
                    {
                        return new OperationDetails(false, "You can't delete this course. Before you have to delete depended course assignments, sibscriments and topics.", "Course");
                    }
                    await Database.Course.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Course deleting completed successfully.", "Course");
                }
                return new OperationDetails(false, "Course with this Id doesn't exists. Deleting is impossible.", "Course");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Course");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
