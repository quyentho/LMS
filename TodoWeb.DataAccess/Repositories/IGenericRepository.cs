using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<int> AddAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync(int? entityId, Expression<Func<T, object>>? include = null);
        Task<T?> GetByIdAsync(int entityId);
        Task<int> UpdateAsync(T entity);
    }
}
