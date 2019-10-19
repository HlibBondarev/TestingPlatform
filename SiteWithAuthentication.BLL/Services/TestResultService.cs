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
using AutoMapper;

namespace SiteWithAuthentication.BLL.Services
{
    class TestResultService : ICommonService<TestResultDTO>
    {
        IUnitOfWork Database { get; set; }
        public TestResultService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<TestResultDTO> GetAll()
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                IEnumerable<TestResult> source = Database.TestResult.GetAll().ToList();
                return iMapper.Map<IEnumerable<TestResult>, IEnumerable<TestResultDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TestResultDTO> GetAsync(int id)
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                TestResult source = await Database.TestResult.GetAsync(id);
                return iMapper.Map<TestResult, TestResultDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<TestResultDTO> Find(Func<TestResultDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Func<TestResult, bool> converted = d => predicate(
                    new TestResultDTO
                    {
                        TestResultId = d.TestResultId,
                        UserProfileId = d.UserProfileId,
                        TestDate = d.TestDate,
                        Result = d.Result,
                        MaxScore = d.MaxScore,
                        IsPassedTest = d.IsPassedTest,
                        IsTopicTest = d.IsTopicTest
                    });
                IEnumerable<TestResult> source = Database.TestResult.Find(converted).ToList();
                return iMapper.Map<IEnumerable<TestResult>, IEnumerable<TestResultDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(TestResultDTO item, string userId)
        {
            try
            {
                if (item.UserProfileId == null)
                {
                    return new OperationDetails(false, "You cannot create a test result because you are not authorized.", "TestResult");
                }
                // Create the new test result.
                TestResult testResult = new TestResult
                {
                    UserProfileId = item.UserProfileId,
                    TestDate = DateTime.Now,
                    Result = item.Result,
                    MaxScore = item.MaxScore,
                    IsPassedTest = item.IsPassedTest,
                    IsTopicTest = item.IsTopicTest
                };
                Database.TestResult.Create(testResult);
                await Database.SaveAsync();
                item.TestResultId = testResult.TestResultId;
                return new OperationDetails(true, "Test result adding completed successfully.", "TestResult");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "TestResult");
            }
        }

        // These methods are unnecessary!!!
        public async Task<OperationDetails> UpdateAsync(TestResultDTO item, string userId)
        {
            try
            {
                if (item.UserProfileId == null)
                {
                    return new OperationDetails(false, "You cannot update a test result because you are not authorized.", "TestResult");
                }
                // Update the test result.
                TestResult testResult = await Database.TestResult.GetAsync(item.TestResultId);
                if (testResult != null)
                {
                    testResult.UserProfileId = item.UserProfileId;
                    testResult.TestDate = DateTime.Now;
                    testResult.Result = item.Result;
                    testResult.MaxScore = item.MaxScore;
                    testResult.IsPassedTest = item.IsTopicTest;
                };
                Database.TestResult.Update(testResult);
                await Database.SaveAsync();
                return new OperationDetails(true, "Test result updating completed successfully.", "TestResult");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "TestResult");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                // Checking for: Does the current user have permission for deleting test results?
                TestResult testResult = await Database.TestResult.GetAsync(id);
                if (testResult.UserProfileId != userId && !isAdmin)
                {
                    return new OperationDetails(false, "You can't delete this test result. This test result has been created by other user so apply to the course creator for the permission.", "TestResult");
                }
                if (testResult != null)
                {
                    await Database.Question.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Test result deleting completed successfully.", "TestResult");
                }
                return new OperationDetails(false, "Test result with this Id doesn't exists. Deleting is impossible.", "TestResult");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "TestResult");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
