using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : IEntity 
    {
        Task<int> AddAsync(T entity);
        Task<int> DeleteAsync(int entityId);
        Task<T?> GetByIdAsync(int entityId);
        Task<IEnumerable<T>> GetAllAsync(int? entityId, Expression<Func<T, object>>? include = null);
        Task<int> UpdateAsync(T entity);
    }
}
