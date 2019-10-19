using System.Collections.Generic;

namespace SiteWithAuthentication.BLL.DTO
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }

        public virtual ICollection<SpecialityDTO> Specialities { get; set; }
    }
}
