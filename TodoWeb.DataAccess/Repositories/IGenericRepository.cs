using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : IEntity 
    {
        Task<int> AddAsync(T course);
        Task<int> DeleteAsync(int courseId);
        Task<T?> GetByIdAsync(int courseId);
        Task<IEnumerable<T>> GetAllAsync(int? courseId, Expression<Func<T, object>>? include = null);
        Task<int> UpdateAsync(T course);
    }
}
