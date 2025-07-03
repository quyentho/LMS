using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    //public class CacheRepository : IStudentRepository 
    //{

    //    public CacheRepository(IStudentRepository studentRepository, IMemoryCache memory)
    //    {
            
    //    }
    //}
    public class StudentRepository : IGenericRepository<Student>
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(int? studentId, Expression<Func<Student, object>>? include = null)
        {
            var query = _dbContext.Students.Where(s => s.Status != Constants.Enums.Status.Deleted);

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

        public async Task<int> AddAsync(Student student)
        {
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            return student.Id;
        }

        public Task<int> DeleteAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<Student?> GetByIdAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Student course)
        {
            throw new NotImplementedException();
        }
    }
}
