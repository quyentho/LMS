using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public class StudentRepository: IStudentRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentRepository(IApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Student>> GetStudentAsync(int? studentId, Expression<Func<Student, object>> include)
        {
            var query = _dbContext.Students.Where(s => s.Status != Constants.Enums.Status.Deleted)
                .AsQueryable();

            if (studentId.HasValue)
            {
                query = query.Where(x => x.Id == studentId);
            }

            query = query.Include(include);

            return await query.ToListAsync();
        }
    }
}
