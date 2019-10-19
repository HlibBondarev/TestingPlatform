using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class SpecialityDTO
    {
        public int SpecialityId { get; set; }
        public int SubjectId { get; set; }
        public string SpecialityName { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }

        public virtual ICollection<CourseDTO> Courses { get; set; }
        public virtual SubjectDTO Subject { get; set; }
    }
}
