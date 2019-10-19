using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;
using SiteWithAuthentication.BLL.Interfaces;
using SiteWithAuthentication.BLL.Util;
using SiteWithAuthentication.DAL.Entities;
using SiteWithAuthentication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteWithAuthentication.BLL.Services
{
    class SpecialityService : ICommonService<SpecialityDTO>
    {
        IUnitOfWork Database { get; set; }

        public SpecialityService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Search methods.
        public IEnumerable<SpecialityDTO> GetAll()
        {
            // AutoMapper Setup.
            var iMapper = BLLAutoMapper.GetMapper;
            try
            {
                IEnumerable<Speciality> source = Database.Speciality.GetAll().ToList();
                return iMapper.Map<IEnumerable<Speciality>, IEnumerable<SpecialityDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SpecialityDTO> GetAsync(int id)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;

                Speciality source = await Database.Speciality.GetAsync(id);
                return iMapper.Map<Speciality, SpecialityDTO>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<SpecialityDTO> Find(Func<SpecialityDTO, bool> predicate)
        {
            try
            {
                // AutoMapper Setup.
                var iMapper = BLLAutoMapper.GetMapper;
                Func<Speciality, bool> converted = d => predicate(
                    new SpecialityDTO
                    {
                        SpecialityId = d.SpecialityId,
                        SubjectId = d.SubjectId,
                        SpecialityName = d.SpecialityName,
                        Description = d.Description,
                        IsApproved = d.IsApproved
                    });

                IEnumerable<Speciality> source = Database.Speciality.Find(converted).ToList();
                return iMapper.Map<IEnumerable<Speciality>, IEnumerable<SpecialityDTO>>(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CRUD methods.
        public async Task<OperationDetails> CreateAsync(SpecialityDTO item, string userId)
        {
            try
            {
                // Checking for:  is the speciality subject approved?
                if (!(await Database.Subject.GetAsync(item.SubjectId)).IsApproved)
                {
                    return new OperationDetails(false, "The subject of the specialty being created is not approved.", "Speciality");
                }
                // Checking for: does the speciality with the same name already exist in DB?
                IEnumerable<Speciality> specialities = Database.Speciality.Find(obj => obj.SubjectId == item.SubjectId && obj.SpecialityName.Trim() == item.SpecialityName.Trim());
                if (specialities.ToList().Count == 0)
                {
                    Speciality speciality = new Speciality
                    {
                        SubjectId = item.SubjectId,
                        SpecialityName = item.SpecialityName.Trim(),
                        Description = item.Description?.Trim(),
                        LastModifiedDateTime = DateTime.Now,
                        IsApproved = item.IsApproved
                    };
                    Database.Speciality.Create(speciality);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Speciality adding completed successfully.", "Speciality");
                }
                else
                {
                    return new OperationDetails(false, "Speciality with the same name has already existed in DB.", "Speciality");
                }
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Speciality");
            }
        }
        public async Task<OperationDetails> UpdateAsync(SpecialityDTO item, string userId)
        {
            try
            {
                // Checking for:  is the speciality subject approved?
                if (!(await Database.Subject.GetAsync(item.SubjectId)).IsApproved)
                {
                    return new OperationDetails(false, "The subject of the specialty being updated is not approved.", "Speciality");
                }
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                var speciality = await Database.Speciality.GetAsync(item.SpecialityId);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't update this speciality. It can only be updated by Admin.", "Speciality");
                }
                if (speciality != null)
                {
                    // Checking for: does the speciality with the same name already exist in DB?
                    IEnumerable<Speciality> specialities = Database.Speciality.Find(obj => obj.SubjectId == item.SubjectId && obj.SpecialityName.Trim() == item.SpecialityName.Trim());
                    if (specialities.ToList().Count > 0 && item.SpecialityName.Trim() != speciality.SpecialityName.Trim())
                    {
                        return new OperationDetails(false, "Speciality with the same name has already existed in DB.", "Speciality");
                    }
                    // Update speciality.
                    speciality.SpecialityName = item.SpecialityName.Trim();
                    speciality.Description = item.Description?.Trim();
                    speciality.LastModifiedDateTime = DateTime.Now;
                    speciality.IsApproved = item.IsApproved;
                    Database.Speciality.Update(speciality);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Speciality updating completed successfully.", "Speciality");
                }
                return new OperationDetails(false, "Speciality with this Id doesn't exists.", "Speciality");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Speciality");
            }
        }
        public async Task<OperationDetails> DeleteAsync(int id, string userId)
        {
            try
            {
                // Checking for: does the current user has a role - "admin"?
                bool isAdmin = BLLRepository.IsAdmin(Database, userId);
                var speciality = await Database.Speciality.GetAsync(id);
                if (!isAdmin)
                {
                    return new OperationDetails(false, "You can't update this speciality. It can only be updated by Admin.", "Speciality");
                }
                if (speciality != null)
                {
                    if (speciality.Courses.Count > 0)
                    {
                        return new OperationDetails(false, "You can't delete this speciality. Before you have to delete depended courses.", "Speciality");
                    }
                    await Database.Speciality.DeleteAsync(id);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Speciality deleting completed successfully.", "Speciality");
                }
                return new OperationDetails(false, "Speciality with this Id doesn't exists. Deleting is impossible.", "Speciality");
            }
            catch (Exception ex)
            {
                return new OperationDetails(false, ex.Message, "Speciality");
            }
        }

        // Disposing method.
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
