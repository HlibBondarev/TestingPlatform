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
    class AnswerService : ICommonService<AnswerDTO>
    {
        IUnitOfWork Database { get; set; }

        public AnswerService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<AnswerDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Answer> source = Database.Answer.GetAll().ToList();
                return iMapper.Map<IEnumerable<Answer>, IEnumerable<AnswerDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<AnswerDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Answer source = await Database.Answer.GetAsync(id);
                return iMapper.Map<Answer, AnswerDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<AnswerDTO> Find(Func<AnswerDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Answer, bool> converted = d => predicate(
                    new AnswerDTO
                    {
                        AnswerId = d.AnswerId,
                        QuestionId = d.QuestionId,
                        AnswerText = d.AnswerText,
                        IsProper = d.IsProper
                    });
                IEnumerable<Answer> source = Database.Answer.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Answer>, IEnumerable<AnswerDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(AnswerDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for creating answers from the test creator?
                int topicId = (await Database.Question.GetAsync(item.QuestionId)).TopicId;
                int courseId = (await Database.Topic.GetAsync(topicId)).CourseId;
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't create this answer. This course has been created by other user so apply to the course creator for the permission.", "Answer");
                }
                // Check the added answer.
                string answerType = (await Database.Question.GetAsync(item.QuestionId)).AnswerType.AnswerTypeDescription;
                var answers = Database.Answer.Find(obj => obj.QuestionId == item.QuestionId);
                int countOfExistedProperAnswersInQuestion = (from row in answers
                                                             where row.IsProper == true
                                                             select row).Count();
                if (!BLLRepository.CheckCountOfProperAnswers(answerType, item.IsProper,
                                                             countOfExistedProperAnswersInQuestion,
                                                             out string result))
                {
                    return new OperationDetails(false, result, "Answer");
                }
                // Create a new answer. 
                Answer answer = new Answer
                {
                    QuestionId = item.QuestionId,
                    AnswerText = item.AnswerText?.Trim(),
                    IsProper = item.IsProper,
                    LastModifiedDateTime = DateTime.Now
                };
                Database.Answer.Create(answer);
                await Database.SaveAsync();
                return new OperationDetails(true, "Answer adding completed successfully.", "Answer");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Answer");
            }
        }
        public async Task<OperationDetails> UpdateAsync(AnswerDTO item, string userId)
        {
            try
            {
                // Checking for: Does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for updating answers from the test creator?
                int courseId = (await Database.Question.GetAsync(item.QuestionId)).Topic.CourseId;
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't update this answer. This course has been created by other user so apply to the course creator for the permission.", "Answer");
                }
                // Get the updated answer from DB by AnswerId.
                Answer answer = await Database.Answer.GetAsync(item.AnswerId);
                if (answer != null)
                {
                    // Check the updated answer.
                    string answerType = (await Database.Question.GetAsync(item.QuestionId)).AnswerType.AnswerTypeDescription;
                    var answers = Database.Answer.Find(obj => obj.QuestionId == item.QuestionId);
                    int countOfExistedProperAnswersInQuestion = (from row in answers
                                                                 where row.IsProper == true
                                                                 select row).Count();
                    if (answer.IsProper)
                    {
                        countOfExistedProperAnswersInQuestion -= countOfExistedProperAnswersInQuestion;
                    }
                    if (!BLLRepository.CheckCountOfProperAnswers(answerType, item.IsProper,
                                                                 countOfExistedProperAnswersInQuestion,
                                                                 out string result))
                    {
                        return new OperationDetails(false, result, "Answer");
                    }
                    // Update the answer.
                    answer.AnswerText = item.AnswerText?.Trim();
                    answer.IsProper = item.IsProper;
                    answer.LastModifiedDateTime = DateTime.Now;
                    Database.Answer.Update(answer);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Answer updating completed successfully.", "Answer");
                }
                return new OperationDetails(false, "Answer with this Id doesn't exists.", "Answer");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Answer");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for updating questions from the test creator?
                int courseId = (await Database.Answer.GetAsync(id)).Question.Topic.CourseId;
                IEnumerable<CourseAssignment> assignments = Database.CourseAssignment.Find(obj => obj.CourseId == courseId);
                bool isCourseAssigned = (from assign in assignments
                                         where assign.UserProfileId == userId
                                         select assign).Count() > 0;
                Course course = await Database.Course.GetAsync(courseId);
                if (course.UserProfileId != userId && !isAdmin && !isCourseAssigned)
                {
                    return new OperationDetails(false, "You can't delete this answer. This answer has been created by other user so apply to the course creator for the permission.", "Answer");
                }
                Answer answer = await Database.Answer.GetAsync(id);
                if (answer != null)
                {
                    await Database.Answer.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Answer deleting completed successfully.", "Answer");
                }
                return new OperationDetails(false, "Answer with this Id doesn't exists. Deleting is impossible.", "Answer");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Answer");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
