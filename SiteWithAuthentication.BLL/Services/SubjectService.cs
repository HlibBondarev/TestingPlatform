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
using AutoMapper;

namespace SiteWithAuthentication.BLL.Services
{
    class SubjectService : ICommonService<SubjectDTO>
    {
        IUnitOfWork Database { get; set; }

        public SubjectService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<SubjectDTO> GetAll()
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                IEnumerable<Subject> source = Database.Subject.GetAll().ToList();
                return iMapper.Map<IEnumerable<Subject>, IEnumerable<SubjectDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SubjectDTO> GetAsync(int id)
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                Subject source = await Database.Subject.GetAsync(id);
                return iMapper.Map<Subject, SubjectDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SubjectDTO> Find(Func<SubjectDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Func<Subject, bool> converted = d => predicate(
                    new SubjectDTO
                    {
                        SubjectId = d.SubjectId,
                        SubjectName = d.SubjectName,
                        Description = d.Description,
                        IsApproved = d.IsApproved
                    });
                IEnumerable<Subject> source = Database.Subject.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Subject>, IEnumerable<SubjectDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(SubjectDTO item, string userId)
        {
            try
            {
                // Checking for: does the subject with the same name already exist in DB?
                IEnumerable<Subject> subjects = Database.Subject.Find(obj => obj.SubjectName.Trim() == item.SubjectName.Trim());
                if (subjects.Count() == 0)
                {
                    Subject subject = new Subject
                    {
                        SubjectName = item.SubjectName.Trim(),
                        Description = item.Description?.Trim(),
                        LastModifiedDateTime = DateTime.Now,
                        IsApproved = item.IsApproved
                    };
                    Database.Subject.Create(subject);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subject adding completed successfully.", "Subject");
                }
                else
                {
                    return new OperationDetails(false, "Subject with the same name has already existed in DB.", "Subject");
                }
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subject");
            }
        }
        public async Task<OperationDetails> UpdateAsync(SubjectDTO item, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Subject subject = await Database.Subject.GetAsync(item.SubjectId);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't update this subject. It can only be updated by Admin.", "Subject");
                }
                if (subject != null)
                {
                    // Checking for: does the subject with the same name already exist in DB?
                    IEnumerable<Subject> subjects = Database.Subject.Find(obj => obj.SubjectName.Trim() == item.SubjectName.Trim());
                    if (subjects.ToList().Count > 0 && item.SubjectName.Trim() != subject.SubjectName.Trim())
                    {
                        return new OperationDetails(false, "Subject with the same name has already existed in DB.", "Subject");
                    }
                    subject.SubjectName = item.SubjectName.Trim();
                    subject.Description = item.Description?.Trim();
                    subject.LastModifiedDateTime = DateTime.Now;
                    subject.IsApproved = item.IsApproved;
                    Database.Subject.Update(subject);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subject updating completed successfully.", "Subject");
                }
                return new OperationDetails(false, "Subject with this Id doesn't exists.", "Subject");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subject");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                Subject subject = await Database.Subject.GetAsync(id);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't update this subject. It can only be deleted by Admin.", "Subject");
                }
                if (subject != null)
                {
                    if (subject.Specialities.Count > 0)
                    {
                        return new OperationDetails(false, "You can't delete this subject. Before you have to delete depended specialities.", "Subject");
                    }
                    await Database.Subject.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Subject deleting completed successfully.", "Subject");
                }
                return new OperationDetails(false, "Subject with this Id doesn't exists. Deleting is impossible.", "Subject");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Subject");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
