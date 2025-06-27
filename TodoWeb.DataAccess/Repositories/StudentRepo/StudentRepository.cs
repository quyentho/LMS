using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.Application.Extensions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentRepository(IApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>> include)
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

        public async Task<List<Student>> GetPagedStudentsAsync(
            int? schoolId,
            string? sortBy,
            bool isDescending,
            int? pageSize,
            int? pageIndex)
        {

            var query = _dbContext.Students
                .Where(student => student.Status != Constants.Enums.Status.Deleted)
                .Include(student => student.School)
                .AsQueryable();

            if (schoolId.HasValue)
            {
                query = query.Where(x => x.School.Id == schoolId);//add theem ddk 
            }


            // Map sortBy string into a list of Expression selectors  
            Expression<Func<Student, object>>[] sortSelectors;
            if (string.IsNullOrEmpty(sortBy))
            {
                sortSelectors = [];
            }
            else
            {
                sortSelectors = sortBy.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => (Expression<Func<Student, object>>)(f.Trim() switch
                    {
                        "id" => student => student.Id,
                        "age" => student => student.Age,
                        "fullname" => student => student.FirstName + " " + student.LastName,
                        "schoolname" => student => student.School.Name,
                        "balance" => student => student.Balance,
                        _ => student => student.Id
                    })).ToArray();
            }

            if (pageSize.HasValue && pageIndex == null)
            {
                pageIndex = 1;
            }
            if (pageIndex.HasValue && pageSize == null)
            {
                pageSize = 5;
            }

            query = query
                .ApplySort(isDescending, sortSelectors)
                .ApplyPaging(pageIndex, pageSize)
                .AsQueryable();

            return await query.ToListAsync();
        }
    }
}
