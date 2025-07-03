using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class CacheRepository : IStudentRepository 
    {

        public CacheRepository(IStudentRepository studentRepository, IMemoryCache memory)
        {
            
        }
        public async Task<IEnumerable<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>>? include)
        {
            var student = Memory.get(studentId)

            if (student != null)
            {
                return student;
            }
            
            var students = await studentRepository.GetStudentsAsync(studentId, include);
        }

    }
    public class StudentRepository : IStudentRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>>? include)
        {
            var query = _dbContext.Students.Where(s => s.Status != Constants.Enums.Status.Deleted)
                .AsQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(s => s.Id == studentId.Value);
            }

            if (include != null)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
    }
}
