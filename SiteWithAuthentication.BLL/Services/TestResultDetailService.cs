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
    class TestResultDetailService : ICommonService<TestResultDetailDTO>
    {
        IUnitOfWork Database { get; set; }
        public TestResultDetailService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<TestResultDetailDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<TestResultDetail> source = Database.TestResultDetail.GetAll().ToList();
                return iMapper.Map<IEnumerable<TestResultDetail>, IEnumerable<TestResultDetailDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TestResultDetailDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                TestResultDetail source = await Database.TestResultDetail.GetAsync(id);
                return iMapper.Map<TestResultDetail, TestResultDetailDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<TestResultDetailDTO> Find(Func<TestResultDetailDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<TestResultDetail, bool> converted = d => predicate(
                    new TestResultDetailDTO
                    {
                        TestResultDetailId = d.TestResultDetailId,
                        TestResultId = d.TestResultId,
                        QuestionId = d.QuestionId,
                        IsProperAnswer = d.IsProperAnswer,
                    });

                IEnumerable<TestResultDetail> source = Database.TestResultDetail.Find(converted).ToList();
                return iMapper.Map<IEnumerable<TestResultDetail>, IEnumerable<TestResultDetailDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(TestResultDetailDTO item, string userId)
        {
            try
            {
                if (Database.TestResult.Find(o => o.TestResultId == item.TestResultId).FirstOrDefault().UserProfileId == null)
                {
                    return new OperationDetails(false, "You cannot create a test result detail because you are not authorized.", "TestResultDetail");
                }
                // Create the new test result.
                TestResultDetail testResultDetail = new TestResultDetail
                {
                    TestResultId = item.TestResultId,
                    QuestionId = item.QuestionId,
                    IsProperAnswer = item.IsProperAnswer
                };
                Database.TestResultDetail.Create(testResultDetail);
                await Database.SaveAsync();
                return new OperationDetails(true, "Test result detail adding completed successfully.", "TestResultDetail");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "TestResultDetail");
            }
        }

        // These methods are unnecessary!!!
        public Task<OperationDetails> UpdateAsync(TestResultDetailDTO item, string userId)
        {
            throw new NotImplementedException();
        }
        public Task<OperationDetails> DeleteAsync(int id, string userId)
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
