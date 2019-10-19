using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SiteWithAuthentication.BLL.Infrastructure;

namespace SiteWithAuthentication.BLL.Interfaces
{
    public interface ICommonService<T> : IDisposable where T : class
    {
        // Search methods interface.
        IEnumerable<T> GetAll();
        Task<T> GetAsync(int id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);

        // CRUD methods interface.
        Task<OperationDetails> CreateAsync(T item, string userId);
        Task<OperationDetails> UpdateAsync(T item, string userId);
        Task<OperationDetails> DeleteAsync(int id, string userId);
    }
}
