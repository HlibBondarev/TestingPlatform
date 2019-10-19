using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.DTO;
using SiteWithAuthentication.BLL.Infrastructure;

namespace SiteWithAuthentication.BLL.Interfaces
{
    public interface ITestingService :IDisposable
    {
        // Search methods interface.
        Task<IList<QuestionDTO>> GetRandomQuestionsForTopic(int topicId);
        Task<IList<QuestionDTO>> GetRandomQuestionsForCourse(int courseId);
    }
}
