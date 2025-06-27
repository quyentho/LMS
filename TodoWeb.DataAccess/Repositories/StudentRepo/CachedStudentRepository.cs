using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public class CachedStudentRepository : ICachedStudentRepository
    {
        private const string ALL_STUDENTS_CACHE_KEY = "AllStudents";
        private const int CACHE_DURATION_SECONDS = 30;
        
        private readonly IStudentRepository _studentRepository;
        private readonly IMemoryCache _memoryCache;

        public CachedStudentRepository(IStudentRepository studentRepository, IMemoryCache memoryCache)
        {
            _studentRepository = studentRepository;
            _memoryCache = memoryCache;
        }

        public async Task<List<Student>> GetAllStudentsWithCacheAsync(Expression<Func<Student, object>> include)
        {
            var cacheKey = $"{ALL_STUDENTS_CACHE_KEY}_{include}";
            
            var cachedStudents = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(CACHE_DURATION_SECONDS);
                return await _studentRepository.GetStudentsAsync(null, include);
            });

            return cachedStudents ?? new List<Student>();
        }

        public void InvalidateCache()
        {
            // Remove all student-related cache entries
            var cacheKeys = new[]
            {
                ALL_STUDENTS_CACHE_KEY,
                $"{ALL_STUDENTS_CACHE_KEY}_*" // Pattern for include-specific cache keys
            };

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }
        }

        // Delegate all other methods to the wrapped repository
        public async Task<List<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>> include)
        {
            return await _studentRepository.GetStudentsAsync(studentId, include);
        }

        public async Task<List<Student>> GetPagedStudentsAsync(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex)
        {
            return await _studentRepository.GetPagedStudentsAsync(schoolId, sortBy, isDescending, pageSize, pageIndex);
        }
    }
}