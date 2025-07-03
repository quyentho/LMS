using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class, IEntity
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly DbContext _dbContext;

        public GenericRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync(int? courseId, Expression<Func<T, object>>? include = null)
        {
            var query = _dbSet.AsQueryable();
            if (courseId.HasValue)
            {
                query = query.Where(c => c.Id == courseId);

                if (!query.Any())
                {
                    return Enumerable.Empty<T>();
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int courseId)
        {
            return await _dbSet.FindAsync(courseId);
        }

        public async Task<int> AddAsync(T course)
        {
            await _dbSet.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course.Id;
        }

        public async Task<int> UpdateAsync(T course)
        {
            _dbSet.Update(course);

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            var course = await GetByIdAsync(courseId);

            if (course == null)
            {
                throw new Exception("T not found.");
            }

            _dbSet.Remove(course);

            return await _dbContext.SaveChangesAsync();
        }
    }
}
