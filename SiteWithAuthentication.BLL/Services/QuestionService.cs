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
    class QuestionService : ICommonService<QuestionDTO>
    {
        IUnitOfWork Database { get; set; }

        public QuestionService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<QuestionDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Question> source = Database.Question.GetAll().ToList();
                return iMapper.Map<IEnumerable<Question>, IEnumerable<QuestionDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<QuestionDTO> GetAsync(int id)
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                Question source = await Database.Question.GetAsync(id);
                return iMapper.Map<Question, QuestionDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<QuestionDTO> Find(Func<QuestionDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Question, bool> converted = d => predicate(
                    new QuestionDTO
                    {
                        QuestionId = d.QuestionId,
                        TopicId = d.TopicId,
                        AnswerTypeId = d.AnswerTypeId,
                        QuestionText = d.QuestionText,
                        ResourceRef = d.ResourceRef,
                        QuestionWeight = d.QuestionWeight
                    });
                IEnumerable<Question> source = Database.Question.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Question>, IEnumerable<QuestionDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(QuestionDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for creating questions from the test creator?
                int courseId = (await Database.Topic.GetAsync(item.TopicId)).CourseId;
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't create this question. This course has been created by other user so apply to the course creator for the permission.", "Question");
                }
                // Create the new question.
                Question question = new Question
                {
                    TopicId = item.TopicId,
                    AnswerTypeId = item.AnswerTypeId,
                    QuestionText = item.QuestionText?.Trim(),
                    ResourceRef = item.ResourceRef,
                    QuestionWeight = item.QuestionWeight,
                    LastModifiedDateTime = DateTime.Now
                };
                Database.Question.Create(question);
                await Database.SaveAsync();
                return new OperationDetails(true, "Question adding completed successfully.", "Question");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Question");
            }
        }
        public async Task<OperationDetails> UpdateAsync(QuestionDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                int courseId = (await Database.Topic.GetAsync(item.TopicId)).CourseId;
                // Checking for: Does the current user have permission for updating questions from the test creator?
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't update this question. This course has been created by other user so apply to the course creator for the permission.", "Question");
                }
                Question question = await Database.Question.GetAsync(item.QuestionId);
                if (question != null)
                {
                    // Update the question.
                    question.AnswerTypeId = item.AnswerTypeId;
                    question.QuestionText = item.QuestionText?.Trim();
                    question.ResourceRef = item.ResourceRef;
                    question.QuestionWeight = item.QuestionWeight;
                    question.LastModifiedDateTime = DateTime.Now;
                    Database.Question.Update(question);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Question updating completed successfully.", "Question");
                }
                return new OperationDetails(false, "Question with this Id doesn't exists.", "Question");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Question");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for deleting questions from the test creator?
                int courseId = (await Database.Question.GetAsync(id)).Topic.CourseId;
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't delete this question. This question has been created by other user so apply to the course creator for the permission.", "Question");
                }
                Question question = await Database.Question.GetAsync(id);
                if (question != null)
                {
                    if (question.Answers.Count > 0)
                    {
                        return new OperationDetails(false, "You can't delete this question. Before you have to delete depended answers.", "Question");
                    }
                    await Database.Question.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Question deleting completed successfully.", "Question");
                }
                return new OperationDetails(false, "Question with this Id doesn't exists. Deleting is impossible.", "Question");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Question");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
