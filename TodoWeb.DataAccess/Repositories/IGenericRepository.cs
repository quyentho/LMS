using System.Linq.Expressions;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Infrastructures.Repositories
{
    public interface IGenericRepository<T> where T : class, IBaseEntity
    {
        // Get operations
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // Synchronous versions for backward compatibility
        T? GetById(int id);
        T? GetById(int id, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // Query operations
        IQueryable<T> Query();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query(params Expression<Func<T, object>>[] includes);
        IQueryable<T> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // Count operations
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);

        // Exists operations
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        bool Exists(Expression<Func<T, bool>> predicate);

        // Add operations
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Remove operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void RemoveById(int id);        // Soft delete operations (for entities implementing IDelete)
        void SoftDelete(T entity);
        void SoftDeleteById(int id);
    }
}
