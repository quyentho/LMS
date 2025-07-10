using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class GenericRepository<T>: IGenericRepository<T>
        where T : class, IEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(int? entityId, Expression<Func<T, object>>? include = null)
        {
            var query = _dbSet.AsQueryable();
            if (entityId.HasValue)
            {
                query = query.Where(c => c.Id == entityId);

                if (!query.Any())
                {
                    return Enumerable.Empty<T>();
                }
            }

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int entityId)
        {
            return await _dbSet.FindAsync(entityId);
        }

        public async Task<int> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);

            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }
    }
}
