using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class CachedRepository<T> : IGenericRepository<T>
        where T : class, IEntity
    {
        private readonly IGenericRepository<T> _decoratee;
        private readonly IMemoryCache _memoryCache;

        public CachedRepository(ApplicationDbContext dbContext, IGenericRepository<T> decoratee, IMemoryCache memoryCache)
        {
            this._decoratee = decoratee;
            this._memoryCache = memoryCache;
        }

        public async Task<int> AddAsync(T entity)
        {
            /// preprocessing logic can be added here
            var cacheKey = GetCacheKey(entity.Id);

            _memoryCache.Set(cacheKey, entity);

            return await _decoratee.AddAsync(entity);
        }

        private static string GetCacheKey(int entityId)
        {
            return $"{typeof(T).FullName}_{entityId}";
        }

        public async Task<int> DeleteAsync(T entity)
        {
            _memoryCache.Remove(GetCacheKey(entity.Id));

            return await _decoratee.DeleteAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(int? entityId, Expression<Func<T, object>>? include = null)
        {
            var cacheKey = entityId.HasValue
                ? GetCacheKey(entityId.Value)
                : $"{typeof(T).FullName}_All";

            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30); // Set cache expiration time
                return await _decoratee.GetAllAsync(entityId, include);
            }) ?? Enumerable.Empty<T>();
        }

        public async Task<T?> GetByIdAsync(int entityId)
        {
            var cacheKey = GetCacheKey(entityId);

            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30); // Set cache expiration time
                return await _decoratee.GetByIdAsync(entityId);
            });
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _memoryCache.Remove(GetCacheKey(entity.Id));
            _memoryCache.Set(GetCacheKey(entity.Id), entity);
            return await _decoratee.UpdateAsync(entity);
        }
    }
}
