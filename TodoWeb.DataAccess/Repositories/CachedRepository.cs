using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TodoWeb.DataAccess.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public class CachedRepository<T> : IGenericRepository<T>
        where T : IEntity
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IGenericRepository<T> _decoratee;

        public CachedRepository(IMemoryCache memoryCache, IGenericRepository<T> decoratee)
        {
            this._memoryCache = memoryCache;
            this._decoratee = decoratee;
        }
        public async Task<int> AddAsync(T entity)
        {
            _memoryCache.Set<T>(entity.Id.ToString(), entity);
            return await _decoratee.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(int entityId)
        {
            _memoryCache.Remove(entityId.ToString());
            return await _decoratee.DeleteAsync(entityId);
        }

        public async Task<IEnumerable<T>> GetAllAsync(int? entityId, Expression<Func<T, object>>? include = null)
        {
            Type type = typeof(T);
            var entityName = type.FullName;

            string key = entityId.HasValue
                ? GetKeyFromId(entityId.Value, type)
                : entityName + "All";

            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(2);

                return await _decoratee.GetAllAsync(entityId, include);
            }) ?? Enumerable.Empty<T>();
        }

        public async Task<T?> GetByIdAsync(int entityId)
        {
            Type type = typeof(T);
            string key = GetKeyFromId(entityId, type);
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(2);
                return await _decoratee.GetByIdAsync(entityId);
            });
        }

        private static string GetKeyFromId(int entityId, Type type)
        {
            var entityName = type.FullName;

            string key = $"{entityName}_{entityId}";
            return key;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            _memoryCache.Remove(GetKeyFromId(entity.Id, typeof(T)));
            return await _decoratee.UpdateAsync(entity);
        }
    }
}
